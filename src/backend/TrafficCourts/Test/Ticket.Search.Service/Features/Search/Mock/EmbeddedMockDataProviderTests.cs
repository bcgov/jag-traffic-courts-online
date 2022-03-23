﻿using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using TrafficCourts.Ticket.Search.Service.Features.Search.Mock;
using Xunit;

namespace TrafficCourts.Test.Ticket.Search.Service.Features.Search.Mock
{
    public class EmbeddedMockDataProviderTests
    {
        [Fact]
        public void can_get_readable_stream()
        {
            Mock<ILogger<EmbeddedMockDataProvider>> mockLogger = new();

            EmbeddedMockDataProvider sut = new(mockLogger.Object);

            using var actual = sut.GetDataStream();
            Assert.NotNull(actual);
            Assert.True(actual!.CanRead);
        }

        [Fact]
        public void constructor_logger_parameter_is_required()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new EmbeddedMockDataProvider(null!));
            Assert.Equal("logger", actual.ParamName);
        }
    }

    public class MockTicketSearchServiceTests
    {
        [Fact]
        public async Task can_get_data_from_embedded_data_provider()
        {
            IMockDataProvider mockDataProvider = GetEmbeddedMockDataProvider();
            Mock<ILogger< MockTicketSearchService >> mockLogger = new();

            MockTicketSearchService sut = new MockTicketSearchService(mockDataProvider, mockLogger.Object);

            var actual = await sut.SearchTicketAsync("EZ02000460", "09:54", CancellationToken.None);

            Assert.NotNull(actual);
            Assert.NotEmpty(actual);
        }

        private IMockDataProvider GetEmbeddedMockDataProvider()
        {
            Mock<ILogger<EmbeddedMockDataProvider>> mockLogger = new();
            EmbeddedMockDataProvider provider = new(mockLogger.Object);
            return provider;
        }
    }
}
