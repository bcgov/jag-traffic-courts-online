using FlatFiles;
using FlatFiles.TypeMapping;
using NodaTime;
using System.Globalization;
using TrafficCourts.Citizen.Service.Services.Tickets.Search.Common;
using TrafficCourts.Common;

namespace TrafficCourts.Citizen.Service.Services.Tickets.Search.Mock
{
    public class MockTicketSearchService : ITicketInvoiceSearchService
    {
        private readonly IMockDataProvider _mockDataProvider;
        private readonly ILogger<MockTicketSearchService> _logger;
        private readonly IClock _clock;

        public MockTicketSearchService(IMockDataProvider mockDataProvider, ILogger<MockTicketSearchService> logger, IClock clock)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _mockDataProvider = mockDataProvider ?? throw new ArgumentNullException(nameof(mockDataProvider));
        }

        public Task<IList<Invoice>> SearchAsync(string ticketNumber, TimeOnly issuedTime, CancellationToken cancellationToken)
        {
            using var activity = Diagnostics.Source.StartActivity("Mock Ticket Search");

            IList<Invoice> invoices = Array.Empty<Invoice>();

            IList<Invoice> database = GetMockData();
            if (database.Count == 0)
            {
                return Task.FromResult(invoices);
            }

            // find all the records where the invoice number starts with ticket number, and ends with a valid count(1..3)
            Func<Invoice, bool> predicate = record =>
                !string.IsNullOrEmpty(record.InvoiceNumber) &&
                record.InvoiceNumber.StartsWith(ticketNumber) &&
                record.InvoiceNumber.Length - 1 == ticketNumber.Length &&
                (record.InvoiceNumber[^1] == '1' || record.InvoiceNumber[^1] == '2' || record.InvoiceNumber[^1] == '3');

            invoices = database
                .Where(predicate)
                .OrderBy(_ => _.InvoiceNumber)
                .ToList();

            AdjustTicketDate(database, invoices);

            return Task.FromResult(invoices);
        }

        private void AdjustTicketDate(IList<Invoice> database, IList<Invoice> invoices)
        {
            if (!TryParse(database[0].ViolationDateTime!, out DateTime baseViolationDateTime))
            {
                return;
            }

            // On May 9th, the first ticket date was May 8th. We want
            // the first ticket to be on the previous date based on today's
            // date

            baseViolationDateTime = baseViolationDateTime.Date.AddDays(1);  // May 8 -> May 9
            var today = _clock.GetCurrentPacificTime().DateTime.Date;       // May 9

            var daysToAdjust = (today - baseViolationDateTime).Days;        // this will be >= 0, bring dates forward

            foreach (var invoice in invoices)
            {
                AddDays(invoice, daysToAdjust);
            }
        }

        private const string ViolationDateTimeFormat = "yyyy-MM-ddTHH:mm";
        private const string ViolationDateFormat = "yyyy-MM-dd";

        private bool TryParse(string value, out DateTime result)
        {
            return DateTime.TryParseExact(value, ViolationDateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out result);
        }

        private void AddDays(Invoice invoice, int days)
        {
            if (TryParse(invoice.ViolationDateTime!, out DateTime violationDateTime))
            {
                DateTime datetime = violationDateTime.AddDays(days);
                invoice.ViolationDateTime = datetime.ToString(ViolationDateTimeFormat);
                invoice.ViolationDate = datetime.ToString(ViolationDateFormat);
            }
        }

        private IList<Invoice> GetMockData()
        {
            IDelimitedTypeMapper<Invoice> mapper = GetTypeMapper();

            using Stream? stream = _mockDataProvider.GetDataStream();
            if (stream is null)
            {
                return Array.Empty<Invoice>();
            }

            using var reader = new StreamReader(stream);
            var options = new DelimitedOptions() { IsFirstRecordSchema = true };
            var invoices = mapper.Read(reader, options).ToList();
            return invoices;
        }

        private IDelimitedTypeMapper<Invoice> GetTypeMapper()
        {
            IDelimitedTypeMapper<Invoice> mapper = DelimitedTypeMapper.Define<Invoice>();
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
