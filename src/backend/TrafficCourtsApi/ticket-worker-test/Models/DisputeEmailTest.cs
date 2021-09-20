using AutoFixture;
using Gov.TicketWorker.Models;
using Gov.TicketWorker.Test;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficCourts.Common.Contract;
using Xunit;

namespace ticket_worker_test.Models
{
    [ExcludeFromCodeCoverage(Justification = Justifications.UnitTestClass)]
    public class DisputeEmailTest
    {
        [Theory]
        [InlineData(3)]
        [InlineData(2)]
        [InlineData(1)]
        public void TicketDisputeContract_DisputeEmail_MappingIsValid(int offenseCount)
        {
            // arrange
            var fixture = new Fixture();
            var expected = fixture.Create<TicketDisputeContract>();

            while (expected.Offences.Count > offenseCount)
            {
                expected.Offences.RemoveAt(0);
            }

            // act
            var actual = new DisputeEmail(expected);

            // assert

            Assert.Equal(expected.Offences[0].ReductionAppearInCourt, actual.CountOneWillAppear);
            Assert.Equal(expected.Offences[0].OffenceDescription, actual.CountOneDescription);
            Assert.Equal(expected.Offences[0].ReductionAppearInCourt, actual.CountOneWillAppear);
            Assert.Equal(expected.Offences[0].AmountDue, actual.CountOneAmount); 

            if (offenseCount >= 2)
            {
                Assert.Equal(expected.Offences[1].ReductionAppearInCourt, actual.CountTwoWillAppear);
                Assert.Equal(expected.Offences[1].OffenceDescription, actual.CountTwoDescription);
                Assert.Equal(expected.Offences[1].ReductionAppearInCourt, actual.CountTwoWillAppear);
                Assert.Equal(expected.Offences[1].AmountDue, actual.CountTwoAmount);
            }

            if (offenseCount > 2)
            {
                Assert.Equal(expected.Offences[2].ReductionAppearInCourt, actual.CountThreeWillAppear);
                Assert.Equal(expected.Offences[2].OffenceDescription, actual.CountThreeDescription);
                Assert.Equal(expected.Offences[2].ReductionAppearInCourt, actual.CountThreeWillAppear);
                Assert.Equal(expected.Offences[2].AmountDue, actual.CountThreeAmount);
            }

            Assert.Equal(expected.Additional.InterpreterLanguage, actual.InterpreterLanguage);
            Assert.Equal(expected.Additional.InterpreterRequired, actual.RequireInterpreter);
            Assert.Equal(expected.Additional.WitnessPresent, actual.CallWitness);
            Assert.Equal(expected.Additional.NumberOfWitnesses, actual.NumberofWitnesses);
            Assert.Equal(expected.ConfirmationNumber, actual.ConfirmationNumber);
            Assert.Equal(expected.ViolationDate, actual.ViolationDate);
            Assert.Equal(expected.ViolationTicketNumber, actual.ViolationTicketNumber);
            Assert.Equal(expected.ViolationTime, actual.ViolationTime);
            Assert.Equal(expected.Additional.InterpreterLanguage, actual.InterpreterLanguage);
        }
    }
}
