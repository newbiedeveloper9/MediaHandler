using System.Collections.Generic;
using System.Threading.Tasks;
using fbchat_sharp.API;

namespace MediaHandler.Interfaces
{
    public interface IFbThreadService
    {
        FB_Thread Thread { get; set; }
        Task SendMessage(FB_Message message);
        Task<IList<FB_Message>> GetLastMessages(int amount = 10);
        bool AmIAuthor(string messageAuthor);
    }
}