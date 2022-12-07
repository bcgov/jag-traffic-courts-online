using AutoFixture;
using AutoMapper;
using System;
using System.Collections.Generic;
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

            FixCountNumbers(tcoDisputeTicket);

            tcoDisputeTicket.DriversLicence = new Random().Next(99999999).ToString(); //must be numeric
            var expectedMvbClientNumber = DriversLicence.WithCheckDigit(tcoDisputeTicket.DriversLicence);

            var now = DateTime.Now;
            DisputeTicketToArcFileRecordListConverter.Now = () => now;

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

            // AutoFixture should have created 3 each of these
            Assert.Equal(3, tcoDisputeTicket.TicketDetails.Count);
            Assert.Equal(3, tcoDisputeTicket.DisputeCounts.Count);
            Assert.Equal(6, actual.Count);

            DateTime expectedTransactionDateTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, DateTimeKind.Local);

            for (int i = 0; i < 3; i++)
            {
                // first record will be AdnotatedTicket, the second record will be DisputedTicket
                var actualAdnotatedTicket = Assert.IsType<AdnotatedTicket>(actual[i * 2]);
                var actualDisputedTicket = Assert.IsType<DisputedTicket>(actual[i * 2 + 1]);

                Assert.Equal(tcoDisputeTicket.TicketDetails[i].Count.ToString("D3"), actualAdnotatedTicket.CountNumber);
                Assert.Equal(tcoDisputeTicket.DisputeCounts[i].Count.ToString("D3"), actualDisputedTicket.CountNumber);

                Assert.Equal(expectedTransactionDateTime.AddSeconds(i * 2), actualAdnotatedTicket.TransactionDateTime);
                Assert.Equal(expectedTransactionDateTime.AddSeconds(i * 2 + 1), actualDisputedTicket.TransactionDateTime);

                Assert.Equal(tcoDisputeTicket.TicketFileNumber.ToUpper() + "  01", actualAdnotatedTicket.FileNumber);
                Assert.Equal(expectedMvbClientNumber, actualAdnotatedTicket.MvbClientNumber);


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

                Assert.Equal(tcoDisputeTicket.StreetAddress?.ToUpper(), actualDisputedTicket.StreetAddress);
                Assert.Equal(tcoDisputeTicket.City?.ToUpper(), actualDisputedTicket.City);
                Assert.Equal(tcoDisputeTicket.Province?.ToUpper(), actualDisputedTicket.Province);
                Assert.Equal(tcoDisputeTicket.PostalCode?.ToUpper(), actualDisputedTicket.PostalCode);
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
#pragma warning disable xUnit1026 // Theory methods should use all of their parameters
        public void can_parse_LegalSection(int code, string act, string section, string description)
#pragma warning restore xUnit1026 // Theory methods should use all of their parameters
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

        private void FixCountNumbers(TcoDisputeTicket disputedTicket)
        {
            // map the count numbers to normal ranges

            if (disputedTicket?.TicketDetails is not null)
            {
                int count = 1;
                foreach (var item in disputedTicket.TicketDetails!)
                {
                    item.Count = count++;
                }
            }

            if (disputedTicket?.DisputeCounts is not null)
            {
                int count = 1;
                foreach (var item in disputedTicket!.DisputeCounts!)
                {
                    item.Count = count++;
                }
            }

        }
    }
}
