namespace TrafficCourts.Arc.Dispute.Service.Services;

public interface ISftpService
{
    void UploadFile(MemoryStream file, string path, string filename);
}
