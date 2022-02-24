using FlatFiles;
using FlatFiles.TypeMapping;
using Microsoft.Extensions.Logging.Abstractions;
using Renci.SshNet;
using System.Text;
using TrafficCourts.Arc.Dispute.Service.Configuration;
using TrafficCourts.Arc.Dispute.Service.Models;

namespace TrafficCourts.Arc.Dispute.Service.Services
{
    public class ArcFileService : IArcFileService
    {
        public async Task createArcFile(List<ArcFileRecord> arcFileData)
        {
            // Inject ticket mapper function based on the derived class object
            var selector = new FixedLengthTypeMapperInjector();
            selector.When<AdnotatedTicket>().Use(CreateAdnotatedTicketMapper());
            selector.When<DisputedTicket>().Use(CreateDisputedTicketMapper());

            /*
            Random rnd = new Random();
            int num = rnd.Next(100);
            string fileName = "arcfile" + num + ".txt";
            */
            // Create a name for the file from the unique file number field of the dispute ticket data
            string fileName = "dispute-" + arcFileData[0].FileNumber + ".txt";
            // Write ARC data into the memory stream that will be uploaded as a file
            var stream = new MemoryStream();
            var fileWriter = new StreamWriter(stream, Encoding.UTF8);
            var writer = selector.GetWriter(fileWriter);

            foreach (var record in arcFileData)
            {
                await writer.WriteAsync(record);
            }

            fileWriter.Flush();
            stream.Position = 0;

            // Configure sftp connection with the following required parameters
            var config = new SftpConfig
            {
                Host = "localhost",
                Port = 22,
                Username = "demo",
                Password = "demo"
            };
            // Create sftp service client
            var sftpService = new SftpService(new NullLogger<SftpService>(), config);
            // Define the path where the file will be uploaded including the filename
            string remoteFilePath = @"./" + fileName;
            // Call sftp upload service
            sftpService.UploadFile(stream, remoteFilePath);
        }

        public static IFixedLengthTypeMapper<AdnotatedTicket> CreateAdnotatedTicketMapper()
        {
            var mapper = FixedLengthTypeMapper.Define<AdnotatedTicket>(() => new AdnotatedTicket());

            mapper.Property(_ => _.LockoutFlag, new Window(1)).ColumnName("lockout_flag");
            mapper.Property(_ => _.TransactionDate, new Window(8)).ColumnName("transaction_date").OutputFormat("yyyyMMdd");
            mapper.Property(_ => _.TransactionTime, new Window(6)).ColumnName("transaction_time").OutputFormat("hhmmss");
            mapper.Property(_ => _.TransactionLocation, new Window(5)).ColumnName("transaction_location");
            mapper.Property(_ => _.TransactionType, new Window(3)).ColumnName("transaction_type");
            mapper.Property(_ => _.EffectiveDate, new Window(8)).ColumnName("effective_date").OutputFormat("yyyyMMdd");
            mapper.Property(_ => _.Owner, new Window(5)).ColumnName("owner");
            mapper.Property(_ => _.FileNumber, new Window(14)).ColumnName("file_number");
            mapper.Property(_ => _.CountNumber, new Window(3)).ColumnName("count_number");
            mapper.Property(_ => _.ReceivableType, new Window(1)).ColumnName("receivable_type");
            mapper.Property(_ => _.TransactionNumber, new Window(5)).ColumnName("transaction_number");
            mapper.Property(_ => _.MvbClientNumber, new Window(9)).ColumnName("mvb_client_number");
            mapper.Property(_ => _.UpdateFlag, new Window(1)).ColumnName("update_flag");
            mapper.Property(_ => _.Name, new Window(30)).ColumnName("name");
            mapper.Property(_ => _.Section, new Window(6) { Alignment = FixedAlignment.RightAligned }).ColumnName("section");
            mapper.Property(_ => _.Subsection, new Window(2) { Alignment = FixedAlignment.RightAligned }).ColumnName("subsection");
            mapper.Property(_ => _.Paragraph, new Window(1)).ColumnName("paragraph");
            mapper.Property(_ => _.Act, new Window(3)).ColumnName("act");
            mapper.Property(_ => _.OriginalAmount, new Window(9) { FillCharacter = '0', Alignment = FixedAlignment.RightAligned }).ColumnName("original_amount").OutputFormat("N2");
            mapper.Property(_ => _.Organization, new Window(4)).ColumnName("organization");
            mapper.Property(_ => _.OrganizationLocation, new Window(20)).ColumnName("organization_location");
            mapper.Property(_ => _.ServiceDate, new Window(8)).ColumnName("service_date").OutputFormat("yyyyMMdd");
            mapper.Property(_ => _.Filler, new Window(103)).ColumnName("filler");
            mapper.Property(_ => _.ARCF0630RecordFlag, new Window(1)).ColumnName("record_flag");

            return mapper;
        }

        public static IFixedLengthTypeMapper<DisputedTicket> CreateDisputedTicketMapper()
        {
            var mapper = FixedLengthTypeMapper.Define<DisputedTicket>(() => new DisputedTicket());

            mapper.Property(_ => _.LockoutFlag, new Window(1)).ColumnName("lockout_flag");
            mapper.Property(_ => _.TransactionDate, new Window(8)).ColumnName("transaction_date").OutputFormat("yyyyMMdd");
            mapper.Property(_ => _.TransactionTime, new Window(6)).ColumnName("transaction_time").OutputFormat("hhmmss");
            mapper.Property(_ => _.TransactionLocation, new Window(5)).ColumnName("transaction_location");
            mapper.Property(_ => _.TransactionType, new Window(3)).ColumnName("transaction_type");
            mapper.Property(_ => _.EffectiveDate, new Window(8)).ColumnName("effective_date").OutputFormat("yyyyMMdd");
            mapper.Property(_ => _.Owner, new Window(5)).ColumnName("owner");
            mapper.Property(_ => _.FileNumber, new Window(14)).ColumnName("file_number");
            mapper.Property(_ => _.CountNumber, new Window(3)).ColumnName("count_number");
            mapper.Property(_ => _.ReceivableType, new Window(1)).ColumnName("receivable_type");
            mapper.Property(_ => _.TransactionNumber, new Window(5)).ColumnName("transaction_number");
            mapper.Property(_ => _.MvbClientNumber, new Window(9)).ColumnName("mvb_client_number");
            mapper.Property(_ => _.UpdateFlag, new Window(1)).ColumnName("update_flag");
            mapper.Property(_ => _.ServiceDate, new Window(8)).ColumnName("service_date").OutputFormat("yyyyMMdd");
            mapper.Property(_ => _.Name, new Window(30)).ColumnName("name");
            mapper.Property(_ => _.DisputeType, new Window(1)).ColumnName("dispute_type");
            mapper.Property(_ => _.StreetAddress, new Window(30)).ColumnName("street_address");
            mapper.Property(_ => _.City, new Window(20)).ColumnName("city");
            mapper.Property(_ => _.Province, new Window(2)).ColumnName("province");
            mapper.Property(_ => _.PostalCode, new Window(6)).ColumnName("postal_code");
            mapper.Property(_ => _.Filler, new Window(89)).ColumnName("filler");
            mapper.Property(_ => _.ARCF0630RecordFlag, new Window(1)).ColumnName("record_flag");

            return mapper;
        }
    }
}
