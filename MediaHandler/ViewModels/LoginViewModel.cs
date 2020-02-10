using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using MediaHandler.Extensions;
using MediaHandler.Interfaces;
using MediaHandler.PubSub;

namespace MediaHandler.ViewModels
{
    public class LoginViewModel : PropertyChangedBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IFbLoginService _fbLoginService;

        public LoginViewModel(IEventAggregator eventAggregator, IFbLoginService fbLoginService)
        {
            _eventAggregator = eventAggregator;

            _fbLoginService = fbLoginService;
            _fbLoginService.Set2FA(GetAuthCode);
            _ = TryCookiesLogin();
        }

        #region Properties
        private string _loginText;
        public string LoginText
        {
            get => _loginText;
            set
            {
                if (_loginText == value) return;
                _loginText = value;
                NotifyOfPropertyChange(() => LoginText);
                NotifyOfPropertyChange(() => CanTryLogin);
            }
        }


        private SecureString _passwordText;
        public SecureString PasswordText
        {
            get => _passwordText;
            set
            {
                if (_passwordText == value) return;
                _passwordText = value;
                NotifyOfPropertyChange(() => PasswordText);
                NotifyOfPropertyChange(() => CanTryLogin);

            }
        }

        private bool _locked;
        bool locked
        {
            get => _locked;
            set
            {
                _locked = value;
                NotifyOfPropertyChange(() => CanTryLogin);
            }
        }

        public bool CanTryLogin => !string.IsNullOrWhiteSpace(LoginText) &&
                                   !string.IsNullOrWhiteSpace(PasswordText.Password()) &&
                                   !locked;
        #endregion Properties

        #region Methods

        public async void TryLogin()
        {
            locked = true;

            var result = await _fbLoginService.Login(LoginText, PasswordText);

            if (result)
            {
                _eventAggregator.BeginPublishOnUIThread(
                    new SubNotification<Navigate>(Navigate.Shell));
            }
            else
            {
                _eventAggregator.BeginPublishOnUIThread(
                    new SubNotification<INotificationPopup>(
                        new NotificationPopup("Login", "Failed")));
            }

            locked = false;
        }

        public async Task<string> GetAuthCode()
        {
            Console.WriteLine("2FA code: ");
            return Console.ReadLine();
        }

        private async Task TryCookiesLogin()
        {
            locked = true;

            var result = await _fbLoginService.Login("", new SecureString());

            if (result)
            {

                _eventAggregator.BeginPublishOnUIThread(
                    new SubNotification<INotificationPopup>(
                        new NotificationPopup("Login", "Cookies found. Fetching data...")));

                _eventAggregator.BeginPublishOnUIThread(
                    new SubNotification<Navigate>(Navigate.Shell));
            }

            locked = false;
        }

        #endregion Methods
    }
}
