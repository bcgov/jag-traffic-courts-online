using AutoFixture;
using AutoFixture.Kernel;
using System;

namespace TrafficCourts.Test
{
    public class DateOnlySpecimenBuilder : ISpecimenBuilder
    {
        public object? Create(object request, ISpecimenContext context)
        {
            if (request is Type type && type == typeof(DateOnly))
            {
                // Use ISpecimenContext to generate a random number for days
                int randomDays = (int)context.Create<int>();
                randomDays = randomDays % 20000 - 10000; // Limit range to -10,000 to 10,000 days

                // Generate a random DateOnly value
                return DateOnly.FromDateTime(DateTime.UtcNow.AddDays(randomDays));
            }

            return new NoSpecimen();
        }
    }
}
