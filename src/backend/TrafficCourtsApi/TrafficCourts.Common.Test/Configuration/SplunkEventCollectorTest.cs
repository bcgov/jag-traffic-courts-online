using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Moq;
using NUnit.Framework;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.TestCorrelator;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Security;
using System.Runtime.InteropServices;
using TrafficCourts.Common.Configuration;

namespace TrafficCourts.Common.Test.Configuration
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class SplunkEventCollectorTest
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.TestCorrelator()
                .CreateLogger();
        }

        [Test]
        public void SplunkEventCollector_custom_server_certificate_validator_returns_true()
        {
            // there are no parameter validations
            Assert.True(SplunkEventCollector.ServerCertificateCustomValidation(null, null, null, SslPolicyErrors.None));
        }

        [Test]
        public void Configure_checks_for_null_arguments()
        {
            var hostBuilderContext = new HostBuilderContext(new Dictionary<object, object>());
            var loggerConfiguration = new LoggerConfiguration();

            Assert.Throws<ArgumentNullException>(() => SplunkEventCollector.Configure(null, loggerConfiguration));
            Assert.Throws<ArgumentNullException>(() => SplunkEventCollector.Configure(hostBuilderContext, null));
        }

        [Test]
        public void Configure_logs_warning_if_splunk_configuration_settings_not_found()
        {
            var configuration = new ConfigurationBuilder()
                .Build();

            var hostBuilderContext = new HostBuilderContext(new Dictionary<object, object>());
            hostBuilderContext.Configuration = configuration;

            var loggerConfiguration = new LoggerConfiguration();

            using (TestCorrelator.CreateContext())
            {
                // act
                SplunkEventCollector.Configure(hostBuilderContext, loggerConfiguration);

                var logEvent = TestCorrelator.GetLogEventsFromCurrentContext().Single();

                Assert.AreEqual(LogEventLevel.Warning, logEvent.Level);

                Assert.NotNull(logEvent.RenderMessage()); // assert anything about this message?
            }
        }

        [Test]
        [Ignore("Having troubles with Serilog reading the configuration and also our reading of the configuration")]
        public void Configure_will_configure_EventCollector_if_url_and_token_are_available()
        {
            var configurationValues = new Mock<IDictionary<string, string>>();

            // without this, we get a NullReferenceException when Serilog tries to read the configuration
            configurationValues.Setup(_ => _.GetEnumerator()).Returns(new List<KeyValuePair<string, string>>().GetEnumerator());

            // setup that the expected configuration keys
            configurationValues.Setup(_ => _["Splunk:Url"]).Returns("http://localhost");
            configurationValues.Setup(_ => _["Splunk:Token"]).Returns("Unit Test");


            var configuration = new ConfigurationBuilder()
                .AddUnitTestValues(configurationValues.Object)
                .Build();

            var hostBuilderContext = new HostBuilderContext(new Dictionary<object, object>());
            hostBuilderContext.Configuration = configuration;

            var loggerConfiguration = new LoggerConfiguration();

            using (TestCorrelator.CreateContext())
            {
                // act
                SplunkEventCollector.Configure(hostBuilderContext, loggerConfiguration);

                var logEvents = TestCorrelator.GetLogEventsFromCurrentContext().ToList();
                Assert.AreEqual(0, logEvents.Count); // this will fail due to the GetEnumerator returning empty list

            }

            configurationValues.VerifyGet(_ => _["Splunk:Url"], Times.Once);
            configurationValues.VerifyGet(_ => _["Splunk:Token"], Times.Once);
        }
    }
}
