namespace TrafficCourts.OrdsDataService;

/// <summary>
/// Represents a sequence of operations to execute in the database.
/// </summary>
public class BatchDatabaseOperation : List<IDictionary<string, object?>>
{
    /// <summary>
    /// Adds an entity to the batch as an insert operation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entity"></param>
    public void Insert<T>(T entity) where T : DatabaseEntity
    {
        Add(entity.ToInsertOperation());
    }

    /// <summary>
    /// Adds an entity to the batch as an update operation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entity"></param>
    public void Update<T>(T entity) where T : DatabaseEntity
    {
        Add(entity.ToUpdateOperation());
    }

    /// <summary>
    /// Adds an entity to the batch as a delete operation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entity"></param>
    public void Delete<T>(T entity) where T : DatabaseEntity
    {
        Add(entity.ToDeleteOperation());
    }
}
