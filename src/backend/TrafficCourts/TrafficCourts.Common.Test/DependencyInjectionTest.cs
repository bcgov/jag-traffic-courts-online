using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Xunit;

namespace TrafficCourts.Common.Test;

public abstract class DependencyInjectionTest
{
    protected static bool Exists<TService, TImplementation>(ServiceDescriptor descriptor)
    {
        return descriptor.ServiceType == typeof(TService) && descriptor.ImplementationType == typeof(TImplementation);
    }
}