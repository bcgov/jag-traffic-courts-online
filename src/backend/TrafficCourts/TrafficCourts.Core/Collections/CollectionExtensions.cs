using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace TrafficCourts.Collections;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// See: https://stackoverflow.com/questions/41244/dynamic-linq-orderby-on-ienumerablet-iqueryablet
/// </remarks>
public static class CollectionExtensions
{
    public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string property)
    {
        return ApplyOrder<T>(source, property, nameof(Queryable.OrderBy));
    }

    public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string property)
    {
        return ApplyOrder<T>(source, property, nameof(Queryable.OrderByDescending));
    }

    public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string property)
    {   
        return ApplyOrder<T>(source, property, nameof(Queryable.ThenBy));
    }

    public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string property)
    {
        return ApplyOrder<T>(source, property, nameof(Queryable.ThenByDescending));
    }

    private static IOrderedQueryable<T> ApplyOrder<T>(
        IQueryable<T> source,
        string property,
        string methodName)
    {
        // create the lambda expression
        string[] props = property.Split('.');
        Type type = typeof(T);
        ParameterExpression arg = Expression.Parameter(type, "x");
        Expression expression = arg;

        foreach (string prop in props)
        {
            // use reflection (not ComponentModel) to mirror LINQ
            PropertyInfo? pi = GetProperty(type, prop);
            if (pi is null)
            {
                throw new InvalidOperationException($"Cannot find property on expression '{property}' for type {typeof(T).FullName}. Failed on component '{prop}'.");
            }
            expression = Expression.Property(expression, pi);
            type = pi.PropertyType;
        }

        Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
        LambdaExpression lambda = Expression.Lambda(delegateType, expression, arg);

        object? result = typeof(Queryable).GetMethods().Single(
                method => method.Name == methodName
                        && method.IsGenericMethodDefinition
                        && method.GetGenericArguments().Length == 2
                        && method.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), type)
                .Invoke(null, [source, lambda]);

        Debug.Assert(result != null);

        return (IOrderedQueryable<T>)result;
    }

    /// <summary>
    /// Gets the property by name. If an exact name property name is not found, 
    /// JSON property are searched.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    private static PropertyInfo? GetProperty(Type type, string name)
    {
        // is it a property that matches by name directly?
        PropertyInfo? property = type.GetProperty(name);
        if (property is not null) return property;

        // get all the public properties, we will need to enumerate over them
        PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

        // try the json properties
        property = GetPropertyByJsonPropertyName(type, properties, name);
        if (property is not null) return property;

        // try case insensive search
        property = properties.SingleOrDefault(_ => string.Equals(_.Name, name, StringComparison.OrdinalIgnoreCase));

        return property;
    }

    /// <summary>
    /// Gets the property based on either Newtonsoft.Json or System.Text.Json property names.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="propertyName">The Json property name</param>
    /// <returns></returns>
    private static PropertyInfo? GetPropertyByJsonPropertyName(Type type, PropertyInfo[] properties, string propertyName)
    {
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
                PropertyInfo? nameProperty = attributeType.GetProperty(attributePropertyName);
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
