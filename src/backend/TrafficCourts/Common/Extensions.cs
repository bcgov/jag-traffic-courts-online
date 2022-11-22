using HashidsNet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using TrafficCourts.Common.Configuration;

namespace TrafficCourts.Common
{
    /// <summary>
    /// Common extensions methods
    /// </summary>
    public static partial class Extensions
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

        /// <summary>
        /// Adds <see cref="IHashids"/> service.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="coniguration"></param>
        /// <returns></returns>
        public static IServiceCollection AddHashids(this IServiceCollection services, IConfiguration coniguration)
        {
            services.ConfigureValidatableSetting<HashidsOptions>(coniguration.GetSection(HashidsOptions.Section));

            services.AddSingleton<IHashids>(services =>
            {
                var configuration = services.GetRequiredService<HashidsOptions>();
                return new Hashids(configuration.Salt);
            });

            return services;
        }

        /// <summary>
        /// Returns the PropertyInfo of the specified property. Only tested on regular properties. 
        /// May not work on fields or other expressions.
        /// </summary>
        public static PropertyInfo GetPropertyInfo<TSource, TProperty>(this Expression<Func<TSource, TProperty>> property)
        {
            // from: https://khalidabuhakmeh.com/get-a-property-name-from-a-dotnet-lambda-expression
            LambdaExpression lambda = property;
            MemberExpression memberExpression = lambda.Body is UnaryExpression expression
                ? (MemberExpression)expression.Operand
                : (MemberExpression)lambda.Body;

            return (PropertyInfo)memberExpression.Member;
        }

        public static IDisposable? BeginScope<T>(this ILogger logger, T data, Expression<Func<T, object>> property)
            where T : class
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(data);
            ArgumentNullException.ThrowIfNull(property);

            var state = new Dictionary<string, object>();

            AddProperty(data, property, state);

            return logger.BeginScope(state);
        }

        private static void AddProperty<T>(T message, Expression<Func<T, object>> property, Dictionary<string, object> state)
            where T : class
        {
            Debug.Assert(message is not null);
            Debug.Assert(property is not null);
            Debug.Assert(state is not null);

            PropertyInfo propertyInfo = property.GetPropertyInfo();
            object? value = propertyInfo.GetValue(message);
            AddNotNullProperty(propertyInfo.Name, value, state);
        }

        /// <summary>
        /// Adds property if the value is not null.
        /// </summary>
        private static void AddNotNullProperty(string name, object? value, Dictionary<string, object> state)
        {
            Debug.Assert(name is not null);
            Debug.Assert(state is not null);

            if (value is not null)
            {
                state[name] = value;
            }
        }
    }
}
