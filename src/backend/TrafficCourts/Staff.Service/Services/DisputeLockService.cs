using TrafficCourts.Staff.Service.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TrafficCourts.Staff.Service.Services;

public class DisputeLockService : IDisputeLockService
{
    private static readonly Dictionary<string, Lock> _database;

    static DisputeLockService()
    {
        _database = new Dictionary<string, Lock>();
    }

    public Lock? GetLock(string ticketNumber, string username)
    {
        lock (_database)
        {
            if (_database.TryGetValue(ticketNumber, out Lock? value) && DateTime.UtcNow < value.ExpiryTimeUtc) {
                if (value.Username == username)
                {
                    return value;
                }
                else
                {
                    throw new LockIsInUseException(value.Username, value);
                }
            }

            Lock disputeLock = new()
            {
                LockId = Guid.NewGuid().ToString("n"),
                TicketNumber = ticketNumber,
                Username = username,
                ExpiryTimeUtc = DateTime.UtcNow.AddSeconds(5 * 60) // 5 minutes
            };

            _database[ticketNumber] = disputeLock;

            return disputeLock;
        }
    }

    public DateTimeOffset? RefreshLock(string lockId, string username)
    {
        lock (_database)
        {
            var lockToUpdate = _database.Values.FirstOrDefault(x => x.LockId == lockId);

            if (lockToUpdate is null) return null;

            if (lockToUpdate.Username != username) throw new LockIsInUseException(lockToUpdate.Username, lockToUpdate);

            lockToUpdate.ExpiryTimeUtc = DateTime.UtcNow.AddSeconds(5 * 60);

            return lockToUpdate.ExpiryTimeUtc;
        }
    }

    public void ReleaseLock(string lockId)
    {
        lock (_database)
        {
            var toRemove = _database.Where(x => x.Value.LockId == lockId).ToList();

            foreach (var item in toRemove)
            {
                _database.Remove(item.Key);
            }
        }
    }
}

[Serializable]
public class LockIsInUseException : Exception
{
    public LockIsInUseException(string username, Lock @lock) : base($"Failed to acquire the lock. The following user has the lock for the dispute: {username}")
    {
        Username = username;
        Lock = @lock;
    }

    public string Username { get; init; }

    public Lock Lock { get; init; }
}
