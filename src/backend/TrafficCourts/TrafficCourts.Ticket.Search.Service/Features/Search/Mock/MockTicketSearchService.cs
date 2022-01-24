using FlatFiles;
using FlatFiles.TypeMapping;

namespace TrafficCourts.Ticket.Search.Service.Features.Search.Mock
{
    public class MockTicketSearchService : ITicketSearchService
    {
        private readonly IMockDataProvider _mockDataProvider;
        private readonly ILogger<MockTicketSearchService> _logger;

        public MockTicketSearchService(IMockDataProvider mockDataProvider, ILogger<MockTicketSearchService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mockDataProvider = mockDataProvider ?? throw new ArgumentNullException(nameof(mockDataProvider));
        }

        public Task<IEnumerable<Invoice>> SearchTicketAsync(string ticketNumber, string time, CancellationToken cancellationToken)
        {
            IEnumerable<Invoice> database = GetMockData();

            // find all the records where the invoice number starts with ticket number, and ends with a valid count(1..3)
            Func<Invoice, bool> predicate = record =>
                !string.IsNullOrEmpty(record.InvoiceNumber) &&
                record.InvoiceNumber.StartsWith(ticketNumber) &&
                record.InvoiceNumber.Length - 1 == ticketNumber.Length &&
                (record.InvoiceNumber[^1] == '1' || record.InvoiceNumber[^1] == '2' || record.InvoiceNumber[^1] == '3');

            IEnumerable<Invoice> invoices = database
                .Where(predicate)
                .OrderBy(_ => _.InvoiceNumber);

            return Task.FromResult(invoices);
        }

        private IEnumerable<Invoice> GetMockData()
        {
            ISeparatedValueTypeMapper<Invoice> mapper = GetTypeMapper();

            using Stream? stream = _mockDataProvider.GetDataStream();
            if (stream is null)
            {
                return Enumerable.Empty<Invoice>();
            }

            using var reader = new StreamReader(stream);
            var options = new SeparatedValueOptions() { IsFirstRecordSchema = true };
            var invoices = mapper.Read(reader, options).ToList();
            return invoices;
        }

        private ISeparatedValueTypeMapper<Invoice> GetTypeMapper()
        {
            ISeparatedValueTypeMapper<Invoice> mapper = SeparatedValueTypeMapper.Define<Invoice>();
            mapper.Property(_ => _.InvoiceNumber).ColumnName("invoice_number");
            mapper.Property(_ => _.PbcRefNumber).ColumnName("pbc_ref_number");
            mapper.Property(_ => _.PartyNumber).ColumnName("party_number");
            mapper.Property(_ => _.PartyName).ColumnName("party_name");
            mapper.Property(_ => _.AccountNumber).ColumnName("account_number");
            mapper.Property(_ => _.SiteNumber).ColumnName("site_number");
            mapper.Property(_ => _.InvoiceType).ColumnName("cust_trx_type");
            mapper.Property(_ => _.ViolationDateTime).ColumnName("term_due_date");
            mapper.Property(_ => _.TicketedAmount).ColumnName("total");
            mapper.Property(_ => _.AmountDue).ColumnName("amount_due");
            mapper.Property(_ => _.OffenceDescription).ColumnName("attribute1");
            mapper.Property(_ => _.VehicleDescription).ColumnName("attribute2");
            mapper.Property(_ => _.ViolationDate).ColumnName("attribute3");
            mapper.Property(_ => _.DiscountAmount).ColumnName("attribute4");

            return mapper;
        }
    }
}
