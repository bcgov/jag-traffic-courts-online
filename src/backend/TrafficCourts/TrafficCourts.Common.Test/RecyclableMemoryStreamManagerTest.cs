using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TrafficCourts.Common.Test;

public class RecyclableMemoryStreamManagerTest : DependencyInjectionTest
{
    [Fact]
    public void AddRecyclableMemoryStreams_should_register_RecyclableMemoryStreamManager_as_Singleton()
    {
        ServiceCollection services = new ServiceCollection();

        services.AddRecyclableMemoryStreams();
        var actual = services.SingleOrDefault(Exists<IMemoryStreamManager, RecyclableMemoryStreamManager>);

        Assert.NotNull(actual);
        Assert.Equal(ServiceLifetime.Singleton, actual!.Lifetime);
    }

    [Fact]
    public void RecyclableMemoryStreamManager_can_be_constructed()
    {
        var sut = new RecyclableMemoryStreamManager();
    }

    [Fact]
    public void RecyclableMemoryStreamManager_can_create_a_MemoryStream()
    {
        var sut = new RecyclableMemoryStreamManager();

        using var actual = sut.GetStream();
        Assert.NotNull(actual);

    }
}
