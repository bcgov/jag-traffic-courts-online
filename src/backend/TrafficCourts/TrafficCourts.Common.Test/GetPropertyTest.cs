using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;
using Xunit;

namespace TrafficCourts.Common.Test;

public class GetPropertyInfoTest
{
    [Fact]
    public void can_property_info_from_expression()
    {
        Expression<Func<Person, string>> expression = f => f.Name;

        PropertyInfo propertyInfo = Extensions.GetPropertyInfo(expression);
        Assert.NotNull(propertyInfo);

        Assert.Equal(nameof(Person.Name), propertyInfo.Name);
    }

    public record Person(Guid Id, string Name, int Age);
}