using MassTransit;
using StackExchange.Redis;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using TrafficCourts.Logging;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Staff.Service.Models;

namespace TrafficCourts.Staff.Service.Services;

public partial class RedisDisputeLockService : IDisputeLockService
{
    private readonly IConnectionMultiplexer _connection;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<RedisDisputeLockService> _logger;

    private static readonly string UnlockScript = EmbeddedResourceLoader.GetEmbeddedResource("TrafficCourts.Staff.Service.Services.RedisLua.Unlock.lua");
    // Returns 1 on success, 0 on failure setting expiry or key not existing, -1 if the key value didn't match
    private static readonly string ExtendIfMatchingValueScript = EmbeddedResourceLoader.GetEmbeddedResource("TrafficCourts.Staff.Service.Services.RedisLua.Extend.lua");

    public RedisDisputeLockService(
        IConnectionMultiplexer connection, 
        TimeProvider timeProvider,
        ILogger<RedisDisputeLockService> logger)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /*
     * Redis Key and Values
     * 
     *                   
     *                   Lock Id                              Lock Value
     *   +-------------------------------------+       +---------------------------+
     *   | staff:v0:lock-ticket-<TicketNumber> |       | staff:v0:lock-id-<LockId> |
     *   |-------------------------------------| <---> |---------------------------|
     *   |                              LockId |       | LockId                    |
     *   +-------------------------------------+       | TicketNumber              |
     *                                                 | Username                  |
     *                                                 | ExpiryTimeUtc             |
     *                                                 | CreatedAtUtc              |
     *                                                 +---------------------------+  
     * 
     */

    /// <summary>
    /// Get the redis key for the "Lock Id" entry for a given ticket number.
    /// </summary>
    private string LockIdRedisKey(string ticketNumber) => $"staff:v0:lock-ticket-{ticketNumber}"; // holds the lock id

    /// <summary>
    /// Get the redis key for the "Lock Value" entry for a given lock id.
    /// </summary>
    private string LockValueRedisKey(string lockId) => $"staff:v0:lock-id-{lockId}"; // holds the Lock object

    /// <summary>
    /// The lock duration. 5 minutes.
    /// </summary>
    private readonly TimeSpan LockDuration = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Gets the redis database.
    /// </summary>
    private IDatabase Database => _connection.GetDatabase(0);

    public Lock? GetLock(string ticketNumber, string username)
    {
        Lock? ticketLock = GetLock(ticketNumber);

        if (ticketLock is null)
        {
            // lock does not exist
            return AquireLock(ticketNumber, username);
        }
        else
        {
            // lock exists already in redis
            if (ticketLock.IsExpired)
            {
                // expired steal the lock
                return AquireLock(ticketNumber, username);
            }

            // not expired yet
            if (ticketLock.Username == username)
            {
                return ticketLock;
            }

            throw new LockIsInUseException(ticketLock.Username, ticketLock);
        }
    }

    public DateTimeOffset? RefreshLock(string lockId, string username)
    {
        Lock? @lock = GetLockById(lockId);

        if (@lock is null) return null;

        if (@lock.Username != username) throw new LockIsInUseException(@lock.Username, @lock);

        string lockIdKey = LockIdRedisKey(@lock.TicketNumber);

        Debug.Assert(lockId == @lock.LockId);

        // update the lock expiry
        @lock.ExpiryTimeUtc = _timeProvider.GetUtcNow().Add(LockDuration);

        // Returns 1 on success, 0 on failure setting expiry or key not existing, -1 if the key value didn't match
        long result = (long)Database.ScriptEvaluate(ExtendIfMatchingValueScript, [lockIdKey], [lockId, (long)GetExpiry(@lock).TotalMilliseconds], CommandFlags.DemandMaster);

        if (result == 1)
        {
            // save the lock value because the lock was extended
            string lockValueKey = LockValueRedisKey(lockId);
            Database.HashSet(lockValueKey, @lock.ToHashEntries(), CommandFlags.DemandMaster);
            // recompute the expiry because due to gc pause or cpu limit on kubernetes, there could be
            // delays from the call above
            Database.KeyExpire(lockValueKey, GetExpiry(@lock), CommandFlags.DemandMaster);

            return @lock.ExpiryTimeUtc;
        }
        else if (result == 0)
        {
            // failure setting expiry or key not existing
            LogExtendKeyNotFoundOrError(lockIdKey, lockId);
        }
        else if (result == -1)
        {
            // key value didn't match
            LogExtendKeyValueDidNotMatch(lockIdKey, lockId);
        }

        return null;
    }


    public void ReleaseLock(string lockId)
    {
        Lock? lockToRelease = GetLockById(lockId);
        if (lockToRelease is null) return;

        string lockIdKey = LockIdRedisKey(lockToRelease.TicketNumber);

        // delete the lock if the lock id matches
        RedisResult result = Database.ScriptEvaluate(UnlockScript, [lockIdKey], [lockToRelease.LockId], CommandFlags.DemandMaster);

        // remove the value regardless
        string lockValueKey = LockValueRedisKey(lockToRelease.LockId);
        Database.KeyDelete(lockValueKey);
    }


    private Lock CreateLock(string ticketNumber, string username) => new()
    {
        LockId = Guid.NewGuid().ToString("n"),
        TicketNumber = ticketNumber,
        Username = username,
        ExpiryTimeUtc = _timeProvider.GetUtcNow().Add(LockDuration)
    };

    /// <summary>
    /// Attempts to aquire the lock. Tries to set the lock and if that fails, 
    /// to 
    /// </summary>
    /// <param name="ticketNumber"></param>
    /// <param name="username"></param>
    /// <returns></returns>
    /// <exception cref="LockIsInUseException"></exception>
    private Lock AquireLock(string ticketNumber, string username)
    {
        Lock? ticketLock = CreateLock(ticketNumber, username);
        if (SetLock(ticketLock))
        {
            return ticketLock; // got the lock
        }

        // someone else got the lock already?
        ticketLock = GetLock(ticketNumber);
        if (ticketLock is not null)
        {
            // do we hold the lock?
            if (!ticketLock.IsExpired && ticketLock.Username == username)
            {
                return ticketLock;
            }

            // expired?
            // held by someone else

            throw new LockIsInUseException(ticketLock.Username, ticketLock);
        }

        // problem, why couldn't we get the lock

        throw new LockIsInUseException(ticketLock.Username, ticketLock);
    }

    /// <summary>
    /// Gets the lock for the ticket number.
    /// </summary>
    /// <param name="ticketNumber"></param>
    /// <returns></returns>
    private Lock? GetLock(string ticketNumber)
    {
        // get the lock-id for the ticket number
        string lockIdKey = LockIdRedisKey(ticketNumber);

        RedisValue lockId = Database.StringGet(lockIdKey);
        if (lockId.IsNull)
        {
            return null;
        }

        // now grab the lock object
        Lock? ticketLock = GetLockById(lockId!);
        return ticketLock;
    }

    private Lock? GetLockById(string lockId)
    {
        string lockValueKey = LockValueRedisKey(lockId);
        HashEntry[] entries = Database.HashGetAll(lockValueKey);
        if (entries.Length == 0)
        {
            return null;
        }

        Lock? ticketLock = entries.FromHashEntries();
        return ticketLock;
    }

    private bool SetLock(Lock ticketLock)
    {
        // save the lock id in redis
        var lockIdKey = LockIdRedisKey(ticketLock.TicketNumber);
        var redisResult = Database.StringSet(lockIdKey, ticketLock.LockId, GetExpiry(ticketLock), When.NotExists, CommandFlags.DemandMaster);

        if (!redisResult)
        {
            return false; // couldnt create the lock id key
        }

        // save the lock object in redis
        string lockValueKey = LockValueRedisKey(ticketLock.LockId);

        Database.HashSet(lockValueKey, ticketLock.ToHashEntries(), CommandFlags.DemandMaster);
        // recompute the expiry because due to gc pause or cpu limit on kubernetes, there could be
        // delays from the call above
        bool expireySet = Database.KeyExpire(lockValueKey, GetExpiry(ticketLock), CommandFlags.DemandMaster);

        return expireySet;
    }

    /// <summary>
    /// Computes when the lock will expire.
    /// </summary>
    private TimeSpan GetExpiry(Lock @lock) => @lock.ExpiryTimeUtc - _timeProvider.GetUtcNow();

    [LoggerMessage(EventId = 0, Level = LogLevel.Information, Message = "Value stpred in Redis {Key} did not having expected {Value}.", EventName = "RefreshErrorLockValueNotMatch")]
    private partial void LogExtendKeyValueDidNotMatch(string key, string value);

    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Could not update {Key} with {Value}. They key may not exist or there was an error.", EventName = "RefreshErrorKeyNotFound")]
    private partial void LogExtendKeyNotFoundOrError(string key, string value);


    internal static class EmbeddedResourceLoader
    {
        internal static string GetEmbeddedResource(string name)
        {
            var assembly = typeof(EmbeddedResourceLoader).GetTypeInfo().Assembly;

            using (var stream = assembly.GetManifestResourceStream(name))
            using (var streamReader = new StreamReader(stream))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
}

internal static class Extensions
{
    public static HashEntry[] ToHashEntries(this Lock theLock)
    {
        var entries = new HashEntry[]
            {
                new(nameof(Lock.CreatedAtUtc), theLock.CreatedAtUtc.ToString("o")),
                new(nameof(Lock.ExpiryTimeUtc), theLock.ExpiryTimeUtc.ToString("o")),
                new(nameof(Lock.LockId), theLock.LockId),
                new(nameof(Lock.TicketNumber), theLock.TicketNumber),
                new(nameof(Lock.Username), theLock.Username)
            };

        return entries;
    }

    public static Lock FromHashEntries(this HashEntry[] entries)
    {
        Lock @lock = new Lock();
        
        foreach (var entry in entries)
        {
            if (entry.Value.IsNull)
            {
                continue;
            }

            switch (entry.Name)
            {
                case nameof(Lock.CreatedAtUtc): @lock.CreatedAtUtc = DateTimeOffset.Parse(entry.Value!); break;
                case nameof(Lock.ExpiryTimeUtc): @lock.ExpiryTimeUtc = DateTimeOffset.Parse(entry.Value!); break;
                case nameof(Lock.LockId): @lock.LockId = entry.Value!; break;
                case nameof(Lock.TicketNumber): @lock.TicketNumber = entry.Value!; break;
                case nameof(Lock.Username): @lock.Username = entry.Value!; break;
            }
        }

        return @lock;
    }
}
