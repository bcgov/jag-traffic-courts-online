using AutoFixture;
using Gov.CitizenApi.Features;
using Gov.CitizenApi.Features.AddressAutoComplete;
using Gov.CitizenApi.Test.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Gov.CitizenApi.Features.AddressAutoComplete.AddressAutocompleteClient;

namespace Gov.CitizenApi.Test.Features.AddressAutoComplete
{
    [ExcludeFromCodeCoverage]
    public class AddressAutocompleteControllerTest
    {
        private readonly Mock<IAddressAutocompleteClient> _serviceMock;

        public AddressAutocompleteControllerTest()
        {
            _serviceMock = new Mock<IAddressAutocompleteClient>();
        }

        [Fact]
        public void throw_ArgumentNullException_if_passed_null()
        {
            Assert.Throws<ArgumentNullException>(() => new AddressAutocompleteController(null));
        }

        [Fact]
        public async Task Find_ReturnsNotNull_ActualResponseEqualsExpected_with_OK()
        {
            // Arrange
            Fixture fixture = new Fixture();

            var expected = (fixture.Create<IEnumerable<AddressAutocompleteFindResponse>>());
            _serviceMock.Setup(x => x.Find(It.Is<string>(s => !String.IsNullOrEmpty(s)),null))
                .Returns(Task.FromResult(expected));
            var sut = new AddressAutocompleteController(_serviceMock.Object);

            // Act

            var actual = await sut.Find("searchterm");
            var actualObjectResult = (ObjectResult)actual;

            // Assert

            var actualEnumerablelValue = Assert.IsAssignableFrom<IEnumerable<AddressAutocompleteFindResponse>>(actualObjectResult.Value);
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<OkObjectResult>(actual);
            
            Assert.Equal(200, (actualObjectResult.StatusCode));

            // The Fixture returns a value of type ConvertedEnumerable and not IEnumerable and thus 
            // values are being to converted into arrays to compare values
            var actualValueArray = actualEnumerablelValue.ToArray<AddressAutocompleteFindResponse>();
            Assert.Equal<AddressAutocompleteFindResponse[]>(expected.ToArray<AddressAutocompleteFindResponse>(), actualValueArray);
        }

        [Fact]
        public async Task Find_ReturnsBadRequestResponse_WhenSearchtermisNull()
        {
            //Arrange
            var sut = new AddressAutocompleteController(_serviceMock.Object);

            // Act
            var actual = await sut.Find(null);

            // Assert
            Assert.IsType<BadRequestResult>(actual);
        }

        [Fact]
        public async Task Retrieve_ReturnsNotNull_ActualResponseEqualsExpected_with_OK()
        {
            // Arrange
            Fixture fixture = new Fixture();

            var expected = (fixture.Create<IEnumerable<AddressAutocompleteRetrieveResponse>>());
            _serviceMock.Setup(x => x.Retrieve(It.Is<string>(s => !String.IsNullOrEmpty(s))))
                .Returns(Task.FromResult(expected));
            var sut = new AddressAutocompleteController(_serviceMock.Object);

            // Act

            var actual = await sut.Retrieve("retrieveid");
            var actualObjectResult = (ObjectResult)actual;

            // Assert

            var actualEnumerablelValue = Assert.IsAssignableFrom<IEnumerable<AddressAutocompleteRetrieveResponse>>(actualObjectResult.Value);
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<OkObjectResult>(actual);

            Assert.Equal(200, (actualObjectResult.StatusCode));

            // The Fixture returns a value of type ConvertedEnumerable and not IEnumerable and thus 
            // values are being to converted into arrays to compare values
            var actualValueArray = actualEnumerablelValue.ToArray<AddressAutocompleteRetrieveResponse>();
            Assert.Equal<AddressAutocompleteRetrieveResponse[]>(expected.ToArray<AddressAutocompleteRetrieveResponse>(), actualValueArray);
        }


        [Fact]
        public async Task Retrieve_ReturnsBadRequestResponse_WhenIDisNull()
        {
            //Arrange
            var sut = new AddressAutocompleteController(_serviceMock.Object);

            // Act
            var actual = await sut.Retrieve(null);

            // Assert
            Assert.IsType<BadRequestResult>(actual);
        }
    }
}
