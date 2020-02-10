using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using JetBrains.Annotations;
using MaterialDesignThemes.Wpf;
using MediaHandler;
using MediaHandler.Interfaces;
using MediaHandler.Models;
using MediaHandler.PubSub;
using MediaHandler.Services;

namespace MediaHandler.ViewModels
{
    public class ShellViewModel : PropertyChangedBase
    {
        private readonly IWindowManager _windowManager;
        private readonly IFbService _fbService;

        public ShellViewModel()
        {

        }

        public ShellViewModel(IEventAggregator eventAggregator,
            IWindowManager windowManager,
            IFbService fbService)
        {
            this._windowManager = windowManager;
            this._fbService = fbService;

            _ = Initialize();
        }

        #region Properties

        private BindableCollection<ThreadPreviewModel> _threadCollection;
        public BindableCollection<ThreadPreviewModel> ThreadCollection
        {
            get => _threadCollection;
            set
            {
                if (_threadCollection == value) return;
                _threadCollection = value;
                NotifyOfPropertyChange(() => ThreadCollection);
            }
        }

        #endregion Properties

        private async Task Initialize()
        {
            ThreadCollection = new BindableCollection<ThreadPreviewModel>();

            var threads = await _fbService.GetLastThreads(20);
            var threadsId = threads.Select(thread => thread.uid).ToList();

            var list = await _fbService.GetThreadPreviews(threadsId);
            for (var i = 0; i < list.Count; i++)
            {
                var threadPreview = list[i];

                ThreadCollection.Add(
                ThreadPreviewModel.Create(threadPreview, threads[i]));
            }
        }

        public async void OpenThread(ThreadPreviewModel threadModel)
        {
            var chatViewModel = IoC.Get<IThread>();
            chatViewModel.FbThreadService.Thread = threadModel.Thread;
            await chatViewModel.Initialize();

            _windowManager.ShowWindow(chatViewModel);
        }
    }
}