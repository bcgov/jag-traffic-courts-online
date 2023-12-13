using Microsoft.Extensions.Caching.Memory;
using TrafficCourts.Citizen.Service.Services.Tickets.Search.Common;

namespace TrafficCourts.Citizen.Service.Services.Tickets.Search.Mock;

public class MockInvoiceCache
{
    private const string cacheKey = "mock-rsi-invoices";
    private readonly IMemoryCache _cache;
    private readonly object SyncLock = new object();

    public MockInvoiceCache(IMemoryCache cache)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public void Add(Invoice invoice)
    {
        ArgumentNullException.ThrowIfNull(invoice);

        lock(SyncLock)
        {
            List<Invoice> invoices = _cache.Get<List<Invoice>>(cacheKey) ?? [];

            // remove any matching invoice by invoice number (ticket number + count)
            invoices.RemoveAll(_ => _.InvoiceNumber == invoice.InvoiceNumber);
            invoices.Add(invoice);

            _cache.Set(cacheKey, invoices);
        }
    }

    public List<Invoice> GetAll()
    {
        lock (SyncLock)
        {
            List<Invoice> invoices = _cache.Get<List<Invoice>>(cacheKey) ?? [];
            return invoices;
        }
    }
}
