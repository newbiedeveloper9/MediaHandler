using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using JetBrains.Annotations;
using MediaHandler;
using MediaHandler.Interfaces;
using MediaHandler.Models;
using MediaHandler.Services;
using Action = System.Action;

namespace MediaHandler.ViewModels
{
    public class ShellViewModel : Conductor<object>, IShell
    {
        private string _text;
        public string Text
        {
            get => _text;
            set
            {
                if (_text == value) return;
                _text = value;
                NotifyOfPropertyChange(() => Text);
            }
        }

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

        private readonly IWindowManager _windowManager;
        private IFbService _fbService;

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

        private async Task Initialize()
        {
            //TODO YOUR CREDENTIALS LOGIN HERE
            var secureString = new NetworkCredential("", "password").SecurePassword;
            await _fbService.Login("login", secureString);

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

        public void OpenThread([NotNull] ThreadPreviewModel threadModel)
        {
            var chatViewModel = IoC.Get<IThread>();
            chatViewModel.FbThreadService.Thread = threadModel.Thread;

            _windowManager.ShowWindow(chatViewModel);
        }
    }
}