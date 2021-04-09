using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DisputeApi.Web
{
    public static class Keys
    {
        public static readonly string OAUTH_TOKEN_KEY = "oauth-token";
        public static readonly int OAUTH_TOKEN_EXPIRE_BUFFER = 60;
        public static string RSI_OPERATION_MODE_FAKE = "FAKE";
    }
}
