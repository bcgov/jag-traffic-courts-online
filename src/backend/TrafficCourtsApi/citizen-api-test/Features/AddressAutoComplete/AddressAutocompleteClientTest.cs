using Gov.CitizenApi.Features.AddressAutoComplete;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Xunit;
using static Gov.CitizenApi.Features.AddressAutoComplete.AddressAutocompleteClient;

namespace Gov.CitizenApi.Test.Features.AddressAutoComplete
{
    public class AddressAutocompleteClientTest
    {
        private readonly HttpClient _httpClient;
        private readonly Mock<ILogger<AddressAutocompleteClient>> _loggerMock;
        private readonly AddressAutoCompleteClientCredentials _credentials;

        public AddressAutocompleteClientTest()
        {
            _loggerMock = new Mock<ILogger<AddressAutocompleteClient>>();
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri("http://test-canada-post.com/"),
            };

            _credentials = new AddressAutoCompleteClientCredentials
            {
                ApiKey = "TE55-HC16-KA69-TH92"
            };
        }

        [Fact]
        public void throw_ArgumentNullException_if_HttpClientpassed_null()
        {
            Assert.Throws<ArgumentNullException>(() => new AddressAutocompleteClient(null, _loggerMock.Object, _credentials));
        }

        [Fact]
        public void throw_ArgumentNullException_if_Loggerpassed_null()
        {
            Assert.Throws<ArgumentNullException>(() => new AddressAutocompleteClient(_httpClient, null, _credentials));
        }

        [Fact]
        public void throw_ArgumentNullException_if_Credentialsrpassed_null()
        {
            Assert.Throws<ArgumentNullException>(() => new AddressAutocompleteClient(_httpClient, _loggerMock.Object, null));
        }

        [Fact]
        public async Task Test_Find_GetsValidDataFromCanadaPostAPI_ExpectedEqualsActualResponse()
        {

            // Arrange
            var expected = new 
            {
                 Items = new List<AddressAutocompleteFindResponse>
                        {
                            new AddressAutocompleteFindResponse() { Id = "CA|CP|A|9728330",
                                                                    Text =  "875-4335 Boul Rigaud",
                                                                    Highlight = "0-3",
                                                                    Cursor = "0",
                                                                    Description = "Trois-Rivières, QC, G8Y 0C6",
                                                                    Next = "Retrieve"
                                                                  },
                            new AddressAutocompleteFindResponse() { Id = "CA|CP|A|7744611",
                                                                    Text =  "875-441 5 Ave SW",
                                                                    Highlight = "0-3",
                                                                    Cursor = "0",
                                                                    Description = "Calgary, AB, T2P 2V1",
                                                                    Next = "Retrieve"
                         }                                         },

             };
            var json = JsonConvert.SerializeObject(expected);

            string url = "https://ws1.postescanada-canadapost.ca/AddressComplete/Interactive/";

            HttpResponseMessage httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = System.Net.HttpStatusCode.OK;
            httpResponse.Content = new StringContent(json, Encoding.UTF8, "application/json");
            
            Mock<HttpMessageHandler> mockHandler = new Mock<HttpMessageHandler>();
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Get && r.RequestUri.ToString().StartsWith(url) &&
                !String.IsNullOrEmpty(HttpUtility.ParseQueryString(r.RequestUri.Query).Get("searchTerm")) 
                && HttpUtility.ParseQueryString(r.RequestUri.Query).Get("searchTerm") == "875"),
                ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            HttpClient httpClient = new HttpClient(mockHandler.Object)
            {
                BaseAddress = new Uri(url)
            };

            // Act
            AddressAutocompleteClient sut = new AddressAutocompleteClient(httpClient, _loggerMock.Object, _credentials);

            var actual = await sut.Find("875", null);


            //assert
            Assert.Equal(expected.Items.GetType(), actual.GetType());
            Assert.Equal(expected.Items.Count, actual.Count());
            Assert.Equal(expected.Items[0].Text, actual.ElementAt(0).Text);
            Assert.Equal(expected.Items[1].Text, actual.ElementAt(1).Text);

        }

        [Fact]
        public async Task Test_Find_Throws_AddressAutocompleteApiException_WhenResponseSuccessCodeisFalse()
        {

            // Arrange
            var expected = new
            {
                Items = new List<AddressAutocompleteFindResponse>
                        {
                            new AddressAutocompleteFindResponse() { Id = "CA|CP|A|9728330",
                                                                    Text =  "875-4335 Boul Rigaud",
                                                                    Highlight = "0-3",
                                                                    Cursor = "0",
                                                                    Description = "Trois-Rivières, QC, G8Y 0C6",
                                                                    Next = "Retrieve"
                                                                  },
                            new AddressAutocompleteFindResponse() { Id = "CA|CP|A|7744611",
                                                                    Text =  "875-441 5 Ave SW",
                                                                    Highlight = "0-3",
                                                                    Cursor = "0",
                                                                    Description = "Calgary, AB, T2P 2V1",
                                                                    Next = "Retrieve"
                         }                                         },

            };
            var json = JsonConvert.SerializeObject(expected);

            string url = "https://ws1.postescanada-canadapost.ca/AddressComplete/Interactive/";

            HttpResponseMessage httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = System.Net.HttpStatusCode.BadRequest;
            httpResponse.Content = new StringContent(json, Encoding.UTF8, "application/json");

            Mock<HttpMessageHandler> mockHandler = new Mock<HttpMessageHandler>();
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Get && r.RequestUri.ToString().StartsWith(url) &&
                !String.IsNullOrEmpty(HttpUtility.ParseQueryString(r.RequestUri.Query).Get("searchTerm"))
                && HttpUtility.ParseQueryString(r.RequestUri.Query).Get("searchTerm") == "875"),
                ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            HttpClient httpClient = new HttpClient(mockHandler.Object)
            {
                BaseAddress = new Uri(url)
            };

            // Act
            AddressAutocompleteClient sut = new AddressAutocompleteClient(httpClient, _loggerMock.Object, _credentials);

            //assert
            await Assert.ThrowsAsync<AddressAutocompleteApiException>(() =>  sut.Find("875", null));

        }

        [Fact]
        public async Task Test_Find_Throws_AddressAutocompleteApiException_WhenGetAsyncFailsDuetoWrongUrl()
        {

            // Arrange
            var expected = new
            {
                Items = new List<AddressAutocompleteFindResponse>
                        {
                            new AddressAutocompleteFindResponse() { Id = "CA|CP|A|9728330",
                                                                    Text =  "875-4335 Boul Rigaud",
                                                                    Highlight = "0-3",
                                                                    Cursor = "0",
                                                                    Description = "Trois-Rivières, QC, G8Y 0C6",
                                                                    Next = "Retrieve"
                                                                  },
                        }

            };
            var json = JsonConvert.SerializeObject(expected);

            // Expected URL for Canada Post API
            string url = "https://ws1.postescanada-canadapost.ca/AddressComplete/Interactive/";

            HttpResponseMessage httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = System.Net.HttpStatusCode.BadRequest;
            httpResponse.Content = new StringContent(json, Encoding.UTF8, "application/json");

            Mock<HttpMessageHandler> mockHandler = new Mock<HttpMessageHandler>();
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Get && r.RequestUri.ToString().StartsWith(url) &&
                !String.IsNullOrEmpty(HttpUtility.ParseQueryString(r.RequestUri.Query).Get("searchTerm"))
                && HttpUtility.ParseQueryString(r.RequestUri.Query).Get("searchTerm") == "875"),
                ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Fake uri provided to test if AddressAutocompleteApiException is thrown
            HttpClient httpClient = new HttpClient(mockHandler.Object)
            {
                BaseAddress = new Uri("https://wss1.fakepostescanada-canadaapost.ca/AddressComplete/Interactive/")
            };

            // Act
            AddressAutocompleteClient sut = new AddressAutocompleteClient(httpClient, _loggerMock.Object, _credentials);

            //assert
            await Assert.ThrowsAsync<AddressAutocompleteApiException>(() => sut.Find("875", null));

        }
    }
}
