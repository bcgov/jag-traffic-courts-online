namespace TrafficCourts.Workflow.Service.Consumers
{
    public class DocumentVirusScanConsumerException : Exception
    {
        public DocumentVirusScanConsumerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
