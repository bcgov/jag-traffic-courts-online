﻿using Microsoft.Extensions.FileProviders;
using System.Reflection;

namespace TrafficCourts.Citizen.Service.Services.Tickets.Search.Mock
{
    public class EmbeddedMockDataProvider : IMockDataProvider
    {
        private const string mockDataPath = "Services.Tickets.Search.Mock.test-ticket-data.csv";
        private readonly ILogger<EmbeddedMockDataProvider> _logger;

        public EmbeddedMockDataProvider(ILogger<EmbeddedMockDataProvider> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Stream? GetDataStream()
        {
            using var activity = Diagnostics.Source.StartActivity("get embedded data");

            var embeddedProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());
            var fileInfo = embeddedProvider.GetFileInfo(mockDataPath);

            if (!fileInfo.Exists)
            {
                _logger.LogInformation("Mock data not found, expected {Filename} to exist", mockDataPath);
                activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, $"File {mockDataPath} not found");
                return null;

            }
            var stream = embeddedProvider.GetFileInfo(mockDataPath).CreateReadStream();
            return stream;
        }
    }
}
