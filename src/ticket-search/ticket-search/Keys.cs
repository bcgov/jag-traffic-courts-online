using System;

namespace Gov.TicketSearch
{
    // TODO: refactor and remove this static class

    public static class Keys
    {

        public static string RsiOperationModeFake = "FAKE";
        public static string Nothing = "n/a";

        public static int TicketDiscountValidDays =
            Environment.GetEnvironmentVariable("TICKET_DISCOUNT_VALID_DAYS") == null
                ? 30
                : Convert.ToInt32(Environment.GetEnvironmentVariable("TICKET_DISCOUNT_VALID_DAYS"));
    }
}
