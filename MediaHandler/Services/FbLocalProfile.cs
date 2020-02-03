using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaHandler.Interfaces;

namespace MediaHandler.Services
{
    public class FbLocalProfile : IFbLocalProfile
    {
        private readonly FbClient _fbClient;

        public FbLocalProfile(FbClient fbClient)
        {
            _fbClient = fbClient;
            AuthorId = fbClient.GetUserUid();
        }

        public string AuthorId { get; }
    }
}
