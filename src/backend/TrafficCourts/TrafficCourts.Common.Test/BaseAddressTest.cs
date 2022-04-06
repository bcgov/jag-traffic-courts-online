using System;
using Xunit;

namespace TrafficCourts.Common.Test;

public class BaseAddressTest
{
    [Fact]
    public void BaseAddress_should_throw_ArgumentNullException_if_passed_null()
    {
        var actual = Assert.Throws<ArgumentNullException>(() => Extensions.BaseAddress(null!));
    }

    [Fact]
    public void BaseAddress_should_extract_base_address()
    {
        Uri expected = new Uri("http://localhost:12345");
        Uri uri = new Uri(expected, "/a/b?q=1");

        Uri actual = Extensions.BaseAddress(uri);
        Assert.Equal(expected, actual);
    }
}
