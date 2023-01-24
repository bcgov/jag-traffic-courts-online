namespace BCGov.VirusScan.Api.Services;

public interface IRecyclableMemoryStreamManager
{
    MemoryStream GetStream(string tag, int requiredSize, bool asContiguousBuffer = false);
}


