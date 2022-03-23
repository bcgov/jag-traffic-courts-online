using Xunit;
using System;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using System.Reflection;
using System.Text.Json;

namespace TrafficCourts.Test.Citizen.Service.Features.Tickets;

public static class FormRecognizerHelper
{
    public static AnalyzeResult CreateEmptyAnalyzeResult()
    {
        return CreateAnalyzeResult("{ \"modelId\": \"1\", \"content\": \"\" }");
    }

    public static AnalyzeResult CreateAnalyzeResult(string json) => Deserialize<AnalyzeResult>(json);

    public static AnalyzedDocument CreateAnalyzedDocument(string json) => Deserialize<AnalyzedDocument>(json);

    private static T Deserialize<T>(string json) where T: class
    {
        // bit of hackery to create a internal types
        var method = GetDeserializeMethod<T>();
        object[] parameters = new object[] { JsonDocument.Parse(json).RootElement };
        T? value = method.Invoke(null, parameters) as T;
        Assert.NotNull(value);
        return value!;
    }

    private static MethodInfo GetDeserializeMethod<T>()
    {
        var bindingFlags = BindingFlags.Static | BindingFlags.NonPublic;
        var parameterTypes = new Type[] { typeof(JsonElement) };

        Type type = typeof(T);
        string name = "Deserialize" + type.Name; // by convention, the internal static methods have signature  {Type} Deserialize{Type}(JsonElement json)
        MethodInfo? method = type.GetMethod(name, bindingFlags, parameterTypes);
        Assert.NotNull(method);

        return method!;
    }

}