using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using MediaHandler.PubSub;

namespace MediaHandler.ViewModels
{
    public class MasterViewModel : Conductor<object>,
        IHandle<ISubNotification<Navigate>>,
        IHandle<ISubNotification<INotificationPopup>>
    {
        private readonly IEventAggregator _eventAggregator;
        private Dictionary<Navigate, PropertyChangedBase> navigateDictionary;

        public ShellViewModel ShellViewModel { get; set; }
        public LoginViewModel LoginViewModel { get; set; }

        public MasterViewModel()
        {

        }

        public MasterViewModel(IEventAggregator eventAggregator,
            ShellViewModel shellViewModel, LoginViewModel loginViewModel)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            ShellViewModel = shellViewModel;
            LoginViewModel = loginViewModel;

            ActivateItem(LoginViewModel);

            navigateDictionary = new Dictionary<Navigate, PropertyChangedBase>()
            {
                {Navigate.Shell, ShellViewModel },
                {Navigate.Login, LoginViewModel }
            };
        }


        private SnackbarMessageQueue _messagesQueue = new SnackbarMessageQueue();
        public SnackbarMessageQueue MessagesQueue
        {
            get => _messagesQueue;
            set
            {
                if (_messagesQueue == value) return;
                _messagesQueue = value;
                NotifyOfPropertyChange(() => MessagesQueue);
            }
        }

        public void Handle(ISubNotification<Navigate> message)
        {
            ActivateItem(navigateDictionary[message.Obj]);
        }


        public async void Handle(ISubNotification<INotificationPopup> message) =>
            await Task.Run(() => MessagesQueue.Enqueue($"{message.Obj.Title}: {message.Obj.Message}"));
    }
}
