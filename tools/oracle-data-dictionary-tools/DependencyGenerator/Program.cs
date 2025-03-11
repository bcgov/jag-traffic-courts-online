using CommandLine;
using CommandLine.Text;
using Microsoft.EntityFrameworkCore;
using Oracle.DataDictionary;
using Oracle.ManagedDataAccess.Client;
using System.Text.RegularExpressions;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var parserResult = Parser.Default.ParseArguments<Options>(args);

        if (parserResult.Errors.Any())
        {
            await DisplayHelp(parserResult);
            return;
        }

        await parserResult.WithParsedAsync(RunAsync);
    }

    private static async Task RunAsync(Options options)
    {
        string owner = options.Username.ToUpper();

        OracleDataDictionaryDbContext context = CreateDbContext(options);

        IList<OracleObject> objects = GetOracleObjectsInDependencyOrder(context, owner);

        foreach (var obj in objects)
        {
            string filename = string.Empty;

            var source = context.Sources
                .Where(_ => _.Owner == obj.Owner && _.Name == obj.Name && _.Type == obj.ObjectType && _.Text != null && _.Text.Contains("$HeadURL"))
                .FirstOrDefault();

            if (source is not null)
            {
                filename = ExtractFileNameFromUrl(source.Text!);
            }

            if (string.IsNullOrEmpty(filename))
            {
                filename = SearchFileInPath(options.Path, obj);
            }

            Console.WriteLine(filename);

        }
    }

    private static string SearchFileInPath(string path, OracleObject obj)
    {
        string searchText = obj.ObjectType switch
        {
            "PACKAGE" => $"create or replace package {obj.Name.ToLower()}",
            "PACKAGE BODY" => $"create or replace package body {obj.Name.ToLower()}",
            "PROCEDURE" => $"create or replace procedure {obj.Name.ToLower()}",
            "FUNCTION" => $"create or replace function {obj.Name.ToLower()}",
            "TRIGGER" => $"create or replace trigger {obj.Name.ToLower()}"
        };

        string altSearchText = obj.ObjectType switch
        {
            "PACKAGE" => $"create or replace package {obj.Owner}.{obj.Name.ToLower()}",
            "PACKAGE BODY" => $"create or replace package body {obj.Owner}.{obj.Name.ToLower()}",
            "PROCEDURE" => $"create or replace procedure {obj.Owner}.{obj.Name.ToLower()}",
            "FUNCTION" => $"create or replace function {obj.Owner}.{obj.Name.ToLower()}",
            "TRIGGER" => $"create or replace trigger {obj.Owner}.{obj.Name.ToLower()}"
        };

        var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            var lines = File.ReadAllLines(file);
            foreach (var line in lines)
            {
                if (line.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    line.Contains(altSearchText, StringComparison.OrdinalIgnoreCase))
                {
                    return Path.GetFileName(file);
                }
            }
        }

        return obj.ObjectType switch
        {
            "PACKAGE" => $"{obj.Name}.sql -- **",
            "PACKAGE BODY" => $"{obj.Name}_body.sql -- **",
            "PROCEDURE" => $"{obj.Name}.sql -- **",
            "FUNCTION" => $"{obj.Name}.sql -- **",
            "TRIGGER" => $"{obj.Name}.sql -- **",
            _ => $"{obj.Name}.{obj.ObjectType.ToLower()} -- **",
        };
    }

    public static string ExtractFileNameFromUrl(string input)
    {
        // Define the regular expression to match the URL and capture the file name
        string pattern = @"https?://[^/]+(?:/[^/]+)*/([^/]+) \$";
        var match = Regex.Match(input, pattern);

        // Check if the match is successful and return the captured file name
        if (match.Success && match.Groups.Count > 1)
        {
            return match.Groups[1].Value;
        }

        // Return an empty string if no match is found
        return string.Empty;
    }

    private static IList<OracleObject> GetOracleObjectsInDependencyOrder(OracleDataDictionaryDbContext context, string owner)
    {
        var order = new List<OracleObject>();

        string[] types = new string[] { "PACKAGE", "PACKAGE BODY", "PROCEDURE", "FUNCTION", "TRIGGER" };

        var typeOrder = new Dictionary<string, int>
    {
        { "TRIGGER", 5 },
        { "PACKAGE", 1 },
        { "PACKAGE BODY", 2 },
        { "FUNCTION", 3 },
        { "PROCEDURE", 4 }
    };

        List<OracleObject> objects = context.Objects
            .Where(o => o.Owner == owner && types.Contains(o.ObjectType))
            .ToList()
            .OrderBy(o => typeOrder[o.ObjectType])
            .ThenBy(o => o.Name)
            .ToList();

        List<Dependency> dependencies = context.Dependencies
            .Where(dependency =>
                    dependency.Owner == owner &&
                    dependency.ReferencedOwner == owner &&
                    types.Contains(dependency.ReferencedType))
            .ToList();

        while (objects.Count != 0)
        {
            //
            var objectToRemove = new List<OracleObject>();
            
            // package body always depends on the package, so skip those
            foreach (var o in objects.Where(o => o.ObjectType != "PACKAGE BODY"))
            {
                // if the object has dependencies, skip it
                if (dependencies.Any(d => d.Name == o.Name && d.Type == o.ObjectType))
                {
                    continue;
                }

                order.Add(o);
                objectToRemove.Add(o);

                if (o.ObjectType == "PACKAGE")
                {
                    var body = objects.Single(_ => _.Name == o.Name && _.ObjectType == "PACKAGE BODY");
                    order.Add(body);
                    objectToRemove.Add(body);
                }
            }

            foreach (var o in objectToRemove)
            {
                objects.Remove(o);
            }

            // remove the dependencies on this object
            dependencies.RemoveAll(d => objectToRemove.Any(o => o.Name == d.ReferencedName && o.ObjectType == d.ReferencedType));
        }


        return order;
    }

    private static Task DisplayHelp<T>(ParserResult<T> result)
    {
        var helpText = HelpText.AutoBuild(result, h =>
        {
            h.AdditionalNewLineAfterOption = false;
            h.Copyright = string.Empty;
            return HelpText.DefaultParsingErrorsHandler(result, h);
        }, e => e);

        return Task.CompletedTask;
    }

    private static OracleDataDictionaryDbContext CreateDbContext(Options options)
    {
        OracleConfiguration.OracleDataSources.Add(options.Sid, $"(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={options.Host})(PORT=1521))(CONNECT_DATA=(SID={options.Sid})(SERVER=dedicated)))");

        var optionsBuilder = new DbContextOptionsBuilder<OracleDataDictionaryDbContext>();
        optionsBuilder.UseOracle($"Data Source={options.Sid};User ID={options.Username};Password={options.Password};");

        var dbContext = new OracleDataDictionaryDbContext(optionsBuilder.Options);
        return dbContext;
    }
}