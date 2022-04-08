using TrafficCourts.Staff.Service.Logging;
using Xunit;

namespace TrafficCourts.Staff.Service.Test.Logging;

public class DiagnosticsTest
{
    [Fact]
    public void Diagnostics_Source_is_not_null()
    {
        Assert.NotNull(Diagnostics.Source);
        Assert.NotNull(Diagnostics.Source.Name);
        Assert.Equal("staff-api", Diagnostics.Source.Name);
    }
}
