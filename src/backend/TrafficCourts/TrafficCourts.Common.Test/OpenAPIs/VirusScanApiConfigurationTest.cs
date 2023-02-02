using TrafficCourts.Common.Configuration.Validation;
using TrafficCourts.Common.OpenAPIs.VirusScan;
using Xunit;

namespace TrafficCourts.Common.Test.OpenAPIs;

public class VirusScanApiConfigurationTest
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void VirusScanApiConfiguration_validates_BaseUrl_is_required(string uri)
    {
        VirusScanApiConfiguration sut = new()
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
    public void VirusScanApiConfiguration_validates_BaseUrl_is_valid_URI(string uri)
    {
        VirusScanApiConfiguration sut = new()
        {
            BaseUrl = uri
        };

        var actual = Assert.Throws<SettingsValidationException>(() => sut.Validate());

        Assert.Equal(GetExceptionMessage("is not a valid uri"), actual.Message);
    }

    private string GetExceptionMessage(string message)
    {
        return $"Settings were invalid: {VirusScanApiConfiguration.Section}.{nameof(VirusScanApiConfiguration.BaseUrl)} {message}. Check that your configuration has been loaded correctly, and all necessary values are set in the configuration files.";
    }
}
