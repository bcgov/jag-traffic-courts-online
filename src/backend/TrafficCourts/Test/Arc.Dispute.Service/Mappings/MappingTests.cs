using AutoFixture;
using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficCourts.Arc.Dispute.Service.Mappings;
using TrafficCourts.Arc.Dispute.Service.Models;
using TrafficCourts.Common;
using Xunit;

namespace TrafficCourts.Test.Arc.Dispute.Service.Mappings
{
    public class MappingTests
    {
        [Fact]
        public void Test_convert_data_from_TcoDisputeTicket_to_ArcFileRecord_list()
        {
            // Arrange
            var fixture = new Fixture();
            var tcoDisputeTicket = fixture.Create<TcoDisputeTicket>();

            tcoDisputeTicket.DriversLicence = new Random().Next(99999999).ToString(); //must be numeric
            var expectedMvbClientNumber = DriversLicence.WithCheckDigit(tcoDisputeTicket.DriversLicence);

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            var mapper = mockMapper.CreateMapper();

            // Act
            var actual = mapper.Map<List<ArcFileRecord>>(tcoDisputeTicket);

            // Assert
            Assert.NotNull(actual);
            Assert.NotEmpty(actual);

            foreach (var actualRec in actual)
            {
                var expectedTimestamp = DateTime.Now;
                var expectedHM = new DateTime(expectedTimestamp.Year, expectedTimestamp.Month, expectedTimestamp.Day, expectedTimestamp.Hour, expectedTimestamp.Minute, 0);

                var actualTimestamp = actualRec.TransactionDate;
                var actualHM = new DateTime(actualTimestamp.Year, actualTimestamp.Month, actualTimestamp.Day, actualTimestamp.Hour, actualTimestamp.Minute, 0);
                Assert.Equal(expectedHM, actualHM);
                Assert.Equal(tcoDisputeTicket.TicketFileNumber.ToUpper() + "  01", actualRec.FileNumber);
                Assert.Equal(expectedMvbClientNumber, actualRec.MvbClientNumber);

                if (actualRec is AdnotatedTicket)
                {
                    AdnotatedTicket actualAdnotatedTicket = (AdnotatedTicket)actualRec;
                    // TODO: Need to add a separate test for reversing the name later
                    //Assert.Equal(tcoDisputeTicket.CitizenName.ToUpper(), actualAdnotatedTicket.Name);
                    Assert.Contains(tcoDisputeTicket.TicketDetails, _ => _.Section?.ToUpper() == actualAdnotatedTicket.Section);
                    Assert.Contains(tcoDisputeTicket.TicketDetails, _ => _.Subsection?.ToUpper() == actualAdnotatedTicket.Subsection);
                    Assert.Contains(tcoDisputeTicket.TicketDetails, _ => _.Paragraph?.ToUpper() == actualAdnotatedTicket.Paragraph);
                    Assert.Contains(tcoDisputeTicket.TicketDetails, _ => _.Subparagraph?.ToUpper() == actualAdnotatedTicket.Subparagraph);
                    Assert.Contains(tcoDisputeTicket.TicketDetails, _ => _.Act.ToUpper() == actualAdnotatedTicket.Act);
                    Assert.Contains(tcoDisputeTicket.TicketDetails, _ => _.Amount == actualAdnotatedTicket.OriginalAmount);
                    Assert.Equal(tcoDisputeTicket.IssuingOrganization.ToUpper(), actualAdnotatedTicket.Organization);
                    Assert.Equal(tcoDisputeTicket.IssuingLocation.ToUpper(), actualAdnotatedTicket.OrganizationLocation);
                }
                else
                {
                    DisputedTicket actualDisputedTicket = (DisputedTicket)actualRec;
                    // TODO: Need to add a separate test for reversing the name later
                    //Assert.Equal(tcoDisputeTicket.CitizenName.ToUpper(), actualDisputedTicket.Name);
                    Assert.Equal(tcoDisputeTicket.StreetAddress?.ToUpper(), actualDisputedTicket.StreetAddress);
                    Assert.Equal(tcoDisputeTicket.City?.ToUpper(), actualDisputedTicket.City);
                    Assert.Equal(tcoDisputeTicket.Province?.ToUpper(), actualDisputedTicket.Province);
                    Assert.Equal(tcoDisputeTicket.PostalCode?.ToUpper(), actualDisputedTicket.PostalCode);
                }
            }  
        }

        /// <summary>
        /// These LegalSection tests really should be in their own test class file
        /// </summary>
        /// <param name="code"></param>
        /// <param name="act"></param>
        /// <param name="section"></param>
        /// <param name="description"></param>
        [Theory]
        [CsvData(@"../../../Arc.Dispute.Service/Data/statutes-test.csv")]
        public void can_parse_LegalSection(int code, string act, string section, string description)
        {
            bool actual = LegalSection.TryParse(section, out LegalSection? legalSection);

            Assert.True(actual);
            Assert.NotNull(legalSection);
            Assert.Equal(section, legalSection!.ToString());
        }

        /// <summary>
        /// These LegalSection tests really should be in their own test class file
        /// </summary>
        [Theory]
        [InlineData("")]
        [InlineData("(a)")]
        [InlineData("x(a)")]
        [InlineData("(a)(i)")]
        public void will_not_parse_invalid_LegalSection(string section)
        {
            bool actual = LegalSection.TryParse(section, out LegalSection? legalSection);
            Assert.False(actual);
            Assert.Null(legalSection);
        }
    }
}
