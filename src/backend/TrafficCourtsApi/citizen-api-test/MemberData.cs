using System.Collections.Generic;
using System.Linq;

namespace Gov.CitizenApi.Test
{
    /// <summary>
    /// Contains helper methods that can be used in MemberDataAttribute parameters
    /// </summary>
    public static class MemberData
    {
        /// <summary>
        /// Returns all the data types declared in the Citizen Api project that are assignable from {T}.
        /// </summary>
        /// <see cref="Type.IsAssignableFrom"/>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<object[]> GetTypesAssignableFrom<T>()
        {
            var types = typeof(Startup).Assembly
                .GetTypes()
                .Where(type => typeof(T).IsAssignableFrom(type));

            foreach (var type in types)
            {
                yield return new object[] { type };
            }
        }
    }
}
