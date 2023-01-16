using Microsoft.IO;

namespace BCGov.VirusScan.Api.Services;

public class DefaultRecyclableMemoryStreamManager : IRecyclableMemoryStreamManager
{
    private readonly RecyclableMemoryStreamManager _manager;

    public DefaultRecyclableMemoryStreamManager(RecyclableMemoryStreamManager manager)
    {
        _manager = manager ?? throw new ArgumentNullException(nameof(manager));
    }

    public MemoryStream GetStream(string tag, int requiredSize, bool asContiguousBuffer = false) => _manager.GetStream(tag, requiredSize, asContiguousBuffer);
}
