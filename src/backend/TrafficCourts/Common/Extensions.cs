using Microsoft.Extensions.DependencyInjection;

namespace TrafficCourts.Common
{
    /// <summary>
    /// Common extensions methods
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Gets the base address of the <see cref="Uri"/>.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns>The base address of the <see cref="Uri"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="uri"/> is null</exception>
        public static Uri BaseAddress(this Uri uri)
        {
            if (uri is null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            return new Uri($"{uri.Scheme}://{uri.Host}:{uri.Port}");
        }

        /// <summary>
        /// Registers MemoryStream Manager that pools memory allocations to improve application performance, especially in the area of garbage collection.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddRecyclableMemoryStreams(this IServiceCollection services)
        {
            services.AddSingleton<IMemoryStreamManager, RecyclableMemoryStreamManager>();
            return services;
        }
    }
}
