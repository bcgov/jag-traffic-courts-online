using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit.Sdk;

namespace TrafficCourts.Test.Arc.Dispute.Service.Mappings
{
    /// <summary>
    /// A custom data attribute to read data from CSV formatted file. Only parses strings and integers.
    /// This method code has been taken from the following resource.
    /// Resource: https://stackoverflow.com/questions/42727394/how-to-run-xunit-test-using-data-from-a-csv-file
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class CsvDataAttribute : DataAttribute
    {
        private readonly string _fileName;
        public CsvDataAttribute(string fileName)
        {
            _fileName = fileName;
        }

        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            var pars = testMethod.GetParameters();
            var parameterTypes = pars.Select(par => par.ParameterType).ToArray();
            using (var csvFile = new StreamReader(_fileName))
            {
                csvFile.ReadLine(); // Headings Row. Comment out if not used
                string line;
                while ((line = csvFile.ReadLine()) != null)
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
}