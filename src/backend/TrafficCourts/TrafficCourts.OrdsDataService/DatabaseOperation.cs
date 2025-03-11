namespace TrafficCourts.OrdsDataService;

/// <summary>
/// Represents a single operation to execute in the database.
/// </summary>
public class DatabaseOperation : Dictionary<string, object?>
{
    internal DatabaseOperation(string operation, string table, Dictionary<string, object?> data)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(operation);
        ArgumentException.ThrowIfNullOrWhiteSpace(table);
        ArgumentNullException.ThrowIfNull(data);

        if (data.Keys.Count == 0)
        {
            throw new ArgumentException("There are no properties set on the data.", nameof(data));
        }

        Add("$operation", operation);
        Add("$table", table);
        Add("$data", data);
    }

    internal DatabaseOperation(string operation, string table, DatabaseEntity entity)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(operation);
        ArgumentException.ThrowIfNullOrWhiteSpace(table);
        ArgumentNullException.ThrowIfNull(entity);

        Dictionary<string, object?> data = entity.BackingStore.ToDictionary();
        if (data.Keys.Count == 0)
        {
            throw new ArgumentException("There are no properties set on the entity.", nameof(entity));
        }

        Add("$operation", operation);
        Add("$table", table);
        Add("$data", data);
    }
}
