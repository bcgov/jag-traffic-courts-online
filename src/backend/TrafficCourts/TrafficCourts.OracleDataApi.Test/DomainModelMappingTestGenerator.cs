using System.Text;
using TrafficCourts.OracleDataApi.Client.V1;
using Xunit.Abstractions;

namespace TrafficCourts.OracleDataApi.Test;

/// <summary>
/// This is not a real unit test class. It was used to generate the source code for 
/// Oracle to Domain model mapping profile and unit tests. The output of these tests
/// were added to project as source files.
/// </summary>
public class DomainModelMappingTestGenerator
{
    private readonly ITestOutputHelper _output;

    public DomainModelMappingTestGenerator(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void generate_mapper()
    {
        GenerateOracleDomainModelProfile();
    }


    [Fact]
    public void generate_round_trip_unit_tests()
    {
        GenrateRoundTripMappingTests();
    }

    /// <summary>
    /// The generated source for from this method is added to the end of RoundTripMappingTest.cs
    /// </summary>
    private void GenrateRoundTripMappingTests()
    {
        var types = typeof(TrafficCourts.OracleDataApi.Client.V1.ApiException)
            .Assembly
            .GetTypes()
            .Where(IsOracleDataApiModel)
            .OrderBy(_ => _.Name);

        _output.WriteLine("using TrafficCourts.Common.Test.OpenAPIs;");
        _output.WriteLine("using Xunit.Abstractions;");
        _output.WriteLine("");
        _output.WriteLine("namespace TrafficCourts.OracleDataApi.Test;");
        _output.WriteLine("");

        foreach (var type in types)
        {
            string name = type.Name;
            _output.WriteLine("/// <summary>");
            _output.WriteLine($"/// Tests to ensure mapping from <see cref=\"TrafficCourts.OracleDataApi.{name}\"/>");
            _output.WriteLine($"/// to <see cref=\"TrafficCourts.Domain.Models.{name}\"/> and back again create the same object");
            _output.WriteLine("/// </summary>");
            _output.WriteLine($"public class {name}_MappingTest");
            _output.WriteLine("{");
            _output.WriteLine($"    private readonly RoundTripMappingTest<TrafficCourts.OracleDataApi.Client.V1.{name}, TrafficCourts.Domain.Models.{name}> _test;");
            _output.WriteLine($"    public {name}_MappingTest(ITestOutputHelper output)");
            _output.WriteLine("    {");
            _output.WriteLine("        _test = new(output);");
            _output.WriteLine("    }");
            _output.WriteLine("");
            _output.WriteLine("    [Fact]");
            _output.WriteLine("    public void can_map_and_reverse_map()");
            _output.WriteLine("    {");
            _output.WriteLine("        _test.can_map_and_reverse_map();");
            _output.WriteLine("    }");
            _output.WriteLine("}");
            _output.WriteLine("");
        }
    }

    private void GenerateOracleDomainModelProfile()
    {
        _indent = 0;

        WriteLine("using Oracle = TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;");
        WriteLine("using DomainModel = TrafficCourts.Domain.Models;");
        WriteLine();
        WriteLine($"namespace {typeof(IOracleDataApiClient).Namespace};");
        WriteLine();
        WriteLine("public class OracleDomainModelMappingProfile : AutoMapper.Profile");
        WriteLine("{");
        Indent();
        WriteLine("public OracleDomainModelMappingProfile()");
        WriteLine("{");
        Indent();
        // generate CreateMaps
        CreateDefaultMappings();
        CreateCustomMappings();
        Outdent();
        WriteLine("}");

        WriteLine();
        // generate EnumValueConverter
        CreateEnumConverter();

        Outdent();
        WriteLine("}");
    }

    private void CreateCustomMappings()
    {
        WriteLine();
        WriteLine($"CreateMap<Models.FileMetadata, {nameof(DomainModel)}.FileMetadata>().ReverseMap();");

        WriteLine("// FileResponse does not have a default constructor");
        WriteLine($"CreateMap<{nameof(Oracle)}.FileResponse, {nameof(DomainModel)}.FileResponse>()");
        Indent();
        WriteLine($".ConstructUsing(src => new {nameof(DomainModel)}.FileResponse(src.StatusCode, src.Headers, src.Stream, null, null));");
        Outdent();

        WriteLine($"CreateMap<{nameof(DomainModel)}.FileResponse, {nameof(Oracle)}.FileResponse>()");
        Indent();
        WriteLine($".ConstructUsing(src => new {nameof(Oracle)}.FileResponse(src.StatusCode, src.Headers, src.Stream, null, null));");
        Outdent();
    }

    private const string Oracle = "TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0";
    private const string DomainModel = "TrafficCourts.Domain.Models";

    private int _indent = 0;

    private void Indent() { _indent++; }
    private void Outdent() { _indent--; }

    private void WriteLine()
    {
        _output.WriteLine(string.Empty);
    }

    private void WriteLine(string value, string? suffix = null)
    {
        StringBuilder buffer = new StringBuilder();
        for (var i = 0; i < _indent; i++)
        {
            buffer.Append("    ");
        }
        buffer.Append(value);
        if (!string.IsNullOrEmpty(suffix))
        {
            buffer.Append(suffix);
        }

        _output.WriteLine(buffer.ToString());
    }

    private void CreateEnumConverter()
    {
        WriteLine("class EnumTypeConverter : ");

        var enums = OracleDataApiEnums.ToList();

        Indent();
        WriteLine($"// {nameof(Oracle)} to Domain Model");
        foreach (var type in enums.Where(_ => !IsYesNoUnknown(_)))
        {
            WriteLine($"AutoMapper.ITypeConverter<{nameof(Oracle)}.{type.Name}, {nameof(DomainModel)}.{type.Name}>,");
        }

        WriteLine($"// {nameof(Oracle)} to Domain Model (Unknown, Yes or No)");
        foreach (var type in enums.Where(_ => IsYesNoUnknown(_)))
        {
            WriteLine($"AutoMapper.ITypeConverter<{nameof(Oracle)}.{type.Name}, {nameof(DomainModel)}.{type.Name}>,");
        }

        WriteLine($"// Domain Model to {nameof(Oracle)}");
        foreach (var type in enums.Where(_ => !IsYesNoUnknown(_)))
        {
            WriteLine($"AutoMapper.ITypeConverter<{nameof(DomainModel)}.{type.Name}, {nameof(Oracle)}.{type.Name}>,");
        }

        WriteLine($"// Domain Model to {nameof(Oracle)} (Unknown, Yes or No)");
        var last = enums.Where(_ => IsYesNoUnknown(_)).ToList();
        for (var i = 0; i < last.Count; i++)
        {
            var type = last[i];
            if (i == last.Count - 1)
            {
                WriteLine($"AutoMapper.ITypeConverter<{nameof(DomainModel)}.{type.Name}, {nameof(Oracle)}.{type.Name}>");
            }
            else
            {
                WriteLine($"AutoMapper.ITypeConverter<{nameof(DomainModel)}.{type.Name}, {nameof(Oracle)}.{type.Name}>,");
            }
        }

        Outdent();

        WriteLine("{");
        Indent();

        // generate the Oracle to Domain Model methods
        foreach (var type in enums.Where(_ => !IsYesNoUnknown(_)))
        {
            WriteConvertMethod(type, nameof(Oracle), nameof(DomainModel));
            WriteConvertMethod(type, nameof(DomainModel), nameof(Oracle));
        }

        foreach (var type in enums.Where(_ => IsYesNoUnknown(_)))
        {
            WriteConvertMethod(type, nameof(Oracle), nameof(DomainModel));
            WriteConvertMethod(type, nameof(DomainModel), nameof(Oracle));
        }

        // generate the Domain Model to Oracle methods

        Outdent();
        WriteLine("}");

    }

    private void WriteConvertMethod(Type type, string source, string destination)
    {
        var names = Enum.GetNames(type);

        WriteLine($"public {destination}.{type.Name} Convert({source}.{type.Name} source, {destination}.{type.Name} destination, AutoMapper.ResolutionContext context)");
        WriteLine("{");
        Indent();
        WriteLine("return source switch");
        WriteLine("{");
        Indent();
        foreach (var name in names)
        {
            WriteLine($"{source}.{type.Name}.{name} => {destination}.{type.Name}.{name},");
        }
        WriteLine($"_ => {destination}.{type.Name}.{names[0]}");
        Outdent();
        WriteLine("};");
        Outdent();
        WriteLine("}");
        WriteLine();
    }

    private static bool IsYesNoUnknown(Type type)
    {
        var names = Enum.GetNames(type);
        if (names.Length != 3) return false;

        return names[0] == "UNKNOWN" && names[1] == "Y" && names[2] == "N";
    }

    private void CreateDefaultMappings()
    {
        var types = typeof(Domain.Models.Dispute).Assembly
            .GetTypes();

        // each enumeration
        WriteLine("// enumerations");
        foreach (var type in types.Where(IsTargetEnum).Where(_ => !IsYesNoUnknown(_)).OrderBy(_ => _.Name))
        {
            WriteLine($"CreateMap<{nameof(Oracle)}.{type.Name}, {nameof(DomainModel)}.{type.Name}>().ConvertUsing<EnumTypeConverter>();");
        }

        foreach (var type in types.Where(IsTargetEnum).Where(_ => IsYesNoUnknown(_)).OrderBy(_ => _.Name))
        {
            WriteLine($"CreateMap<{nameof(Oracle)}.{type.Name}, {nameof(DomainModel)}.{type.Name}>().ConvertUsing<EnumTypeConverter>();");
        }

        WriteLine("// classes");
        foreach (var type in types.Where(IsTargetClass).OrderBy(_ => _.Name))
        {
            WriteLine($"CreateMap<{nameof(Oracle)}.{type.Name}, {nameof(DomainModel)}.{type.Name}>().ReverseMap();");
        }
    }

    private static IEnumerable<Type> OracleDataApiEnums => typeof(IOracleDataApiClient).Assembly
        .GetTypes()
        .Where(type => type.IsEnum && type.Namespace == typeof(IOracleDataApiClient).Namespace && type.Name != "FileResponse" && type.Name != "FileMetadata")
        .OrderBy(type => type.Name);

    private static bool IsTargetEnum(Type type) => type.IsEnum && IncludeType(type);

    private static bool IsTargetClass(Type type) => type.IsClass && IncludeType(type);

    private static bool IncludeType(Type type) => type.Name != "FileResponse" && type.Name != "FileMetadata" && type.Namespace == "TrafficCourts.Domain.Models";


    private static bool IsOracleDataApiModel(Type type) => type.IsClass 
        && !type.IsGenericType 
        && !type.Name.Contains('<') 
        && type.Name != "ApiException"
        && type.Name != "OracleDataApiClient"
        && type.Name != "OracleDataApiConfiguration"
        && type.Name != "OracleDataApiExtensions"
        && type.Name != "OracleDataApiOperationMetrics"
        && type.Name != "EnumTypeConverter"
        && type.Name != "OracleDataApiService"
        && type.Name != "OracleDomainModelMappingProfile"
        && type.Name != "TimedOracleDataApiClient"
        ;
}

