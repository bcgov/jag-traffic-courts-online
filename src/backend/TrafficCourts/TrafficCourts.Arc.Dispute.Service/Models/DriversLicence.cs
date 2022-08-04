namespace TrafficCourts.Arc.Dispute.Service.Models
{
    public static class DriversLicence
    {
        public static string WithCheckDigit(string driversLicence)
        {
            if (driversLicence is null)
            {
                // shouldn't happen but we dont want to throw exception
                driversLicence = string.Empty;
            }

            // ensure we are left padded to 8 characters
            driversLicence = driversLicence.PadLeft(8, '0');

            int sum = 0;

            for (int i = 0; i <= 7; i++)
            {
                sum += (driversLicence[i] - '0') * i;
            }

            char checkDigit = Convert.ToChar((sum % 11) + '0');

            return driversLicence + checkDigit;
        }
    }
}
