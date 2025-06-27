using System;
using AutoMapper;
using Xunit;
using Oracle = TrafficCourts.OracleDataApi.Client.V1;
using DomainModel = TrafficCourts.Domain.Models;
using TrafficCourts.OracleDataApi.Client.V1;

namespace TrafficCourts.OracleDataApi.Test;

public class OracleDomainModelMappingProfileTests
{
    private readonly IMapper _mapper;

    public OracleDomainModelMappingProfileTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<OracleDomainModelMappingProfile>());
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void UtcToPacificTimeDateTime_ValidUtcDateTimeOffset_ReturnsPacificTime()
    {
        // Arrange
        DateTimeOffset utcDateTime = new DateTimeOffset(2023, 10, 1, 12, 0, 0, TimeSpan.Zero);

        // Act
        DateTime? result = OracleDomainModelMappingProfile.UtcToPacificTimeDateTime(utcDateTime);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(DateTimeKind.Unspecified, result.Value.Kind);
        Assert.Equal(new DateTime(2023, 10, 1, 5, 0, 0), result.Value); // Pacific Time (UTC-7)
    }

    [Fact]
    public void UtcToPacificTimeDateTime_Null_ReturnsNull()
    {
        // Act
        DateTime? result = OracleDomainModelMappingProfile.UtcToPacificTimeDateTime(null);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void UtcToPacificTimeDateTime_NonUtcDateTimeOffset_ReturnsNull()
    {
        // Arrange
        DateTimeOffset nonUtcDateTime = new DateTimeOffset(2023, 10, 1, 12, 0, 0, TimeSpan.FromHours(1));

        // Act
        DateTime? result = OracleDomainModelMappingProfile.UtcToPacificTimeDateTime(nonUtcDateTime);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void DisputeMapping_IssuedTs_MapsToPacificTime()
    {
        // Arrange
        var source = new Oracle.Dispute
        {
            IssuedTs = new DateTime(2023, 10, 1, 12, 0, 0, DateTimeKind.Unspecified) // UTC
        };

        // Act
        var destination = _mapper.Map<DomainModel.Dispute>(source);

        // Assert
        Assert.NotNull(destination);
        Assert.NotNull(destination.IssuedTs);
        Assert.Equal(new DateTime(2023, 10, 1, 5, 0, 0), destination.IssuedTs); // Pacific Time (UTC-7)
    }
}