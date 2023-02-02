using AutoFixture;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using TrafficCourts.Common.Models;
using Xunit;

namespace TrafficCourts.Common.Test.Models
{
    public class StatuteTest
    {
        [Fact]
        public void can_deserialize_into_array_of_records()
        {
            Fixture fixture = new Fixture();
            List<Statute> expected = fixture.CreateMany<Statute>().ToList();

            string json = JsonSerializer.Serialize(expected);

            List<Statute>? actual = JsonSerializer.Deserialize<List<Statute>>(json);

            Assert.Equal(expected, actual);
        }
    }
}
