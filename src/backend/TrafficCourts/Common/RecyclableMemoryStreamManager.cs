public class RecyclableMemoryStreamManager : IMemoryStreamManager
{
    // see https://github.com/microsoft/Microsoft.IO.RecyclableMemoryStream
    private readonly Microsoft.IO.RecyclableMemoryStreamManager _manager = new Microsoft.IO.RecyclableMemoryStreamManager();

    public MemoryStream GetStream() => _manager.GetStream();
}
