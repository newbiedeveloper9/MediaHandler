using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using Caliburn.Micro;
using fbchat_sharp.API;
using JetBrains.Annotations;
using MediaHandler.Interfaces;
using MediaHandler.Models;
using MediaHandler.PubSub;
using Microsoft.Win32.SafeHandles;
using SharpDj.Logic.UI;

namespace MediaHandler.ViewModels
{
    public partial class ThreadViewModel : PropertyChangedBase, IThread,
        IHandle<ISubNotification<NewMessageStruct>>
    {
        private ScrollViewerLogic _scrollViewerLogic;
        private string ThreadId => FbThreadService.Thread.uid;

        private IEventAggregator _eventAggregator;
        private readonly IFbLocalProfile _fbLocalProfile;

        public IFbThreadService FbThreadService { get; set; }
        public SolidColorBrush MyColor { get; set; } = new SolidColorBrush(Colors.Purple);
        public SolidColorBrush FriendColor { get; set; } = new SolidColorBrush(Colors.Green);

        #region .ctor
        public ThreadViewModel()
        {

        }

        public ThreadViewModel(IEventAggregator eventAggregator, IFbThreadService fbService, IFbLocalProfile fbLocalProfile)
        {
            _eventAggregator = eventAggregator;
            _fbLocalProfile = fbLocalProfile;
            _eventAggregator.Subscribe(this);

            FbThreadService = fbService;
        }
        #endregion .ctor

        #region Properties

        private bool _scrollToBottomIsVisible = false;
        public bool ScrollToBottomIsVisible
        {
            get => _scrollToBottomIsVisible;
            set
            {
                if (_scrollToBottomIsVisible == value) return;
                _scrollToBottomIsVisible = value;
                NotifyOfPropertyChange(() => ScrollToBottomIsVisible);
            }
        }

        private BindableCollection<MessageModel> _messageColumnCollection;
        public BindableCollection<MessageModel> MessageColumnCollection
        {
            get => _messageColumnCollection;
            set
            {
                if (_messageColumnCollection == value) return;
                _messageColumnCollection = value;
                NotifyOfPropertyChange(() => MessageColumnCollection);
            }
        }

        private string _chatMessage;
        public string ChatMessage
        {
            get => _chatMessage;
            set
            {
                if (_chatMessage == value) return;
                _chatMessage = value;
                NotifyOfPropertyChange(() => ChatMessage);
            }
        }

        #endregion Properties

        #region Methods

        public async Task Initialize()
        {
            Console.WriteLine(FbThreadService.Thread.message_count);
            MessageColumnCollection = new BindableCollection<MessageModel>();
            var lastMessages = await FbThreadService.GetLastMessages();
            foreach (var lastMessage in lastMessages.Reverse())
            {
                var newMessage = new NewMessageStruct(ThreadId, lastMessage);
                HandleNewMessage(new SubNotification<NewMessageStruct>(newMessage), false, true);
            }

            await BackgroundBuffer();
        }

        public void ScrollLoaded(ScrollViewer scrollViewer)
        {
            _scrollViewerLogic = new ScrollViewerLogic(scrollViewer);
            _scrollViewerLogic.ScrollNotOnBottom +=
                (sender, args) => ScrollToBottomIsVisible = !_scrollViewerLogic.CanScrollDown;
        }

        public void ScrollToBottom()
        {
            _scrollViewerLogic.ScrollToDown();
        }

        public async void SendChatMessage()
        {
            if (string.IsNullOrWhiteSpace(ChatMessage)) return;
            var message = new FB_Message(ChatMessage, is_from_me: true, thread_id: ThreadId, author: _fbLocalProfile.AuthorId);
            ChatMessage = string.Empty;

            var messageStruct = new NewMessageStruct(ThreadId, message);
            HandleNewMessage(new SubNotification<NewMessageStruct>(messageStruct), true);

            await FbThreadService.SendMessage(message);
        }
        #endregion Methods

        #region Handlers

        public void Handle(ISubNotification<NewMessageStruct> message) => HandleNewMessage(message);

        private void HandleNewMessage(ISubNotification<NewMessageStruct> message, bool isLocal = false, bool isInitialize = false)
        {
            var messageParam = message.Obj.Message;
            if (!message.Obj.ThreadId.Equals(ThreadId)) return;

            var simpleMessage = new SimpleMessage(messageParam.text, DateTime.Now);
            var newMessage = new MessageModel()
            {
                IsOwnMessage = FbThreadService.AmIAuthor(messageParam.author),
                Time = DateTime.Now,
                SimpleMessageCollection = { simpleMessage }
            };
            newMessage.Color = newMessage.IsOwnMessage ? MyColor : FriendColor;

            if (MessageColumnCollection.Count <= 0)
            {
                MessageColumnCollection.Add(newMessage);
                if(isLocal)
                    _messageBuffer.Add(simpleMessage);
                return;
            }

            var lastMessageColumn = MessageColumnCollection[MessageColumnCollection.Count - 1];
            if (isLocal)
            {
                if (lastMessageColumn.IsOwnMessage != newMessage.IsOwnMessage)
                    MessageColumnCollection.Add(newMessage);
                else
                    lastMessageColumn.SimpleMessageCollection.Add(simpleMessage);

                _messageBuffer.Add(simpleMessage);
                return;
            }

            if (lastMessageColumn.IsOwnMessage != newMessage.IsOwnMessage)
            {
                MessageColumnCollection.Add(newMessage);
            }
            else
            {
                if (newMessage.IsOwnMessage && !isInitialize)
                {
                    SetMessageSuccess(simpleMessage.Message);
                    return;
                }

                lastMessageColumn.SimpleMessageCollection.Add(simpleMessage);
            }
        }
        #endregion Handlers
    }
}
