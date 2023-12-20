using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;

namespace TrafficCourts.SourceGenerators.Diagnostics
{
    [Generator]
    public partial class TimingMetricsGenerator : IIncrementalGenerator
    {
        private const string AttributeName = "System.Diagnostics.Metrics.OperationTimerAttribute";
        private const string Attribute = """
namespace System.Diagnostics.Metrics
{
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
    public sealed class OperationTimerAttribute : System.Attribute
    {
    }
}
""";

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // Add the marker attribute to the compilation
            context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
                "OperationTimerAttribute.g.cs",
                SourceText.From(Attribute, Encoding.UTF8)));

            var provider = context.SyntaxProvider.ForAttributeWithMetadataName<ClassDeclarationSyntax>(
                AttributeName, 
                IsTargetSyntaxNode, 
                GetClassDeclarationData)
                .Where(m => m is not null);

            var compilation = context.CompilationProvider.Combine(provider.Collect());

            context.RegisterSourceOutput(compilation,
                (spc, source) => Execute(spc, source.Left, source.Right));
        }

        private static bool IsTargetSyntaxNode(SyntaxNode syntaxNode, CancellationToken cancellationToken)
        {
            return syntaxNode is ClassDeclarationSyntax { AttributeLists.Count: > 0 };
        }

        private static ClassDeclarationSyntax? GetClassDeclarationData(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
        {
            return (ClassDeclarationSyntax)context.TargetNode;
        }

        private void Execute(SourceProductionContext context, Compilation compilation, ImmutableArray<ClassDeclarationSyntax> typeList)
        {

            foreach (var type in typeList)
            {
                if (context.CancellationToken.IsCancellationRequested)
                {
                    break;
                }

                var emitter = new Emitter();

                SemanticModel semanticModel = compilation.GetSemanticModel(type.SyntaxTree);
                if (semanticModel.GetDeclaredSymbol(type) is not ITypeSymbol typeSymbol)
                {
                    continue;
                }

                emitter.EmitStartNamespace(type);
                emitter.EmitStartClass(typeSymbol);

                foreach (INamedTypeSymbol i in typeSymbol.Interfaces)
                {
                    var members = i.GetMembers()
                        .OfType<IMethodSymbol>()
                        .OrderBy(_ => _.Name)
                        .ThenBy(_ => _.Parameters.Length)
                        .ToList();

                    for (int m = 0; m < members.Count; m++)
                    {
                        ISymbol? member = members[m];
                        if (member is not IMethodSymbol methodSymbol)
                        {
                            continue;
                        }

                        emitter.EmitMethod(methodSymbol, m == members.Count - 1);
                    }
                }

                emitter.EmitEndClass();
                emitter.EmitEndNamespace();

                context.AddSource($"{typeSymbol.Name}.g.cs", SourceText.From(emitter.GetCode(), Encoding.UTF8));
            }

        }

    }
}
