
/// <summary>
/// Simple <see cref="IMemoryStreamManager"/> that can be used in tests.
/// </summary>
public class SimpleMemoryStreamManager : IMemoryStreamManager
{
    public MemoryStream GetStream() => new MemoryStream();
}
