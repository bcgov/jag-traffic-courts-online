using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Testcontainers.Redis;
using TrafficCourts.Staff.Service.Services;
using Xunit;

namespace TrafficCourts.Staff.Service.Test.Services;

public sealed class TestcontainerFactAttribute : FactAttribute
{
    public TestcontainerFactAttribute()
    {
#if !DEBUG
        Skip = "Unit tests using Testcontainers only run in debug builds";
#endif
    }
}

    public class RedisDisputeLockServiceTest
{
    private const string Alice = "Alice";
    private const string Bob = "Bob";

    private readonly string TicketNumber = Guid.NewGuid().ToString("n");

    private readonly RedisContainer _redis = new RedisBuilder()
        .WithImage("redis:7.2")
        .Build();

    private readonly ILogger<RedisDisputeLockService> _logger = NSubstitute.Substitute.For<ILogger<RedisDisputeLockService>>();

    private ConnectionMultiplexer? _connection;

    private async Task<RedisDisputeLockService> GetSubjectUnderTestAsync()
    {
        await _redis.StartAsync();

        _connection = await ConnectionMultiplexer.ConnectAsync(_redis.GetConnectionString());

        RedisDisputeLockService sut = new RedisDisputeLockService(_connection, TimeProvider.System, _logger);

        return sut;
    }

    [TestcontainerFact]
    public async Task getting_a_lock_returns_the_equivalent_lock_object_when_we_hold_lock()
    {
        RedisDisputeLockService sut = await GetSubjectUnderTestAsync();

        var expected = sut.GetLock(TicketNumber, Alice);

        var actual = sut.GetLock(TicketNumber, Alice);

        Assert.Equivalent(expected, actual);

        Assert.Equal(2, GetKeyCount()); // should be two keys
    }

    [TestcontainerFact]
    public async Task getting_a_lock_sets_properties_on_the_lock()
    {
        RedisDisputeLockService sut = await GetSubjectUnderTestAsync();

        var actual = sut.GetLock(TicketNumber, Alice);
        Assert.NotNull(actual);

        Assert.Equal(Alice, actual.Username);
        Assert.Equal(TicketNumber, actual.TicketNumber);
        Assert.True(actual.CreatedAtUtc < actual.ExpiryTimeUtc);
        Assert.False(actual.IsExpired);

        Assert.Equal(2, GetKeyCount()); // should be two keys
    }

    [TestcontainerFact]
    public async Task Getting_the_lock_throws_LockIsInUseException_when_someone_else_holds_lock()
    {
        RedisDisputeLockService sut = await GetSubjectUnderTestAsync();

        var expected = sut.GetLock(TicketNumber, Alice);

        Assert.NotNull(expected);

        var actual = Assert.Throws<LockIsInUseException>(() => sut.GetLock(TicketNumber, Bob));

        // exception should say who holds lock
        Assert.Equal(expected.Username, actual.Username);
        Assert.Equivalent(expected, actual.Lock);

        Assert.Equal(2, GetKeyCount()); // should be two keys
    }

    [TestcontainerFact]
    public async Task release_removes_the_keys()
    {
        RedisDisputeLockService sut = await GetSubjectUnderTestAsync();

        var @lock = sut.GetLock(TicketNumber, Alice);

        Assert.NotNull(@lock);
        Assert.Equal(Alice, @lock.Username);
        Assert.Equal(2, GetKeyCount()); // should be two keys

        sut.ReleaseLock(@lock.LockId);

        // after release, there should be no keys
        Assert.Equal(0, GetKeyCount());
    }

    [TestcontainerFact]
    public async Task refresh_advances_the_lock_expiry()
    {
        RedisDisputeLockService sut = await GetSubjectUnderTestAsync();

        var orginalLock = sut.GetLock(TicketNumber, Alice);

        Assert.NotNull(orginalLock);
        Assert.Equal(Alice, orginalLock.Username);

        var when = sut.RefreshLock(orginalLock.LockId, orginalLock.Username);

        Assert.NotNull(when);
        Assert.True(orginalLock.ExpiryTimeUtc < when);

        var refreshedLock = sut.GetLock(TicketNumber, Alice);
        Assert.NotNull(refreshedLock);

        Assert.Equal(orginalLock.CreatedAtUtc, refreshedLock.CreatedAtUtc);
        Assert.Equal(orginalLock.LockId, refreshedLock.LockId);
        Assert.Equal(orginalLock.TicketNumber, refreshedLock.TicketNumber);
        Assert.Equal(orginalLock.Username, refreshedLock.Username);
        Assert.Equal(when, refreshedLock.ExpiryTimeUtc);
    }

    private int GetKeyCount()
    {
        var server = _connection!.GetServer(_redis.GetConnectionString());
        return server.Keys(0).Count();
    }
}
