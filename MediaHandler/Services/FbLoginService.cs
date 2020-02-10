using System;
using System.Security;
using System.Threading.Tasks;
using MediaHandler.Extensions;
using MediaHandler.Interfaces;

namespace MediaHandler.Services
{
    public class FbLoginService : IFbLoginService
    {
        private readonly FbClient _fbClient;
        private Func<Task<string>> _get2FACode;


        public FbLoginService(FbClient fbClient) 
        {
            _fbClient = fbClient;
            _fbClient.Set2FACallback(Get2FACode);
        }

        public void Set2FA(Func<Task<string>> codeTask)
        {
            _get2FACode = codeTask;
        }

        private async Task<string> Get2FACode()
        {
            return await _get2FACode?.Invoke();
        }

        public async Task<bool> Login(string login, SecureString password)
        {
            var result = await _fbClient.TryLogin();
            if (!result)
                result = await _fbClient.DoLogin(login, password.Password());
            if (result)
                await _fbClient.StartListening();
            return result;
        }
    }
}
