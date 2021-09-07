using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Gov.CitizenApi.Features.Lookups
{
    public class CourtLocation
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class Language
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class Status
    {
        public int Code { get; set; }
        public string Name { get; set; }
    }

    public class Country
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class Province
    {
        public string Code { get; set; }
        public string CountryCode { get; set; }
        public string Name { get; set; }
    }

    public class Statute
    {
        public decimal Code { get; set; }
        public string Name { get; set; }
    }

    public class PoliceLocation
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class LookupsAll
    {
        public List<CourtLocation> CourtLocations { get; set; }
        public List<Language> Languages { get; set; }
        public List<Status> Statuses { get; set; }
        public List<Country> Countries { get; set; }
        public List<Province> Provinces { get; set; }
        public List<Statute> Statutes { get; set; }
        public List<PoliceLocation> PoliceLocations { get; set; }
    }
}
