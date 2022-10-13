using AutoFixture;
using System.Collections.Generic;
using TrafficCourts.Common.Features.Mail.Templates;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using Xunit;

namespace TrafficCourts.Common.Test.Features.Mail.Templates
{
    public class EmailTemplateTest
    {
        [Theory]
        [MemberData(nameof(EmailTemplates))]
        public void create_email_from_template(IEmailTemplate<Dispute> disputeEmailTemplate)
        {
            Fixture fixture = new Fixture();
            var dispute = fixture.Create<Dispute>();
            var actual = disputeEmailTemplate.Create(dispute);

            Assert.NotNull(actual);
        }

        public static IEnumerable<object[]> EmailTemplates()
        {
            yield return new object[] { new CancelledDisputeEmailTemplate() };
            yield return new object[] { new ConfirmationEmailTemplate() };
            yield return new object[] { new ProcessingDisputeEmailTemplate() };
            yield return new object[] { new RejectedDisputeEmailTemplate() };
            yield return new object[] { new VerificationEmailTemplate() };
        }
    }
}
