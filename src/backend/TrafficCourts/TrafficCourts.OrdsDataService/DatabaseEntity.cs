namespace TrafficCourts.OrdsDataService;

public abstract class DatabaseEntity
{
    /// <summary>
    /// Creates a new instance of the entity
    /// </summary>
    public DatabaseEntity()
    {
        BackingStore = new InMemoryBackingStore();
    }

    /// <summary>
    /// The name of the table in the database
    /// </summary>
    [System.Text.Json.Serialization.JsonIgnore]
    public abstract string Name { get; }

    /// <summary>
    /// The backing store for this entity. This is where the data is stored.
    /// </summary>
    [System.Text.Json.Serialization.JsonIgnore]
    public IBackingStore BackingStore { get; private set; }

    /// <summary>
    /// Creates an insert operation for this entity
    /// </summary>
    /// <returns></returns>
    internal virtual DatabaseOperation ToInsertOperation() => new DatabaseOperation("insert", Name, this);
    /// <summary>
    /// Creates an update operation for this entity
    /// </summary>
    /// <returns></returns>
    internal virtual DatabaseOperation ToUpdateOperation() => new DatabaseOperation("update", Name, this);

    /// <summary>
    /// Creates an delete operation for this entity
    /// </summary>
    /// <returns></returns>
    internal virtual DatabaseOperation ToDeleteOperation() => new DatabaseOperation("delete", Name, this);
}


internal static class DatabaseEntityExtensions
{
    /// <summary>
    /// Sets the value in the backing store and marks it as dirty to the specified value, normally <c>false</c>.
    /// </summary>
    public static void SetDirty(this IEnumerable<DatabaseEntity> entities, bool value)
    {
        ArgumentNullException.ThrowIfNull(entities);

        foreach (var entity in entities)
        {
            entity.BackingStore.Dirty = value;
        }
    }
}
