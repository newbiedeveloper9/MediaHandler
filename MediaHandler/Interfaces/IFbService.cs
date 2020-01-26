using System.Collections.Generic;
using System.Security;
using System.Threading.Tasks;
using fbchat_sharp.API;
using MediaHandler.Models;

namespace MediaHandler.Interfaces
{
    public interface IFbService
    {
        Task<bool> Login(string login, SecureString password);

        /// <param name="amount">If is null then returns every thread</param>
        Task<IList<FB_Thread>> GetLastThreads(int amount = 10);

        Task<IList<ThreadPreviewStruct>> GetThreadPreviews(IList<string> threadUid);
        Task<IList<FB_Thread>> GetActivePersons();
        Task Logout();
    }
}