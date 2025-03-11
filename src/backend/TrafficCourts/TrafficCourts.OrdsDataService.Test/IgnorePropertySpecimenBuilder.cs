using AutoFixture;
using AutoFixture.Kernel;
using System.Linq.Expressions;
using System.Reflection;

namespace TrafficCourts.OrdsDataService.Test;

public class IgnorePropertySpecimenBuilder<T> : ISpecimenBuilder
{
    private readonly string _propertyName;

    public IgnorePropertySpecimenBuilder(string propertyName)
    {
        _propertyName = propertyName;
    }

    public object Create(object request, ISpecimenContext context)
    {
        var pi = request as System.Reflection.PropertyInfo;
        if (pi != null && pi.DeclaringType == typeof(T) && pi.Name == _propertyName)
        {
            return new OmitSpecimen();
        }

        return new NoSpecimen();
    }
}

public class YnPropertySpecimenBuilder : ISpecimenBuilder
{
    private static readonly Random _random = new Random();

    public object Create(object request, ISpecimenContext context)
    {
        var propertyInfo = request as PropertyInfo;
        if (propertyInfo != null && propertyInfo.PropertyType == typeof(string) && propertyInfo.Name.EndsWith("yn", StringComparison.OrdinalIgnoreCase))
        {
            return _random.Next(2) == 0 ? "Y" : "N";
        }

        return new NoSpecimen();
    }
}

public class YnPropertyCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customizations.Add(new YnPropertySpecimenBuilder());
    }
}