using AutoFixture;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using TrafficCourts.Common.Models;
using Xunit;

namespace TrafficCourts.Common.Test.Models;

public class LanguageTest
{
    [Fact]
    public void can_deserialize_into_array_of_records()
    {
        Fixture fixture = new();
        List<Language> expected = fixture.CreateMany<Language>().ToList();

        string json = JsonSerializer.Serialize(expected);

        List<Language>? actual = JsonSerializer.Deserialize<List<Language>>(json);

        Assert.Equal(expected, actual);
    }
}
