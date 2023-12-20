using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;

namespace TrafficCourts.SourceGenerators.Diagnostics
{
    internal class Emitter : EmitterBase
    {
        public void EmitStartNamespace(ClassDeclarationSyntax classDeclaration)
        {
            OutLn($"namespace TrafficCourts.Common.OpenAPIs.OracleDataAPI.v1_0");
            OutOpenBrace();
        }

        public void EmitEndNamespace()
        {
            OutCloseBrace();
        }
        public void EmitStartClass(ITypeSymbol typeSymbol)
        {
            OutLn($"public partial class {typeSymbol.Name}");
            OutOpenBrace();
        }

        public void EmitEndClass()
        {
            OutCloseBrace();            
        }

        public void EmitMethod(IMethodSymbol methodSymbol, bool lastMethod)
        {
            bool isTaskReturnType = $"{methodSymbol.ReturnType}" == "System.Threading.Tasks.Task";
            var parameters = methodSymbol.Parameters;

            OutIndent();
            Out($"public async global::{methodSymbol.ReturnType} {methodSymbol.Name}(");
            for (int i = 0; i < parameters.Length; i++)
            {
                IParameterSymbol parameter = parameters[i];
                if (i != 0) Out(", ");
                Out($"{parameter.Type} {parameter.Name}");
            }

            OutPP(")");
            OutOpenBrace();
            OutLn("var operation = _metrics.BeginOperation();");
            OutLn();
            OutLn("try");
            OutOpenBrace();
            OutIndent();
            if (!isTaskReturnType) Out("var result = ");
            Out($"await _inner.{methodSymbol.Name}(");
            for (int i = 0; i < parameters.Length; i++)
            {
                IParameterSymbol parameter = parameters[i];
                if (i != 0) Out(", ");
                Out($"{parameter.Name}");
            }
            OutPP(").ConfigureAwait(false);");
            OutIndent();
            Out("return");
            if (!isTaskReturnType) Out(" result");
            OutPP(";");
            OutCloseBrace();
            OutLn("catch (global::System.Exception exception)");
            OutOpenBrace();
            OutLn("operation.Error(exception);");
            OutLn("throw;");
            OutCloseBrace();


            OutCloseBrace();
            if (!lastMethod) OutLn();
        }

        public string GetCode() => Capture();

    }

}
