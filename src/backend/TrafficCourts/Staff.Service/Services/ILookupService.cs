﻿using TrafficCourts.Staff.Service.Models;

namespace TrafficCourts.Staff.Service.Services;

public interface ILookupService
{
    /// <summary> 
    /// Returns a list of Violation Ticket Statutes filtered by given section text (if provided), case insensitive.
    /// </summary>
    /// <param name="section">Motor vehicle act Section text to query by, ie "13(1)(a)" for "Motor Vehicle or Trailer without Licence" contravention, or blank for no filter.</param>
    /// <returns></returns>
    public IEnumerable<Statute> GetStatutes(string? section);
}
