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
using MediaHandler;
using MediaHandler.Interfaces;
using MediaHandler.Models;
using MediaHandler.Services;

namespace MediaHandler.ViewModels
{
    public class ShellViewModel : Conductor<object>, IShell
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
            await Login();

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

        private async Task Login()
        {
            var result = await _fbService.Login("", null);
            if (result) return;

            //TODO create form for login
            Console.WriteLine("Login: ");
            var login = Console.ReadLine();
            Console.WriteLine("Password: ");
            var secureString = new NetworkCredential("", Console.ReadLine()).SecurePassword;
            await _fbService.Login(login, secureString);
        }

        public void OpenThread([NotNull] ThreadPreviewModel threadModel)
        {
            var chatViewModel = IoC.Get<IThread>();
            chatViewModel.FbThreadService.Thread = threadModel.Thread;
            chatViewModel.Initialize();

            _windowManager.ShowWindow(chatViewModel);
        }
    }
}