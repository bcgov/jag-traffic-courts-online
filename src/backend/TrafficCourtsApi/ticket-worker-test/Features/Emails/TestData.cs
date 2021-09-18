using Gov.TicketWorker.Test;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Gov.TicketWorker.Features.Emails
{
    /// <summary>
    /// Provides test data for validation of email filter logic
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = Justifications.UnitTestClass)]
    public static class TestData
    {
        /// <summary>
        /// Gets valid email addresses
        /// </summary>
        public static IEnumerable<object[]> GetValidEmails()
        {
            yield return new object[] { "bob@example.ca" };
            yield return new object[] { "bob@example.net" };
            yield return new object[] { "bob@example.org" };
            yield return new object[] { "bob@example.com" };
            yield return new object[] { "bob-smith@example.ca" };
            yield return new object[] { "bob-smith@example.net" };
            yield return new object[] { "bob-smith@example.org" };
            yield return new object[] { "bob-smith@example.com" };
            yield return new object[] { "bob.smith@example.ca" };
            yield return new object[] { "bob.smith@example.net" };
            yield return new object[] { "bob.smith@example.org" };
            yield return new object[] { "bob.smith@example.com" };
        }

        /// <summary>
        /// Gets invalid email addresses
        /// </summary>
        public static IEnumerable<object[]> GetInvalidEmails()
        {
            yield return new object[] { "bob @example.com" };
            yield return new object[] { "bob@example com" };
            yield return new object[] { "bob@example" };
        }
    }
}
