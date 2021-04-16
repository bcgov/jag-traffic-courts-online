using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DisputeApi.Web
{
    public static class Keys
    {
        public static readonly string OauthTokenKey = "oauth-token";
        public static readonly int OauthTokenExpireBuffer = 60;
        public static string RsiOperationModeFake = "FAKE";
        public static string Nothing = "n/a";
        public static int TicketDiscountValidDays =
            Environment.GetEnvironmentVariable("TICKET_DISCOUNT_VALID_DAYS") == null
                ? 30
                : Convert.ToInt32(Environment.GetEnvironmentVariable("TICKET_DISCOUNT_VALID_DAYS"));
    }
}
