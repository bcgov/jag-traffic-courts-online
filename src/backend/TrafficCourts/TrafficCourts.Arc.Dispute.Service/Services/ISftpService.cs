namespace TrafficCourts.Arc.Dispute.Service.Services
{
    public interface ISftpService : IDisposable
    {
        void UploadFile(Stream file, string remoteFilePath);
    }
}
