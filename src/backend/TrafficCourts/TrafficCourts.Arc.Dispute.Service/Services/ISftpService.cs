namespace TrafficCourts.Arc.Dispute.Service.Services
{
    public interface ISftpService
    {
        void UploadFile(Stream file, string remoteFilePath);
    }
}
