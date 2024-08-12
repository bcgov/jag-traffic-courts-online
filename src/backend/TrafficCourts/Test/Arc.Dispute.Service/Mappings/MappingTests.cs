﻿using AutoFixture;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TrafficCourts.Arc.Dispute.Service.Mappings;
using TrafficCourts.Arc.Dispute.Service.Models;
using TrafficCourts.Common;
using TrafficCourts.Domain.Models;
using Xunit;

namespace TrafficCourts.Test.Arc.Dispute.Service.Mappings
{
    public class MappingTests
    {
        private readonly IMapper _mapper;

        public MappingTests()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });

            _mapper = configuration.CreateMapper();
        }

        /// <summary>
        /// Create a dispute ticket with the specified number of ticket and dispute counts
        /// </summary>
        private static TcoDisputeTicket CreateDisputeTicket(int ticketCount, int disputeCount)
        {
            var dispute = new TcoDisputeTicket();

            for (int i = 0; i < ticketCount; i++)
            {
                dispute.TicketDetails.Add(new TicketCount { Count = i + 1, Act = "MVA" });
            }

            for (int i = 0; i < disputeCount; i++)
            {
                dispute.DisputeCounts.Add(new TrafficCourts.Arc.Dispute.Service.Models.DisputeCount { Count = i + 1 });
            }

            return dispute;
        }

        #region Citizen Name Mapping

        public static IEnumerable<object?[]> CitizenNameMappingTestCases
        {
            get
            {
                // parameters: string? citizenName, string? surname, string? given1, string? given2, string? given3, string expected

                // all null should map to empty string
                yield return new object?[] { null, null, null, null, "" };
                // all empty strings should map to empty string
                yield return new object?[] { "", "", "", "", "" };
                // all white space should map to empty string
                yield return new object?[] { " ", " ", " ", " ", "" };

                // formats with surname, given1, given2 and given3

                // last name only
                yield return new object?[] { "Doe", "", "", "", "Doe,".ToUpper() };
                yield return new object?[] { "Doe", "", "", "", "Doe,".ToUpper() };

                // last name + given 1
                yield return new object?[] { "Doe", "John", "", "", "Doe, John".ToUpper() };
                yield return new object?[] { "Doe", "John", "", "", "Doe, John".ToUpper() };

                // last name + given 1  + given 2
                yield return new object?[] { "Doe", "John", "James", "", "Doe, John James".ToUpper() };
                yield return new object?[] { "Doe", "John", "James", "", "Doe, John James".ToUpper() };

                // last name + given 1  + given 2 + given 3
                yield return new object?[] { "Doe", "John", "James", "Jack", "Doe, John James Jack".ToUpper() };
                yield return new object?[] { "Doe", "John", "James", "Jack", "Doe, John James Jack".ToUpper() };
            }
        }

        [Theory]
        [MemberData(nameof(CitizenNameMappingTestCases))]
        public void client_name_is_mapped_correctly_with_no_disputed_count(string? surname, string? given1, string? given2, string? given3, string expected)
        {
            // mapper requires at least one ticket count
            var dispute = CreateDisputeTicket(1, 0);
            dispute.Surname = surname;
            dispute.GivenName1 = given1;
            dispute.GivenName2 = given2;
            dispute.GivenName3 = given3;

            // act
            var actual = _mapper.Map<List<ArcFileRecord>>(dispute);

            // assert
            var record = Assert.Single(actual);
            var adnotated = Assert.IsType<AdnotatedTicket>(record);

            Assert.Equal(expected, adnotated.Name);
        }

        [Theory]
        [MemberData(nameof(CitizenNameMappingTestCases))]
        public void client_name_is_mapped_correctly_with_disputed_count(string? surname, string? given1, string? given2, string? given3, string expected)
        {
            // mapper requires at least one ticket count
            var dispute = CreateDisputeTicket(1, 1);
            dispute.Surname = surname;
            dispute.GivenName1 = given1;
            dispute.GivenName2 = given2;
            dispute.GivenName3 = given3;

            // act
            var actual = _mapper.Map<List<ArcFileRecord>>(dispute);

            // assert, should be two records
            Assert.Equal(2, actual.Count);

            var adnotated = Assert.IsType<AdnotatedTicket>(actual[0]);
            var disputed = Assert.IsType<DisputedTicket>(actual[1]);

            // name should match on both records
            Assert.Equal(expected, adnotated.Name);
            Assert.Equal(expected, disputed.Name);
        }

        #endregion

        #region Citizen Street Address Mapping

        public static IEnumerable<object?[]> CitizenStreetAddressMappingTestCases
        {
            get
            {
                yield return new object?[] { null, "" }; // null to empty string
                yield return new object?[] { "", "" }; // empty string to empty string
                yield return new object?[] { " \t\r\n\f", "" }; // white space characters to empty string
                yield return new object?[] { "123 Main St", "123 Main St".ToUpper() };
                yield return new object?[] { " 123 Main St", "123 Main St".ToUpper() }; // leading space
                yield return new object?[] { "123 Main St ", "123 Main St".ToUpper() }; // trailing space
                yield return new object?[] { " 123 Main St ", "123 Main St".ToUpper() }; // leading and trailing spaces
            }
        }

        [Theory]
        [MemberData(nameof(CitizenStreetAddressMappingTestCases))]
        public void client_street_address_is_mapped_correctly(string? streetAddress, string expected)
        {
            // arrange
            var dispute = CreateDisputeTicket(1, 1);
            dispute.StreetAddress = streetAddress;

            // act
            var actual = _mapper.Map<List<ArcFileRecord>>(dispute);

            Assert.Equal(2, actual.Count);

            var adnotated = Assert.IsType<AdnotatedTicket>(actual[0]);
            var disputed = Assert.IsType<DisputedTicket>(actual[1]);

            Assert.Equal(expected, disputed.StreetAddress);
        }

        #endregion

        [Fact]
        public void Test_convert_data_from_TcoDisputeTicket_to_ArcFileRecord_list()
        {
            // Arrange
            var fixture = new Fixture();
            var tcoDisputeTicket = fixture.Create<TcoDisputeTicket>();

            FixCountNumbers(tcoDisputeTicket);

            tcoDisputeTicket.DriversLicence = new Random().Next(99999999).ToString(); //must be numeric

            var now = DateTime.Now;
            DisputeTicketToArcFileRecordListConverter.Now = () => now;

            // Act
            var actual = _mapper.Map<List<ArcFileRecord>>(tcoDisputeTicket);

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

                // count numbers should be formatted correctly on each record
                Assert.Equal(tcoDisputeTicket.TicketDetails[i].Count.ToString("D3"), actualAdnotatedTicket.CountNumber);
                Assert.Equal(tcoDisputeTicket.DisputeCounts[i].Count.ToString("D3"), actualDisputedTicket.CountNumber);

                // transaction times should be sequential by 1 second
                Assert.Equal(expectedTransactionDateTime.AddSeconds(i * 2), actualAdnotatedTicket.TransactionDateTime);
                Assert.Equal(expectedTransactionDateTime.AddSeconds(i * 2 + 1), actualDisputedTicket.TransactionDateTime);

                // FileNumber should be the upper case ticket number with two spaces and then 01
                Assert.Equal(tcoDisputeTicket.TicketFileNumber.ToUpper() + "  01", actualAdnotatedTicket.FileNumber);

                // the MvbClientNumber should be the drivers licence with check digit
                Assert.Equal(DriversLicence.WithCheckDigit(tcoDisputeTicket.DriversLicence), actualAdnotatedTicket.MvbClientNumber);

                // TODO: Need to add a separate test for reversing the name later
                //Assert.Equal(tcoDisputeTicket.CitizenName.ToUpper(), actualAdnotatedTicket.Name);
                Assert.Contains(tcoDisputeTicket.TicketDetails, _ => _.Section?.ToUpper() == actualAdnotatedTicket.Section);
                Assert.Contains(tcoDisputeTicket.TicketDetails, _ => _.Subsection?.ToUpper() == actualAdnotatedTicket.Subsection);
                Assert.Contains(tcoDisputeTicket.TicketDetails, _ => _.Paragraph?.ToUpper() == actualAdnotatedTicket.Paragraph);
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
        [CsvEmbeddedResourceData("TrafficCourts.Test.Arc.Dispute.Service.Data.statutes-test.csv")]
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

        /// <summary>
        /// Fixes up the ticket and dispute counts to be in valid range
        /// </summary>
        /// <param name="disputedTicket"></param>
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
