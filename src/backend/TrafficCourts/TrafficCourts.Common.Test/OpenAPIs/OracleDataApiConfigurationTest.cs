using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using TrafficCourts.Common.Configuration.Validation;
using TrafficCourts.Common.OpenAPIs.OracleDataAPI;
using Xunit;

namespace TrafficCourts.Common.Test.OpenAPIs
{
    public class OracleDataApiConfigurationTest
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void OracleDataApiConfiguration_validates_BaseUrl_is_required(string uri)
        {
            OracleDataApiConfiguration sut = new()
            {
                BaseUrl = uri
            };

            var actual = Assert.Throws<SettingsValidationException>(() => sut.Validate());
       
            Assert.Equal(GetExceptionMessage("is required"), actual.Message);
        }

        [Theory]
        [InlineData("localhost")]
        [InlineData("http:")]
        [InlineData("http://")]
        public void OracleDataApiConfiguration_validates_BaseUrl_is_valid_URI(string uri)
        {
            OracleDataApiConfiguration sut = new()
            {
                BaseUrl = uri
            };

            var actual = Assert.Throws<SettingsValidationException>(() => sut.Validate());

            Assert.Equal(GetExceptionMessage("is not a valid uri"), actual.Message);
        }

        private string GetExceptionMessage(string message)
        {
            return $"Settings were invalid: {OracleDataApiConfiguration.Section}.{nameof(OracleDataApiConfiguration.BaseUrl)} {message}. Check that your configuration has been loaded correctly, and all necessary values are set in the configuration files.";
        }
    }
}
