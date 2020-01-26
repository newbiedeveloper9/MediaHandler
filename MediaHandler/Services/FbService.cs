using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using fbchat_sharp.API;
using MediaHandler.Extensions;
using MediaHandler.Interfaces;
using MediaHandler.Models;

namespace MediaHandler.Services
{
    public class FbService : IFbService
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly FbClient _fbClient;

        public FbService(IEventAggregator eventAggregator, FbClient fbClient)
        {
            _eventAggregator = eventAggregator;
            _fbClient = fbClient;
        }

        public async Task<bool> Login(string login, SecureString password)
        {
            var result = await _fbClient.TryLogin();
            if(!result) 
                result = await _fbClient.DoLogin(login, password.Password());
            if (result)
                await _fbClient.StartListening();
            return result;
        }

        public async Task<IList<FB_Thread>> GetLastThreads(int amount = 10)
        {
            return await _fbClient.fetchThreadList(amount);
        }

        public async Task<IList<ThreadPreviewStruct>> GetThreadPreviews(IList<string> threadUid)
        {
            List<ThreadPreviewStruct> threadsPreview = new List<ThreadPreviewStruct>();

            foreach (var uid in threadUid)
            {
                var lastMessData = await _fbClient.fetchThreadMessages(uid, 1);
                if (lastMessData.Count <= 0)
                    continue;
                var message = lastMessData[0];

                var users = await _fbClient.fetchUserInfo(new List<string>() { message.author });
                users.TryGetValue(message.author, out var author);

                threadsPreview.Add(
                    new ThreadPreviewStruct(isRead: message.is_read, text: message.text, author: author?.name));
            }

            return threadsPreview;
        }

        public async Task<IList<FB_Thread>> GetActivePersons()
        {
            var activeUsers = await _fbClient.fetchActiveUsers();
            var threadInfos = await _fbClient.fetchThreadInfo(activeUsers);
            return threadInfos.Values.ToList();
        }

        public async Task Logout()
        {
            await _fbClient.DoLogout();
        }
    }
}
