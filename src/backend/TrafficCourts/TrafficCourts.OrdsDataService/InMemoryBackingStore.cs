using System.Collections.Concurrent;
using System.Globalization;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TrafficCourts.OrdsDataService.Test")]

namespace TrafficCourts.OrdsDataService;

internal class InMemoryBackingStore : IBackingStore
{
    private ConcurrentDictionary<string, Field> _store = new ConcurrentDictionary<string, Field>();

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

    public void Set<T>(string key, T? value, bool alwaysDirty = false)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentNullException(nameof(key));
        }

        if (!_store.TryAdd(key, new Field { Value = value, Dirty = true, AlwaysDirty = alwaysDirty }))
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

    public Dictionary<string, object?> ToDictionary()
    {
        var values = new Dictionary<string, object?>();

        foreach (var (key, field) in _store)
        {
            if (field.Dirty)
            {
                values.Add(key, field.Value);
            }
        }

        return values;
    }

    public bool Dirty
    {
        get
        {
            foreach (var (_, field) in _store)
            {
                if (field.AlwaysDirty || field.Dirty)
                {
                    return true;
                }
            }
            return false;
        }
        set
        {
            foreach (var (_, field) in _store)
            {
                if (!field.AlwaysDirty)
                {
                    field.Dirty = value;
                }
            }
        }
    }
}

public class Field
{
    public object? Value { get; set; }
    public bool Dirty { get; set; }
    public bool AlwaysDirty { get; set; }
}