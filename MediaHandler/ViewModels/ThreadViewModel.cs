using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using Caliburn.Micro;
using JetBrains.Annotations;
using MediaHandler.Interfaces;
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

        #endregion Properties

        #region Methods
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
        #endregion Methods

        public void Handle([NotNull] IThreadNotification<NewMessageStruct> message)
        {
            if (message.Obj.ThreadId.Equals(FbThreadService.Thread.uid, 
                StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine(FbThreadService.Thread.name + ": " + message.Obj.Message.text);
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
