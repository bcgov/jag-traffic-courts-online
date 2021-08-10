using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Gov.CitizenApi.Features.Lookups
{
    public class CourtLocation
    {
        public string code { get; set; }
        public string name { get; set; }
    }

    public class Language
    {
        public string code { get; set; }
        public string name { get; set; }
    }

    public class Status
    {
        public int code { get; set; }
        public string name { get; set; }
    }

    public class Country
    {
        public string code { get; set; }
        public string name { get; set; }
    }

    public class Province
    {
        public string code { get; set; }
        public string countryCode { get; set; }
        public string name { get; set; }
    }

    public class Statute
    {
        public int code { get; set; }
        public string name { get; set; }
    }

    public class PoliceLocation
    {
        public string code { get; set; }
        public string name { get; set; }
    }

    public class Lookups
    {
        public List<CourtLocation> courtLocations { get; set; }
        public List<Language> languages { get; set; }
        public List<Status> statuses { get; set; }
        public List<Country> countries { get; set; }
        public List<Province> provinces { get; set; }
        public List<Statute> statutes { get; set; }
        public List<PoliceLocation> policeLocations { get; set; }
    }
}
