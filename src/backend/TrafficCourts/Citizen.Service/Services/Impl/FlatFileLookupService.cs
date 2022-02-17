using CsvHelper;
using CsvHelper.Configuration;
using TrafficCourts.Citizen.Service.Models.Tickets;
using System.Globalization;

namespace TrafficCourts.Citizen.Service.Services.Impl;

/// <summary>
/// This is an initial simple implementation of the ILookupService that pulls data from a flat file (csv).
/// </summary>
public class FlatFileLookupService : ILookupService
{
    private readonly string _basePath;
    private readonly ILogger<FlatFileLookupService> _logger;

    public FlatFileLookupService(string basePath, ILogger<FlatFileLookupService> logger)
    {
        _basePath = basePath ?? throw new ArgumentNullException(nameof(basePath));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IEnumerable<Statute> GetStatutes()
    {
        _logger.LogDebug("Retrieving Statute lookup list");

        var csvConfig = new CsvConfiguration(CultureInfo.CurrentCulture)
        {
            HasHeaderRecord = true,
            HeaderValidated = null,
            MissingFieldFound = null,
            BadDataFound = null
        };
        var streamReader = File.OpenText(_basePath + "/Statutes.csv");
        var csvReader = new CsvReader(streamReader, csvConfig);
        return csvReader.GetRecords<Statute>();
    }
}
