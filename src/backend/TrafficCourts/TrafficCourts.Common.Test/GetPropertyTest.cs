using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
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

    public record Person(Guid Id, [Newtonsoft.Json.JsonProperty("name")] string Name, int Age);

    public class Model
    {
        [Newtonsoft.Json.JsonProperty("name")]
        public string Name { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("age")]
        public string Age { get; set; }
    }


    [Fact]
    public void can_property_info_from_json_property()
    {
        Type type = typeof(Model);

        PropertyInfo? namePropertyInfo = GetPropertyByJsonPropertyName(type, "name"); 
        Assert.NotNull(namePropertyInfo);

        PropertyInfo? agePropertyInfo = GetPropertyByJsonPropertyName(type, "age");
        Assert.NotNull(agePropertyInfo);
    }

    private static PropertyInfo? GetPropertyByJsonPropertyName(Type type, string propertyName)
    {
        var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        foreach (var property in properties)
        {
            var attributes = property.GetCustomAttributes();

            var p = GetPropertyByJsonPropertyAttribute(property, propertyName, attributes, "Newtonsoft.Json.JsonPropertyAttribute", "PropertyName");
            p ??= GetPropertyByJsonPropertyAttribute(property, propertyName, attributes, "System.Text.Json.Serialization.JsonPropertyNameAttribute", "Name");

            if (p is not null)
            {
                return p;
            }
        }

        return null;
    }

    private static PropertyInfo? GetPropertyByJsonPropertyAttribute(PropertyInfo property, string propertyName, IEnumerable<Attribute> attributes, string attributeName, string attributePropertyName)
    {
        foreach (var attribute in attributes)
        {
            Type attributeType = attribute.GetType();
            if (attributeType.FullName == attributeName)
            {
                var nameProperty = attributeType.GetProperty(attributePropertyName);
                string? name = nameProperty?.GetValue(attribute, null) as string;

                if (name == propertyName)
                {
                    return property;
                }
            }
        }

        return null;
    }
}