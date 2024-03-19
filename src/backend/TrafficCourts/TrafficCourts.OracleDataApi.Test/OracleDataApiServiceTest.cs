using AutoFixture;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Oracle = TrafficCourts.OracleDataApi.Client.V1;

namespace TrafficCourts.OracleDataApi.Test;

public class OracleDataApiServiceTest
{
    private readonly Fixture _fixture = new Fixture();

    private readonly Oracle.IOracleDataApiClient _client = Substitute.For<Oracle.IOracleDataApiClient>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly ILogger<OracleDataApiService> _logger = Substitute.For<ILogger<OracleDataApiService>>();

    public OracleDataApiServiceTest()
    {
        // Tests - these are the tests per method that should be implmented
        //
        // 1. input parameters are mapped from domain to oracle
        // 2. client method is called with correct parameters
        // 3. client response is mapped from oracle to domain
        // 4. if call a changed data, the correct event is published to _mediator
        // 5. returns the output of the mapper
        // 6. if ApiException is thrown, common domain ApiException is thrown with same parameters
        // 7. if any other exception is thrown, the exact same exception is re-thrown
    }

    [Fact]
    public void constructor_arguments_should_not_be_null()
    {
        void constructor_argument_should_not_be_null(string paramName, Oracle.IOracleDataApiClient client, IMapper mapper, IMediator mediator, ILogger<OracleDataApiService> logger)
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new OracleDataApiService(client, mapper, mediator, logger));
            Assert.Equal(paramName, actual.ParamName);
        }

        constructor_argument_should_not_be_null("client", null!, _mapper, _mediator, _logger);
        constructor_argument_should_not_be_null("mapper", _client, null!, _mediator, _logger);
        constructor_argument_should_not_be_null("mediator", _client, _mapper, null!, _logger);
        constructor_argument_should_not_be_null("logger", _client, _mapper, _mediator, null!);
    }

    [Fact]
    public async Task AcceptJJDisputeAsync()
    {
        // create oracle object that should be returned from the client
        var oracle = _fixture.Create<Oracle.JJDispute>();

        _client.AcceptJJDisputeAsync(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromResult(oracle));

        // setup mapping
        var expected = SetupMapping<Domain.Models.JJDispute, Oracle.JJDispute>(oracle);

        OracleDataApiService sut = new(_client, _mapper, _mediator, _logger);

        // act
        var actual = await sut.AcceptJJDisputeAsync("abc", true, "123", CancellationToken.None);

        // assert calls were made
        await _client.Received().AcceptJJDisputeAsync(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<string>(), Arg.Any<CancellationToken>());

        AssertMapped<Domain.Models.JJDispute, Oracle.JJDispute>(oracle);

        await AssertEventNotPublished();

        // assert service returns the object returned from the mapper
        Assert.Equal(expected, actual);
    }

    private async Task AssertEventNotPublished()
    {
        await _mediator.DidNotReceive().Publish(Arg.Any<object>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Ensures the mapping was called.
    /// </summary>
    /// <typeparam name="TDestination"></typeparam>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    private void AssertMapped<TDestination, TSource>(TSource source)
    {
        _mapper.Received().Map<TDestination>(source);
    }

    /// <summary>
    /// Configures the mapper to return an random object.
    /// When testing the OracleDataApiService, we dont
    /// need to test if the mapper is working. The mapper
    /// is tested separately.
    /// </summary>
    /// <typeparam name="TExpected">The type of the expected value.</typeparam>
    /// <typeparam name="TSource">The type of the source value.</typeparam>
    /// <param name="source">The source value.</param>
    /// <returns>A new random object as the result of calling Map.</returns>
    private TExpected SetupMapping<TExpected, TSource>(TSource source)
    {
        // create the expected value returned from the mapper
        TExpected expected = _fixture.Create<TExpected>();

        // when converting source to expected,
        _mapper.Map<TExpected>(source).Returns(expected);

        // return the exptected value
        return expected;
    }
}
