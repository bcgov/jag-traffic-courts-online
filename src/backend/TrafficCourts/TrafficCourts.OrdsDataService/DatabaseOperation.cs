namespace TrafficCourts.OrdsDataService;

/// <summary>
/// Represents a single operation to execute in the database.
/// </summary>
public class DatabaseOperation : Dictionary<string, object?>
{
    internal DatabaseOperation(DatabaseOperationType operationType, string table, DatabaseEntity entity)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(table);
        ArgumentNullException.ThrowIfNull(entity);

        Dictionary<string, object?> data = entity.BackingStore.ToDictionary(operationType);

        if (data.Keys.Count == 0)
        {
            throw new ArgumentException("There are no properties set on the entity.", nameof(entity));
        }

        Add("$operation", GetOperationName(operationType));
        Add("$table", table);
        Add("$data", data);
    }

    private static string GetOperationName(DatabaseOperationType operationType)
    {
        return operationType switch
        {
            DatabaseOperationType.Insert => "insert",
            DatabaseOperationType.Update => "update",
            DatabaseOperationType.Delete => "delete",
            _ => throw new ArgumentOutOfRangeException(nameof(operationType), operationType, null)
        };
    }
}
