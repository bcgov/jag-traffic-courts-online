using TrafficCourts.Staff.Service.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TrafficCourts.Staff.Service.Services;

public class DisputeLockService : IDisputeLockService
{
    private static readonly Dictionary<long, Lock> _database;

    public Lock? GetLock(long disputeId, string username)
    {
        lock (_database)
        {
            if (_database.TryGetValue(disputeId, out Lock? value)) throw new LockIsInUseException(value.Username);

            Lock disputeLock = new()
            {
                LockId = Guid.NewGuid().ToString("n"),
                DisputeId = disputeId,
                Username = username,
                ExpiryTimeUtc = DateTime.UtcNow.AddSeconds(5 * 60) // 5 minutes
            };

            _database[disputeId] = disputeLock;

            return disputeLock;
        }
    }

    public DateTimeOffset? RefreshLock(Guid lockId, string username)
    {
        throw new NotImplementedException();
    }

    public void ReleaseLock(Guid lockId)
    {
        throw new NotImplementedException();
    }
}

[Serializable]
public class LockIsInUseException : Exception
{
    public LockIsInUseException(string username) : base($"Failed to acquire the lock. The following user has the lock for the given dispute: {username}")
    {
        Username = username;
    }

    public string Username { get; init; }
}
