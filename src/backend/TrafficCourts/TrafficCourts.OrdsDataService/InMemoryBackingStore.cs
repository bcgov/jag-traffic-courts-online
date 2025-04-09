using System.Collections.Concurrent;
using System.Globalization;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TrafficCourts.OrdsDataService.Test")]

namespace TrafficCourts.OrdsDataService;

internal class InMemoryBackingStore : IBackingStore
{
    private readonly ConcurrentDictionary<string, Field> _store = new ConcurrentDictionary<string, Field>();

    public T? Get<T>(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentNullException(nameof(key));
        }

        if (_store.TryGetValue(key, out var value))
        {
            return (T?)value.Value;
        }

        return default;
    }

    public DateTime? GetDateTime(string key, string format, DateTimeKind kind = DateTimeKind.Unspecified)
    {
        string? value = Get<string>(key);
        if (value is null)
        {
            return null;
        }

        var date = DateTime.ParseExact(value, format, CultureInfo.InvariantCulture);
        return DateTime.SpecifyKind(date, kind);
    }

    public void Set<T>(string key, T? value, bool isKey = false)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentNullException(nameof(key));
        }

        if (!_store.TryAdd(key, new Field { Value = value, Dirty = true, IsKey = isKey }))
        {
            // key already exists, update the value
            Field field = _store[key];

            if (!Equals(field.Value, value))
            {
                field.Value = value;
                field.Dirty = true;
            }
        }
    }

    public void SetDateTime(string key, DateTime? value, string format)
    {
        if (value is null)
        {
            Set<string>(key, null);
        }
        else
        {
            Set(key, value.Value.ToString(format, CultureInfo.InvariantCulture));
        }
    }

    /// <summary>
    /// Create the dictionary to send to the database base on the operation type.
    /// </summary>
    /// <param name="operationType"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public Dictionary<string, object?> ToDictionary(DatabaseOperationType operationType)
    {
        Func<Field, bool> predicate = operationType switch
        {
            DatabaseOperationType.Insert => Insert,
            DatabaseOperationType.Update => Update,
            DatabaseOperationType.Delete => Delete,
            _ => throw new ArgumentOutOfRangeException(nameof(operationType), operationType, null)
        };

        Dictionary<string, object?> values = _store
            .Where(kv => predicate(kv.Value))
            .ToDictionary(kv => kv.Key, kv => kv.Value.Value);

        return values;
    }

    public bool Dirty
    {
        get
        {
            return _store.Any(kv => kv.Value.Dirty);
        }
        set
        {
            foreach (var (_, field) in _store)
            {
                field.Dirty = value;
            }
        }
    }


    private static bool Insert(Field field)
    {
        return !field.IsKey && field.Dirty; // we need to send all fields except the key fields
    }

    private static bool Update(Field field)
    {
        return field.IsKey || field.Dirty; // we need to send the key fields and any dirty fields
    }

    private static bool Delete(Field field)
    {
        return field.IsKey; // we only need the key fields for delete
    }
}

public class Field
{
    public object? Value { get; set; }
    /// <summary>
    /// The value has changed.
    /// </summary>
    public bool Dirty { get; set; }
    /// <summary>
    /// The value represents a key.
    /// </summary>
    public bool IsKey { get; set; }
}
