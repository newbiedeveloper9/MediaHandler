using System;
using System.Security;
using System.Threading.Tasks;

namespace MediaHandler.Interfaces
{

    public interface IFbLoginService
    {
        Task<bool> Login(string login, SecureString password);

        void Set2FA(Func<Task<string>> task);
    }
}