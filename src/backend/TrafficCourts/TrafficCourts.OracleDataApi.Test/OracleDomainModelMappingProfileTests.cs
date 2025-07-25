using AutoFixture;
using AutoMapper;
using TrafficCourts.OracleDataApi.Client.V1;
using DomainModel = TrafficCourts.Domain.Models;

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
    public void DisputeMapping_IssuedTs_MapsEqual()
    {
        Fixture fixture = new Fixture();

        var expected = DateTime.SpecifyKind(fixture.Create<DateTime>(), DateTimeKind.Unspecified);

        // Arrange
        var source = new Dispute
        {
            IssuedTs = expected
        };

        // Act
        var actual = _mapper.Map<DomainModel.Dispute>(source);

        // Assert
        Assert.NotNull(actual);
        Assert.NotNull(actual.IssuedTs);
        Assert.Equal(expected, actual.IssuedTs);
    }
}