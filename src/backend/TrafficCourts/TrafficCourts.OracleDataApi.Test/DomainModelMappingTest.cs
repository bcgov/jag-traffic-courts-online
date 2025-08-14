using AutoFixture.Kernel;
using AutoFixture;
using AutoMapper;
using Xunit.Abstractions;
using TrafficCourts.OracleDataApi.Client.V1;

namespace TrafficCourts.OracleDataApi.Test;

public class DateOnlySpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type type && (type == typeof(DateOnly) || type == typeof(DateOnly?)))
        {
           
            // Generate a random date in a reasonable range (e.g., within the last 50 years)
            var randomDate = context.Create<DateTime>();
            return new DateOnly(randomDate.Year, randomDate.Month, randomDate.Day);
        }

        return new NoSpecimen();
    }
}

/// <summary>
/// Base class for testing domain model mapping. Creates the <see cref="IMapper"/> based
/// on the <see cref="OracleDomainModelMappingProfile"/> profile.
/// </summary>
public abstract class DomainModelMappingTest
{
    /// <summary>
    /// Fixture used to create instances
    /// </summary>
    protected readonly Fixture _fixture;
    /// <summary>
    /// The mapper under test.
    /// </summary>
    protected readonly IMapper _sut;
    /// <summary>
    /// Output helper if required.
    /// </summary>
    protected readonly ITestOutputHelper _output;

    protected DomainModelMappingTest(ITestOutputHelper output)
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<OracleDomainModelMappingProfile>());
        configuration.AssertConfigurationIsValid();

        _sut = configuration.CreateMapper();
        _output = output;

        _fixture = new Fixture();
        // EnumGenerator will round robin thru enum values, may need to generate types
        // multiple times to get non-default values
        _fixture.Customizations.Add(new EnumGenerator());
        _fixture.Customizations.Add(new DateOnlySpecimenBuilder());

        _fixture.Customizations.Add(new StreamGenerator());
        _fixture.Customizations.Add(new DisposableGenerator());
    }

    class StreamGenerator : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            var type = request as Type;
            if (type == null || type != typeof(System.IO.Stream))
                return new NoSpecimen();

            return new System.IO.MemoryStream();
        }
    }

    class DisposableGenerator : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            var type = request as Type;
            if (type == null || type != typeof(System.IDisposable))
                return new NoSpecimen();

            return null!;
        }
    }
}
