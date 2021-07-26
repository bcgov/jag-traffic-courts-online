using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Gov.CitizenApi.Test.Utils
{
    [ExcludeFromCodeCoverage]
    public static class PropertyCopy
    {
        public static T CopyProperties<T>(T source)
            where T : class, new()
        {
            T target = new T();

            var properties = typeof(T).GetProperties();
            foreach (var property in properties.Where(_ => _.CanRead && _.CanWrite))
            {
                object value = property.GetValue(source);
                property.SetValue(target, value);
            }

            return target;
        }
    }

}
