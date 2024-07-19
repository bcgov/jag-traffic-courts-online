using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit.Sdk;

namespace TrafficCourts.Test.Arc.Dispute.Service.Mappings;

/// <summary>
/// A custom data attribute to read data from CSV formatted file that is embedded as a resource. Only parses strings and integers.
/// This method code has been taken from the following resource.
/// Resource: https://stackoverflow.com/questions/42727394/how-to-run-xunit-test-using-data-from-a-csv-file
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class CsvEmbeddedResourceDataAttribute : DataAttribute
{
    private readonly string _resourceName;
    private readonly bool _hasHeadings;

    public CsvEmbeddedResourceDataAttribute(string resourceName, bool hasHeadings)
    {
        _resourceName = resourceName;
        _hasHeadings = hasHeadings;
    }

    public CsvEmbeddedResourceDataAttribute(string resourceName)
        : this(resourceName, true)
    {
    }

    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        Type[] parameterTypes = testMethod
            .GetParameters()
            .Select(parameterInfo => parameterInfo.ParameterType)
            .ToArray();

        var assembly = testMethod.ReflectedType?.Assembly;

        using (Stream stream = assembly.GetManifestResourceStream(_resourceName))
        using (StreamReader reader = new StreamReader(stream))
        {
            if (_hasHeadings)
            {
                reader.ReadLine(); // skip headers
            }
            
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                var row = line.Split(',');
                yield return ConvertParameters((object[])row, parameterTypes);
            }

        }
    }

    private static object[] ConvertParameters(IReadOnlyList<object> values, IReadOnlyList<Type> parameterTypes)
    {
        var result = new object[parameterTypes.Count];
        for (var idx = 0; idx < parameterTypes.Count; idx++)
        {
            result[idx] = ConvertParameter(values[idx], parameterTypes[idx]);
        }

        return result;
    }

    private static object ConvertParameter(object parameter, Type parameterType)
    {
        return parameterType == typeof(int) ? Convert.ToInt32(parameter) : parameter;
    }
}
