namespace TrafficCourts.Domain.Models
{
    /// <summary>
    /// Extensions for domain models to handle time zone conversions.
    /// </summary>
    public static class ModelExtensions
    {
        #region JJDispute

        #region UTC to Local Time
        public static void UtcToLocalTime(this IEnumerable<JJDispute> records, TimeZoneInfo timeZone)
        {
            ArgumentNullException.ThrowIfNull(records);
            ArgumentNullException.ThrowIfNull(timeZone);

            foreach (var record in records)
            {
                record.UtcToLocalTime(timeZone);
            }
        }

        public static void UtcToLocalTime(this JJDispute record, TimeZoneInfo timeZone)
        {
            ArgumentNullException.ThrowIfNull(record);
            ArgumentNullException.ThrowIfNull(timeZone);

            record.JjDecisionDate = record.JjDecisionDate.UtcToLocalTime(timeZone);
            record.SubmittedTs = record.SubmittedTs.UtcToLocalTime(timeZone);
            record.VtcAssignedTs = record.VtcAssignedTs.UtcToLocalTime(timeZone);

            record.JjDisputedCounts?.UtcToLocalTime(timeZone);
        }

        private static void UtcToLocalTime(this IEnumerable<JJDisputedCount> records, TimeZoneInfo timeZone)
        {
            foreach (var record in records)
            {
                record.UtcToLocalTime(timeZone);
            }
        }

        private static void UtcToLocalTime(this JJDisputedCount record, TimeZoneInfo timeZone)
        {
            record.LatestPleaUpdateTs = record.LatestPleaUpdateTs.UtcToLocalTime(timeZone);
        }
        #endregion

        #region Local to UTC Time

        public static void LocalToUtcTime(this JJDispute record, TimeZoneInfo timeZone)
        {
            ArgumentNullException.ThrowIfNull(record);
            ArgumentNullException.ThrowIfNull(timeZone);

            record.JjDecisionDate = record.JjDecisionDate.LocalToUtcTime(timeZone);
            record.SubmittedTs = record.SubmittedTs.LocalToUtcTime(timeZone);
            record.VtcAssignedTs = record.VtcAssignedTs.LocalToUtcTime(timeZone);

            record.JjDisputedCounts?.LocalToUtcTime(timeZone);
        }

        private static void LocalToUtcTime(this IEnumerable<JJDisputedCount> records, TimeZoneInfo timeZone)
        {
            foreach (var record in records)
            {
                record.LocalToUtcTime(timeZone);
            }
        }

        private static void LocalToUtcTime(this JJDisputedCount record, TimeZoneInfo timeZone)
        {
            record.LatestPleaUpdateTs = record.LatestPleaUpdateTs.LocalToUtcTime(timeZone);
        }

        #endregion

        #endregion


        #region Dispute

        public static void UtcToLocalTime(this Dispute record, TimeZoneInfo timeZone)
        {
            ArgumentNullException.ThrowIfNull(record);
            ArgumentNullException.ThrowIfNull(timeZone);

            record.SubmittedTs = record.SubmittedTs.UtcToLocalTime(timeZone);
        }

        public static void LocalToUtcTime(this Dispute record, TimeZoneInfo timeZone)
        {
            record.SubmittedTs = record.SubmittedTs.LocalToUtcTime(timeZone);
        }

        #endregion

        public static void UtcToLocalTime(this IEnumerable<EmailHistory> records, TimeZoneInfo timeZone)
        {
            ArgumentNullException.ThrowIfNull(records);
            ArgumentNullException.ThrowIfNull(timeZone);

            foreach (var record in records)
            {
                record.UtcToLocalTime(timeZone);
            }
        }

        private static void UtcToLocalTime(this EmailHistory record, TimeZoneInfo timeZone)
        {
            ArgumentNullException.ThrowIfNull(record);
            ArgumentNullException.ThrowIfNull(timeZone);

            record.EmailSentTs = record.EmailSentTs.UtcToLocalTime(timeZone);
        }

        public static void UtcToLocalTime(this IEnumerable<DisputeListItem> records, TimeZoneInfo timeZone)
        {
            ArgumentNullException.ThrowIfNull(records);
            ArgumentNullException.ThrowIfNull(timeZone);

            foreach (var record in records)
            {
                record.UtcToLocalTime(timeZone);
            }
        }

        private static void UtcToLocalTime(this DisputeListItem record, TimeZoneInfo timeZone)
        {
            ArgumentNullException.ThrowIfNull(record);
            ArgumentNullException.ThrowIfNull(timeZone);

            record.SubmittedTs = record.SubmittedTs.UtcToLocalTime(timeZone);
            record.JjDecisionDate = record.JjDecisionDate.UtcToLocalTime(timeZone);
        }



        public static void UtcToLocalTime(this DisputeCaseFileSummary record, TimeZoneInfo timeZone)
        {
            ArgumentNullException.ThrowIfNull(record);
            ArgumentNullException.ThrowIfNull(timeZone);

            record.SubmittedTs = record.SubmittedTs.UtcToLocalTime(timeZone);
            record.JjDecisionDate = record.JjDecisionDate.UtcToLocalTime(timeZone);
            record.VtcAssignedTs = record.VtcAssignedTs.UtcToLocalTime(timeZone);
        }


        public static void UtcToLocalTime(this OccamDisputeListItemModel record, TimeZoneInfo timeZone)
        {
            ArgumentNullException.ThrowIfNull(record);
            ArgumentNullException.ThrowIfNull(timeZone);

            record.submittedTs = record.submittedTs.UtcToLocalTime(timeZone);
            record.jjDecisionDate = record.jjDecisionDate.UtcToLocalTime(timeZone);
        }

        public static void UtcToLocalTime(this OccamDisputeWithUpdateRequestListItemModel record, TimeZoneInfo timeZone)
        {
            ArgumentNullException.ThrowIfNull(record);
            ArgumentNullException.ThrowIfNull(timeZone);

            record.submittedTs = record.submittedTs.UtcToLocalTime(timeZone);
            record.jjDecisionDate = record.jjDecisionDate.UtcToLocalTime(timeZone);
        }
    }
}
