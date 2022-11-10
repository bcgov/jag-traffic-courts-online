using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using TrafficCourts.Common.Converters;

namespace Microsoft.Extensions.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class Extensions
{
    /// <summary>
    /// Adds <see cref="DateOnly"/> and <see cref="TimeOnly"/> serializers to System.Text.Json.
    /// </summary>
    public static JsonOptions UseDateOnlyTimeOnlyStringConverters(this JsonOptions options)
    {
        options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
        options.JsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter());
        return options;
    }

    /// <summary>
    /// Adds <see cref="TypeConverter"/> to <see cref="DateOnly"/> and <see cref="TimeOnly"/> type definitions.
    /// </summary>
    /// <param name="options">Not currently used.</param>
    public static MvcOptions UseDateOnlyTimeOnlyStringConverters(this MvcOptions options)
    {
        TypeDescriptor.AddAttributes(typeof(DateOnly), new TypeConverterAttribute(typeof(DateOnlyTypeConverter)));
        TypeDescriptor.AddAttributes(typeof(TimeOnly), new TypeConverterAttribute(typeof(TimeOnlyTypeConverter)));
        return options;
    }

    /// <summary>
    /// Maps <see cref="DateOnly"/> and <see cref="TimeOnly"/> to string.
    /// </summary>
    public static void UseDateOnlyTimeOnlyStringConverters(this SwaggerGenOptions options)
    {
        options.MapType<DateOnly>(() => new OpenApiSchema
        {
            Type = "string",
            Format = "date"
        });
        options.MapType<TimeOnly>(() => new OpenApiSchema
        {
            Type = "string",
            Format = "time",
            Example = OpenApiAnyFactory.CreateFromJson("\"13:45:42.0000000\"")
        });
    }

}
