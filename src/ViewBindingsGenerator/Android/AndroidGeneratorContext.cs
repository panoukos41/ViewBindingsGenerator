using Microsoft.CodeAnalysis.Text;
using System.Threading;

namespace P41.ViewBindingsGenerator;

internal readonly struct AndroidGeneratorContext
{
    private readonly GeneratorExecutionContext? _sourceProductionContext;

    public AndroidGeneratorContext(GeneratorExecutionContext sourceProductionContext)
    {
        _sourceProductionContext = sourceProductionContext;
        _compilationAnalysisContext = null;
        var syntaxReceiver = (AndroidBindingsSyntaxReceiver)sourceProductionContext.SyntaxReceiver!;

        Compilation = sourceProductionContext.Compilation;
        ClassAttributePairs = syntaxReceiver.ClassAttributePairs;
        Mappings = Compilation.GetNamespaceMappings();
        Files = sourceProductionContext.AdditionalFiles.GetXmlFiles();
    }

    private readonly CompilationAnalysisContext? _compilationAnalysisContext;

    public Compilation Compilation { get; }

    public Dictionary<string, AdditionalText> Files { get; }

    public Dictionary<ClassDeclarationSyntax, AttributeSyntax> ClassAttributePairs { get; }

    public Dictionary<string, string> Mappings { get; }

    public CancellationToken CancellationToken => _sourceProductionContext?.CancellationToken ?? default;

    public bool IsCancellationRequested => _sourceProductionContext?.CancellationToken.IsCancellationRequested ?? false;

    public void AddSource(string nameHint, string source)
    {
        _sourceProductionContext?.AddSource(nameHint, source);
    }

    public void ReportDiagnostic(Diagnostic diagnostic)
    {
        _sourceProductionContext?.ReportDiagnostic(diagnostic);
        _compilationAnalysisContext?.ReportDiagnostic(diagnostic);
    }
}
