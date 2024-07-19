using FlatFiles.TypeMapping;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using TrafficCourts.TicketSearch.Common;

namespace TrafficCourts.TicketSearch.Mock
{
    public class MockTicketSearchService : ITicketInvoiceSearchService
    {
        private readonly IMockDataProvider _mockDataProvider;
        private readonly ILogger<MockTicketSearchService> _logger;
        private readonly TimeProvider _clock;
        private readonly MockInvoiceCache _cache;

        public MockTicketSearchService(IMockDataProvider mockDataProvider, ILogger<MockTicketSearchService> logger, TimeProvider clock, IMemoryCache cache)
        {
            ArgumentNullException.ThrowIfNull(cache);

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _mockDataProvider = mockDataProvider ?? throw new ArgumentNullException(nameof(mockDataProvider));

            _cache = new MockInvoiceCache(cache);
        }

        public Task<IList<Invoice>> SearchAsync(string ticketNumber, TimeOnly issuedTime, CancellationToken cancellationToken)
        {
            using var activity = TrafficCourts.TicketSearch.Instrumentation.Diagnostics.Source.StartActivity("mock ticket search");

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
            List<Invoice> invoices = _cache.GetAll();

            IDelimitedTypeMapper<Invoice> mapper = GetTypeMapper();

            using Stream? stream = _mockDataProvider.GetDataStream();
            if (stream is null)
            {
                return invoices;
            }

            using var reader = new StreamReader(stream);
            var options = new FlatFiles.DelimitedOptions() { IsFirstRecordSchema = true };
            var data = mapper.Read(reader, options);

            // make sure there are no duplicate in invoice numbers
            foreach (var invoice in data)
            {
                if (!invoices.Any(_ => _.InvoiceNumber == invoice.InvoiceNumber))
                {
                    invoices.Add(invoice);
                }
            }

            return invoices;
        }

        private IDelimitedTypeMapper<Invoice> GetTypeMapper()
        {
            IDelimitedTypeMapper<Invoice> mapper = DelimitedTypeMapper.Define<Invoice>();

            // map all the property to column matching the JsonPropertyNameAttribute on them
            mapper.JsonProperty(_ => _.InvoiceNumber);
            mapper.JsonProperty(_ => _.PbcRefNumber);
            mapper.JsonProperty(_ => _.PartyNumber);
            mapper.JsonProperty(_ => _.PartyName);
            mapper.JsonProperty(_ => _.PartySurname);
            mapper.JsonProperty(_ => _.PartyFirstGivenName);
            mapper.JsonProperty(_ => _.PartySecondGivenName);
            mapper.JsonProperty(_ => _.AccountNumber);
            mapper.JsonProperty(_ => _.SiteNumber);
            mapper.JsonProperty(_ => _.InvoiceType);
            mapper.JsonProperty(_ => _.ViolationDateTime);
            mapper.JsonProperty(_ => _.TicketedAmount);
            mapper.JsonProperty(_ => _.AmountDue);
            mapper.JsonProperty(_ => _.OffenceDescription);
            mapper.JsonProperty(_ => _.VehicleDescription);
            mapper.JsonProperty(_ => _.ViolationDate);
            mapper.JsonProperty(_ => _.DiscountAmount);
            mapper.JsonProperty(_ => _.FormNumber);
            mapper.JsonProperty(_ => _.Act);
            mapper.JsonProperty(_ => _.Section);

            return mapper;
        }
    }
}

public static class Extensions
{
    /// <summary>
    /// Maps the property to the associated JsonPropertyNameAttribute property name.
    /// </summary>
    public static void JsonProperty<TEntity>(this IDelimitedTypeMapper<TEntity> mapper, Expression<Func<TEntity, string?>> accessor)
    {
        string columnName = GetColumnName(accessor);
        mapper.Property(accessor).ColumnName(columnName);
    }

    /// <summary>
    /// Maps the property to the associated JsonPropertyNameAttribute property name.
    /// </summary>
    public static void JsonProperty<TEntity>(this IDelimitedTypeMapper<TEntity> mapper, Expression<Func<TEntity, decimal>> accessor)
    {
        string columnName = GetColumnName(accessor);
        mapper.Property(accessor).ColumnName(columnName);
    }

    private static string GetColumnName<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> accessor)
    {
        var memberExpression = (MemberExpression)accessor.Body;
        var jsonPropertyName = memberExpression.Member.GetCustomAttribute<System.Text.Json.Serialization.JsonPropertyNameAttribute>(false);

        string columnName = jsonPropertyName?.Name ?? memberExpression.Member.Name;
        return columnName;
    }
}
