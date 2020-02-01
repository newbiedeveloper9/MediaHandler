using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using Caliburn.Micro;
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

        #endregion Properties

        #region Methods

        public async Task Initialize()
        {
            MessageCollection = new BindableCollection<MessageModel>();
            var lastMessages = await FbThreadService.GetLastMessages();
            foreach (var lastMessage in lastMessages)
            {
                var newMessage = new NewMessageStruct(FbThreadService.Thread.uid, lastMessage);
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

        public void SendChatMessage(string message)
        {
            Console.WriteLine(message);
        }
        #endregion Methods

        public void Handle([NotNull] IThreadNotification<NewMessageStruct> message)
        {
            if (!message.Obj.ThreadId.Equals(FbThreadService.Thread.uid,
                StringComparison.OrdinalIgnoreCase)) return;

            if (MessageCollection.Count <= 0)
            {

                return;
            }

            var lastMessage = MessageCollection[MessageCollection.Count - 1];
            if (lastMessage.IsOwnMessage)
            {
                MessageCollection.Add(new MessageModel()
                {
                    Color = FriendColor, IsOwnMessage = false, Time = DateTime.Now, Texts = { message.Obj.Message.text }
                });
            }
            else
            {
                lastMessage.Texts.Add(message.Obj.Message.text);
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
