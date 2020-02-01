using System.Collections.Generic;
using System.Threading.Tasks;
using fbchat_sharp.API;
using MediaHandler.Interfaces;

namespace MediaHandler.Services
{
    class FbThreadService 
        : IFbThreadService
    {
        public FB_Thread Thread { get; set; }
        private readonly FbClient _fbClient;

        public FbThreadService(FbClient fbClient)
        {
            _fbClient = fbClient;
        }

        public async Task SendMessage(FB_Message message)
        {
            await _fbClient.send(message, Thread.uid, Thread.type);
        }

        public async Task<IList<FB_Message>> GetLastMessages(int amount = 10)
        {
            return await _fbClient.fetchThreadMessages(Thread.uid, amount);
        }
    }
}
