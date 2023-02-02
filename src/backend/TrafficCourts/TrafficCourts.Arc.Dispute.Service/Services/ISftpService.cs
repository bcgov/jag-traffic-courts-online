namespace TrafficCourts.Arc.Dispute.Service.Services;

public interface ISftpService : IDisposable
{
    void UploadFile(MemoryStream file, string path, string filename);
}
