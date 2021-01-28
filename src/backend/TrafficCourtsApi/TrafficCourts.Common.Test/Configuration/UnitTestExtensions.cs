using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace TrafficCourts.Common.Test.Configuration
{
    public static class UnitTestExtensions
    {
        /// <summary>
        /// Adds an <see cref="IConfigurationProvider"/> that reads configuration values the provided dictionary.
        /// </summary>
        /// <param name="configurationBuilder">The configuration builder.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static IConfigurationBuilder AddUnitTestValues(this IConfigurationBuilder configurationBuilder, IDictionary<string,string> values = null)
        {
            if (configurationBuilder == null) throw new ArgumentNullException(nameof(configurationBuilder));

            configurationBuilder.Add(new UnitTestConfigurationSource(values));
            return configurationBuilder;
        }

        private class UnitTestConfigurationSource : IConfigurationSource
        {
            private readonly IDictionary<string, string> _values;

            public UnitTestConfigurationSource(IDictionary<string, string> values)
            {
                _values = values ?? new Dictionary<string, string>();
            }

            public IConfigurationProvider Build(IConfigurationBuilder builder)
            {
                return new UnitTestConfigurationProvider(_values);
            }
        }

        private class UnitTestConfigurationProvider : ConfigurationProvider
        {
            private readonly IDictionary<string, string> _values;

            public UnitTestConfigurationProvider(IDictionary<string, string> values)
            {
                _values = values;
            }

            public override void Load()
            {
                Data = _values;
            }
        }
    }
}
