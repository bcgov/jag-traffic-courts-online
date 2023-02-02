namespace TrafficCourts.Coms.Client;

public partial class File : IDisposable
{
    /// <summary>Is this already disposed?</summary>
    private bool _disposed;
    /// <summary>Does this instance own the memory stream and should dispose of it?</summary>
    private bool _ownsDataStream = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="File"/> class.
    /// </summary>
    /// <param name="data">The data stream containing the file data. The file stream will be rewound so the file position is at the beginning.</param>
    public File(Stream data)
        : this(data, null, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="File"/> class.
    /// </summary>
    /// <param name="data">The data stream containing the file data. The file stream will be rewound so the file position is at the beginning.</param>
    /// <param name="fileName">The optional file name.</param>
    public File(Stream data, string? fileName)
        : this(data, fileName, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="File"/> class.
    /// </summary>
    /// <param name="data">The data stream containing the file data. The file stream will be rewound so the file position is at the beginning.</param>
    /// <param name="fileName">The optional file name.</param>
    /// <param name="contentType">The optional content type of the file</param>
    public File(Stream data, string? fileName, string? contentType)
        : this(data, fileName, contentType, null, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="File"/> class.
    /// </summary>
    /// <param name="data">The data stream containing the file data. The file stream will be rewound so the file position is at the beginning.</param>
    /// <param name="fileName">The optional file name.</param>
    /// <param name="contentType">The optional content type of the file</param>
    /// <param name="metadata">The optional metadata properties</param>
    /// <param name="tags">The optional tag properties</param>
    public File(Stream data, string? fileName, string? contentType, IDictionary<string, string>? metadata, IDictionary<string, string>? tags)
        : this(null, data, fileName, contentType, metadata, tags)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="File"/> class.
    /// </summary>
    /// <param name="id">The system generated file identifier.</param>
    /// <param name="data">The data stream containing the file data. The file stream will be rewound so the file position is at the beginning.</param>
    /// <param name="fileName">The optional file name.</param>
    /// <param name="contentType">The optional content type of the file</param>
    /// <param name="metadata">The optional metadata properties</param>
    /// <param name="tags">The optional tag properties</param>
    public File(Guid? id, Stream data, string? fileName, string? contentType, IDictionary<string, string>? metadata, IDictionary<string, string>? tags)
    {
        Id = id;
        Data = data;
        FileName = fileName;
        ContentType = contentType;

        Metadata = Client.Metadata.Create(metadata);
        Tags = Factory.CreateTags(tags);

        // data shouldn't be null, but to be on the safe side as there is no guard 
        if (data is not null)
        {
            data.Position = 0; // rewind the stream so callers can consume the data
        }
    }

    /// <summary>
    /// The id of the document. Only set when getting documents.
    /// </summary>
    public Guid? Id { get; internal set; }

    public Stream Data { get; private set; }

    public string? FileName { get; private set; }

    public string? ContentType { get; private set; }

    public Dictionary<string, string> Metadata { get; private set; }
    public Dictionary<string, string> Tags { get; private set; }

    public void Dispose()
    {
        Dispose(disposing: true);
        // This object will be cleaned up by the Dispose method.
        // Therefore, you should call GC.SuppressFinalize to
        // take this object off the finalization queue
        // and prevent finalization code for this object
        // from executing a second time.
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                if (_ownsDataStream)
                {
                    Data.Dispose();
                }

                _disposed = true;
            }
        }
    }
}
