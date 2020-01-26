using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace MediaHandler.Extensions
{
    public static class SecureStringExtension
    {
        public static string Password(this SecureString password)
        {
            return new NetworkCredential(string.Empty, password)
                .Password;
        }
    }
}
