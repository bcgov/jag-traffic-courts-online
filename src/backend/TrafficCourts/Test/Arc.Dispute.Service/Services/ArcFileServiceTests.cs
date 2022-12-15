using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TrafficCourts.Arc.Dispute.Service.Configuration;
using TrafficCourts.Arc.Dispute.Service.Models;
using TrafficCourts.Arc.Dispute.Service.Services;
using Xunit;

namespace TrafficCourts.Test.Arc.Dispute.Service.Services
{
    public class ArcFileServiceTests
    {
        private Mock<ISftpService> _mockArcFileService;
        private readonly ILogger<ArcFileService> _logger;
        private IMemoryStreamManager _memoryStreamManager = new SimpleMemoryStreamManager();

        public ArcFileServiceTests()
        {
            _mockArcFileService = new Mock<ISftpService>();
            _logger = new Mock<ILogger<ArcFileService>>().Object;
        }

        [Theory]
        [MemberData(nameof(EachArcFileRecordType))]
        public async Task returned_stream_should_be_at_the_beginning(ArcFileRecord record)
        {
            // Act
            var stream = await CreateFileAsync(record);

            // assert
            Assert.Equal(0, stream.Position); // should be at the start
        }

        public static IEnumerable<object[]> EachArcFileRecordType
        {
            get
            {
                yield return new object[] { new AdnotatedTicket() };
                yield return new object[] { new DisputedTicket() };
            }
        }

        public static IEnumerable<object[]> EachArcFileRecordTypeWithDateAndTime
        {
            get
            {
                var year = 2024; // pick a leap year

                foreach (var item in EachArcFileRecordType)
                {
                    var record = item[0];

                    for (int month = 1; month <= 12; month++)
                    {
                        for (int day = 1; day <= DateTime.DaysInMonth(year, month); day++)
                        {
                            // single digit times
                            yield return new object[] { record, new DateTime(year, month, day, 1, 2, 3, DateTimeKind.Local) };
                            // double digit times
                            yield return new object[] { record, new DateTime(year, month, day, 10, 11, 12, DateTimeKind.Local) };
                        }
                    }
                }
            }
        }

        public static IEnumerable<object[]> EachArcFileRecordTypeWithDate
        {
            get
            {
                var year = 2024; // pick a leap year

                foreach (var item in EachArcFileRecordType)
                {
                    var record = item[0];

                    for (int month = 1; month <= 12; month++)
                    {
                        for (int day = 1; day <= DateTime.DaysInMonth(year, month); day++)
                        {
                            yield return new object[] { record, new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Local) };
                        }
                    }
                }
            }
        }

        [Theory]
        [MemberData(nameof(EachArcFileRecordType))]
        public async Task record_should_have_a_length_of_256_and_end_with_newline(ArcFileRecord record)
        {
            // Act
            var actual = await CreateFileAsStringAsync(record);

            // assert
            Assert.NotNull(actual);
            Assert.Equal(256 + 1, actual.Length);
            Assert.Equal('\n', actual[^1]); // should end with unix new line
        }

        [Theory]
        [MemberData(nameof(EachArcFileRecordType))]
        public async Task lockout_flag_should_be_zero(ArcFileRecord record)
        {
            // Act
            var actual = await CreateFileAsStringAsync(record);

            // 1-   1      Lockout Flag 0=Unlocked; 1=Locked
            Assert.Equal("0", Get(actual, 1, 1));
        }


        [Theory]
        [MemberData(nameof(EachArcFileRecordTypeWithDateAndTime))]
        public async Task transaction_date_and_time_should_be_correct(ArcFileRecord record, DateTime date)
        {
            record.TransactionDateTime = date;

            var actual = await CreateFileAsStringAsync(record);

            // 2-   9      Transaction Date (yyyymmdd)
            Assert.Equal(date.ToString("yyyyMMdd"), Get(actual, 2, 9));

            // 10-  15      Transaction Time
            Assert.Equal(date.ToString("HHmmss"), Get(actual, 10, 15));
        }

        [Theory]
        [MemberData(nameof(EachArcFileRecordType))]
        public async Task transaction_location_should_be_be_correct(ArcFileRecord record)
        {
            // Act
            var actual = await CreateFileAsStringAsync(record);

            // 
            Assert.Equal("20254", Get(actual, 16, 20)); // column 16-20
        }

        [Fact]
        public async Task adnotated_ticket_record_should_have_EV_transaction_type()
        {
            var actual = await CreateFileAsStringAsync(new AdnotatedTicket());

            // 21-  23      Transaction Type   
            Assert.Equal("EV ", Get(actual, 21, 23));
        }


        [Fact]
        public async Task dispute_ticket_record_should_have_ED_transaction_type()
        {
            var actual = await CreateFileAsStringAsync(new DisputedTicket());

            // 21-  23      Transaction Type   
            Assert.Equal("ED ", Get(actual, 21, 23));
        }

        [Theory]
        [MemberData(nameof(EachArcFileRecordTypeWithDate))]
        public async Task effective_date_should_be_correct(ArcFileRecord record, DateTime date)
        {
            record.EffectiveDate = date;

            var actual = await CreateFileAsStringAsync(record);

            // 24-  31      Effective Date (yyyymmdd) 
            Assert.Equal(date.ToString("yyyyMMdd"), Get(actual, 24, 31));
        }

        [Theory]
        [MemberData(nameof(EachArcFileRecordType))]
        public async Task owner_should_be_00001(ArcFileRecord record)
        {
            var actual = await CreateFileAsStringAsync(record);

            // 32 - 36      Owner
            Assert.Equal("00001", Get(actual, 32, 36));
        }

        [Theory]
        [MemberData(nameof(EachArcFileRecordType))]
        public async Task file_number_should_be_correct(ArcFileRecord record)
        {
            var expected = Guid.NewGuid().ToString("n")[0..14];
            record.FileNumber = expected;

            var actual = await CreateFileAsStringAsync(record);

            // 37-  50      File Number 
            Assert.Equal(expected, Get(actual, 37, 50));
        }

        [Theory]
        [MemberData(nameof(EachArcFileRecordType))]
        public async Task count_number_should_be_correct(ArcFileRecord record)
        {
            var expected = Guid.NewGuid().ToString("n")[0..3];
            record.CountNumber = expected;

            var actual = await CreateFileAsStringAsync(record);

            // 51-  53      Count Number 
            Assert.Equal(expected, Get(actual, 51, 53));
        }

        [Theory]
        [MemberData(nameof(EachArcFileRecordType))]
        public async Task receivable_type_should_be_correct(ArcFileRecord record)
        {
            var actual = await CreateFileAsStringAsync(record);

            // 54-  54      Receivable Type 
            Assert.Equal("M", Get(actual, 54, 54));
        }

        [Theory]
        [MemberData(nameof(EachArcFileRecordType))]
        public async Task transaction_number_should_be_00000(ArcFileRecord record)
        {
            var actual = await CreateFileAsStringAsync(record);

            // 55-  59      Transaction Number
            Assert.Equal("00000", Get(actual, 55, 59));
        }

        [Theory]
        [MemberData(nameof(EachArcFileRecordType))]
        public async Task mvb_client_number_should_be_correct(ArcFileRecord record)
        {
            var expected = Guid.NewGuid().ToString("n")[0..9];
            record.MvbClientNumber = expected;

            var actual = await CreateFileAsStringAsync(record);

            // 60-  68      MVB Client Number 
            Assert.Equal(expected, Get(actual, 60, 68));
        }


        [Theory]
        [MemberData(nameof(EachArcFileRecordType))]
        public async Task update_flag_should_be_correct(ArcFileRecord record)
        {
            var actual = await CreateFileAsStringAsync(record);

            // 69-  69      Update Flag
            Assert.Equal(" ", Get(actual, 69, 69));
        }

        [Theory]
        [MemberData(nameof(EachArcFileRecordType))]
        public async Task filler_should_be_correct(ArcFileRecord record)
        {
            var actual = await CreateFileAsStringAsync(record);

            // 170- 255      Filler
            var expected = new string(' ', 255 - 170 + 1);
            Assert.Equal(expected, Get(actual, 170, 255));
        }


        [Theory]
        [MemberData(nameof(EachArcFileRecordType))]
        public async Task ARCF0630_should_be_correct(ArcFileRecord record)
        {
            var actual = await CreateFileAsStringAsync(record);

            // 256- 256      Create ARCF0630 Record Flag
            Assert.Equal(" ", Get(actual, 256, 256));
        }

        /*
ARCUDTLS DIM       100          70- 169      Transaction Details  (Overlay)        # This is the section of the file row reserved for either "Adnotated Ticket"/"Dispute" data
ARCUFILL DIM       86          170- 255      Filler                                # Nothing   
ARCUICBC DIM       1           256- 256      Create ARCF0630 Record Flag           # Leave blank (Set by system)      
         */

        /// <summary>
        /// Helper method to extract specific 1-based data from the given string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="from">The column from</param>
        /// <param name="to">The column to (inclusive)</param>
        /// <returns></returns>
        private static string Get(string value, int from, int to)
        {
            Assert.NotNull(value);
            Assert.True(1 <= from);
            Assert.True(from <= to);

            int expectedLength = to - from + 1;
            
            from--; // we are zero based and "to" is not included in the range selector

            var result = value[from..to];
            Assert.Equal(expectedLength, result.Length);

            return result;
        }

        private async Task<MemoryStream> CreateFileAsync(ArcFileRecord record)
        {
            return await CreateFileAsync(new[] { record });
        }

        private async Task<string> CreateFileAsStringAsync(ArcFileRecord record)
        {
            var stream = await CreateFileAsync(new[] { record });
            return AsString(stream);
        }

        private async Task<MemoryStream> CreateFileAsync(IList<ArcFileRecord> records)
        {
            SftpOptions options = new SftpOptions();
            var sut = new ArcFileService(_mockArcFileService.Object, _memoryStreamManager, options, _logger);
            var stream = await sut.CreateStreamFromArcDataAsync(records, CancellationToken.None);
            return stream;
        }

        private static string AsString(MemoryStream stream) => Encoding.ASCII.GetString(stream.ToArray());

        private IEnumerable<string> GetLines(MemoryStream stream)
        {
            var reader = new StreamReader(stream);
            string? line;
            while ((line = reader.ReadLine()) is not null)
            {
                yield return line;
            }
        }

        [Fact]
        public async Task TestCreateStreamFromArcData()
        {
            // Arrange
            SftpOptions options = new SftpOptions();
            var arcFileService = new ArcFileService(_mockArcFileService.Object, _memoryStreamManager, options, _logger);

            var records = new List<ArcFileRecord>();
            records.Add(new AdnotatedTicket());

            // Act
            var actual = await arcFileService.CreateStreamFromArcDataAsync(records, CancellationToken.None);

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(0, actual.Position);
            Assert.NotEqual(0, actual.Length);
        }

        [Fact]
        public async void Test_throw_ArgumentNullException_if_null_value_passed_for_CreateArcFile()
        {
            // Arrange
            SftpOptions options = new SftpOptions();
            var arcFileService = new ArcFileService(_mockArcFileService.Object, _memoryStreamManager, options, _logger);

            await Assert.ThrowsAsync<ArgumentNullException>(() => arcFileService.CreateArcFile(null!, CancellationToken.None));
        }


        [Fact]
        public async void when_creating_arc_file_it_is_upload_to_correct_remote_path()
        {
            // Arrange
            SftpOptions options = new SftpOptions { RemotePath = Guid.NewGuid().ToString("n") };
            string fileNumber = Guid.NewGuid().ToString("n");

            _mockArcFileService = new Mock<ISftpService>(MockBehavior.Strict);

            // expecte the uploaded file to be uploaded to the remote path, using the file number with extension .txt
            string expectedFilename = "dispute-" + fileNumber + ".txt";
            _mockArcFileService.Setup(service => service.UploadFile(
                It.IsAny<MemoryStream>(),
                It.Is<string>(_ => _ == options.RemotePath),
                It.Is<string>(_ => _ == expectedFilename)
            ));

            var records = new List<ArcFileRecord> { new AdnotatedTicket { FileNumber = fileNumber } };
            var arcFileService = new ArcFileService(_mockArcFileService.Object, _memoryStreamManager, options, _logger);

            // Act
            await arcFileService.CreateArcFile(records, CancellationToken.None);
        }
    }
}
