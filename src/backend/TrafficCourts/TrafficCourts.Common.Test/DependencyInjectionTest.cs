using Microsoft.Extensions.DependencyInjection;

namespace TrafficCourts.Common.Test;

public abstract class DependencyInjectionTest
{
    protected static bool Exists<TService, TImplementation>(ServiceDescriptor descriptor)
    {
        return descriptor.ServiceType == typeof(TService) && descriptor.ImplementationType == typeof(TImplementation);
    }
}