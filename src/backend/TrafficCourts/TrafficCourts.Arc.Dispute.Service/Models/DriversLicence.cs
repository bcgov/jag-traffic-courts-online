namespace TrafficCourts.Arc.Dispute.Service.Models
{
    public static class DriversLicence
    {
        private static readonly int[] _digitWeights = { 1, 2, 5, 3, 6, 4, 8, 7 };
        public static string WithCheckDigit(string driversLicence)
        {
            if (string.IsNullOrEmpty(driversLicence))
            {
                // Return an empty string for drivers licence if the drivers licence is null or empty.
                // It should appear as 9 blank characters on ARC file. (TCVP-2802)
                driversLicence = string.Empty;
                return driversLicence;
            }

            // ensure we are left padded to 8 characters
            driversLicence = driversLicence.PadLeft(8, '0');

            int sum = 0;

            for (int i = 0; i <= 7; i++)
            {
                sum += (driversLicence[i] - '0') * _digitWeights[i];
            }

            // Calculate the check digit by taking the last digit of the modulus. (TCVP-2829)
            char checkDigit = Convert.ToChar((sum % 11) % 10 + '0');

            return driversLicence + checkDigit;
        }
    }
}
