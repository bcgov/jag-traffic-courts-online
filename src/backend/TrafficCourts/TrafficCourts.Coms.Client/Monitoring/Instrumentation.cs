using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace TrafficCourts.Coms.Client.Monitoring;


internal static class Instrumentation
{
    public const string MeterName = "ComsClient";

    static Meter _meter;

    // create object
    private static readonly Timer _createObjectsDuration;
    private static readonly Counter<long> _createObjectsErrorTotal;

    // delete object
    private static readonly Timer _deleteObjectDuration;
    private static readonly Counter<long> _deleteObjectErrorTotal;

    // read object
    private static readonly Timer _readObjectDuration;
    private static readonly Counter<long> _readObjectErrorTotal;

    // fetc metadata
    private static readonly Timer _fetchMetadataDuration;
    private static readonly Counter<long> _fetchMetadataErrorTotal;

    // fetch tags
    private static readonly Timer _fetchTagsDuration;
    private static readonly Counter<long> _fetchTagsErrorTotal;

    // search objects
    private static readonly Timer _searchObjectsDuration;
    private static readonly Counter<long> _searchObjectsErrorTotal;

    // other operations
    private static readonly Timer _otherOperationDuration;
    private static readonly Counter<long> _otherOperationErrorTotal;

    static Instrumentation()
    {
        _meter = new Meter(MeterName);

        _createObjectsDuration = new Timer(_meter, "coms.create_file.duration", "ms", "Elapsed time spent creating a file");
        _createObjectsErrorTotal = _meter.CreateCounter<long>("coms.create_file.errors", "ea", "Number of times a file could not be created due to an error");

        _deleteObjectDuration = new Timer(_meter, "coms.delete_file.duration", "ms", "Elapsed time spent deleting a file");
        _deleteObjectErrorTotal = _meter.CreateCounter<long>("coms.delete_file.errors", "ea", "Number of times a file could not be deleted due to an error");

        _readObjectDuration = new Timer(_meter, "coms.read_file.duration", "ms", "Elapsed time spent getting a file");
        _readObjectErrorTotal = _meter.CreateCounter<long>("coms.read_file.errors", "ea", "Number of times a file could not be read due to an error");

        _fetchMetadataDuration = new Timer(_meter, "coms.fetch_metadata.duration", "ms", "Elapsed time spent fetching metadata");
        _fetchMetadataErrorTotal = _meter.CreateCounter<long>("coms.fetch_metadata.errors", "ea", "Number of times fetching metadata could not be completed due to an error");

        _fetchTagsDuration = new Timer(_meter, "coms.fetch_tags.duration", "ms", "Elapsed time spent fetching tags");
        _fetchTagsErrorTotal = _meter.CreateCounter<long>("coms.fetch_tags.errors", "ea", "Number of times fetching tags could not be completed due to an error");

        _searchObjectsDuration = new Timer(_meter, "coms.search_files.duration", "ms", "Elapsed time spent searching for files");
        _searchObjectsErrorTotal = _meter.CreateCounter<long>("coms.search_files.errors", "ea", "Number of times searching for files could not be completed due to an error");

        _otherOperationDuration = new Timer(_meter, "coms.operation.duration", "ms", "Elapsed time spent performing other operations");
        _otherOperationErrorTotal = _meter.CreateCounter<long>("coms.operation.errors", "ea", "Number of times performing other operations could not be completed due to an error");
    }

    #region Create Objects
    public static ITimerOperation BeginCreateObjects()
    {
        return _createObjectsDuration.Start();
    }

    public static void EndCreateObjects(ITimerOperation operation, Exception exception)
    {
        operation.Error(exception);
        End(_createObjectsErrorTotal, operation, exception);
    }
    #endregion

    #region Delete Object
    public static ITimerOperation BeginDeleteObject()
    {
        return _deleteObjectDuration.Start();
    }

    public static void EndDeleteObject(ITimerOperation operation, Exception exception)
    {
        operation.Error(exception);
        End(_deleteObjectErrorTotal, operation, exception);
    }
    #endregion

    #region Read Object
    public static ITimerOperation BeginReadObject()
    {
        return _readObjectDuration.Start();
    }

    public static void EndReadObject(ITimerOperation operation, Exception exception)
    {
        operation.Error(exception);
        End(_readObjectErrorTotal, operation, exception);
    }
    #endregion

    #region Fetch Metadata
    public static ITimerOperation BeginFetchMetadata()
    {
        return _fetchMetadataDuration.Start();
    }

    public static void EndFetchMetadata(ITimerOperation operation, Exception exception)
    {
        operation.Error(exception);
        End(_fetchMetadataErrorTotal, operation, exception);
    }
    #endregion

    #region Fetch Tags
    public static ITimerOperation BeginFetchTags()
    {
        return _fetchTagsDuration.Start();
    }

    public static void EndFetchTags(ITimerOperation operation, Exception exception)
    {
        operation.Error(exception);
        End(_fetchTagsErrorTotal, operation, exception);
    }
    #endregion

    #region Search Objects
    public static ITimerOperation BeginSearchObjects()
    {
        return _searchObjectsDuration.Start();
    }

    public static void EndSearchObjects(ITimerOperation operation, Exception exception)
    {
        operation.Error(exception);
        End(_searchObjectsErrorTotal, operation, exception);
    }
    #endregion

    #region Other Operation
    public static ITimerOperation BeginOtherOperation(string operationName)
    {
        return _otherOperationDuration.Start(new TagList { { "operation_name", operationName } });
    }

    public static void EndOtherOperation(ITimerOperation operation, Exception exception)
    {
        operation.Error(exception);
        End(_otherOperationErrorTotal, operation, exception);
    }
    #endregion

    /// <summary>
    /// The operation ended due to an exception.
    /// </summary>
    private static void End(Counter<long> errorTotal, ITimerOperation operation, Exception exception)
    {
        // let the timer know there was an excetion
        operation.Error(exception);

        TagList tagList = new()
        {
            { "exception_type", exception.GetType().Name }
        };

        // common exception types

        if (exception is ApiException apiException)
        {
            tagList.Add("http_status_code", apiException.StatusCode);
        }

        // increment the error counter
        errorTotal.Add(1, tagList);
    }
}

