using System.ComponentModel.DataAnnotations;

namespace TrafficCourts.Citizen.Service.Configuration;

public class FlatFileLookupServiceConfiguration
{
    /// <summary>
    /// This is the base folder path of csv data files needed for the FlatFileLookupService
    /// </summary>
    public string BasePath { get; set; } = "Data";

}
