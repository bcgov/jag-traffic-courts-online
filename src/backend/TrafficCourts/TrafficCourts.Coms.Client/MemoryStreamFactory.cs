namespace TrafficCourts.Coms.Client;

internal class MemoryStreamFactory : IMemoryStreamFactory
{
    private readonly Func<MemoryStream> _memoryStreamFactory;

    public MemoryStreamFactory(Func<MemoryStream> memoryStreamFactory)
    {
        _memoryStreamFactory = memoryStreamFactory ?? throw new ArgumentNullException(nameof(memoryStreamFactory));
    }

    public MemoryStream GetStream() => _memoryStreamFactory();
}
