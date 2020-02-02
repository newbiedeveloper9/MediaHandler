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
    public class ThreadViewModel : PropertyChangedBase, IThread,
        IHandle<IThreadNotification<NewMessageStruct>>, IDeactivate,
        IDisposable
    {
        bool disposed = false;

        private ScrollViewerLogic _scrollViewerLogic;
        private string ThreadId => FbThreadService.Thread.uid;


        private IEventAggregator _eventAggregator;
        public IFbThreadService FbThreadService { get; set; }
        public SolidColorBrush MyColor { get; set; } = new SolidColorBrush(Colors.Purple);
        public SolidColorBrush FriendColor { get; set; } = new SolidColorBrush(Colors.Green);

        #region .ctor
        public ThreadViewModel()
        {

        }

        public ThreadViewModel(IEventAggregator eventAggregator, IFbThreadService fbService)
        {
            _eventAggregator = eventAggregator;
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

        private BindableCollection<MessageModel> _messageCollection;
        public BindableCollection<MessageModel> MessageCollection
        {
            get => _messageCollection;
            set
            {
                if (_messageCollection == value) return;
                _messageCollection = value;
                NotifyOfPropertyChange(() => MessageCollection);
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
            MessageCollection = new BindableCollection<MessageModel>();
            var lastMessages = await FbThreadService.GetLastMessages();
            foreach (var lastMessage in lastMessages.Reverse())
            {
                var newMessage = new NewMessageStruct(ThreadId, lastMessage);
                Handle(new ThreadNotification<NewMessageStruct>(newMessage));
            }
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

        public void SendChatMessage()
        {
            var message = new FB_Message(ChatMessage, is_from_me: true, thread_id: ThreadId);
            FbThreadService.SendMessage(message);

            // var messageStruct = new NewMessageStruct(ThreadId, message);
            // Handle(new ThreadNotification<NewMessageStruct>(messageStruct));
            //TODO Create a buffer for messages to show them quicker or if a message has not been sent successfully then mark it
        }
        #endregion Methods

        public void Handle([NotNull] IThreadNotification<NewMessageStruct> message)
        {
            var messageShort = message.Obj.Message;
            if (!message.Obj.ThreadId.Equals(ThreadId,
                StringComparison.OrdinalIgnoreCase)) return;

            var newMessage = new MessageModel()
            {
                Color = FriendColor,
                IsOwnMessage = FbThreadService.AmIAuthor(messageShort.author),
                Time = DateTime.Now,
                Texts = { messageShort.text }
            };

            if (MessageCollection.Count <= 0)
            {
                MessageCollection.Add(newMessage);
                return;
            }

            var lastMessage = MessageCollection[MessageCollection.Count - 1];
            if (lastMessage.IsOwnMessage != FbThreadService.AmIAuthor(messageShort.author))
            {
                MessageCollection.Add(newMessage);
            }
            else
            {
                lastMessage.Texts.Add(messageShort.text);
            }
        }

        #region CloseWindow
        public void Deactivate(bool close)
        {
            Dispose();
        }

        public event EventHandler<DeactivationEventArgs> AttemptingDeactivation;
        public event EventHandler<DeactivationEventArgs> Deactivated;

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                _eventAggregator.Unsubscribe(this);
                _eventAggregator = null;
                FbThreadService.Thread = null;
                FbThreadService = null;
                _scrollViewerLogic = null;
            }

            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ThreadViewModel()
        {
            Dispose(false);
        }
        #endregion CloseWindow
    }
}
