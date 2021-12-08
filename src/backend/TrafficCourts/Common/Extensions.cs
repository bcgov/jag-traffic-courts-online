using Microsoft.Extensions.Logging;

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

    }
}
