using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using static P41.ViewBindingsGenerator.AndroidBindingsMixins;

namespace P41.ViewBindingsGenerator;

internal class GenerateBindingsSyntaxReceiver : ISyntaxReceiver
{
    public List<ClassDeclarationSyntax> Views { get; } = new List<ClassDeclarationSyntax>();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not ClassDeclarationSyntax classSyntax) return;

        var attrs = classSyntax
            .DescendantNodes()
            .OfType<AttributeSyntax>()
            .Any(attr =>
            {
                var name = attr.Name.ToString();

                return AttributeNames.Contains(name);
            });

        if (attrs) Views.Add(classSyntax);
    }
}

[Generator]
internal class AndroidBindingsGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new GenerateBindingsSyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.IsCancelled()) return;

        //todo: Should start emmiting warnings etc.

        var compilation = context.Compilation;
        var files = context.AdditionalFiles.GetXmlFiles();
        var views = ((GenerateBindingsSyntaxReceiver)context.SyntaxReceiver!).Views;
        var mappings = compilation.GetNamespaceMappings();

        for (int i = 0; i < views.Count; i++)
        {
            if (context.IsCancelled()) return;
            var view = views[i];
            var model = compilation.GetSemanticModel(view.SyntaxTree);
            var fileName = GetFileName(model, view);

            if (!files.ContainsKey(fileName)) continue;

            var symbol = model.GetDeclaredSymbol(view)!;
            var symbolType = GetSymbolType(symbol);

            if (symbolType == SymbolType.None) continue;

            var @namespace = symbol.GetNamespace();
            var @class = symbol.GetClass();

            ProcessFile(files[fileName], context, @namespace, @class, symbolType is SymbolType.Fragment, mappings);
        }
    }

    private static string GetFileName(SemanticModel model, ClassDeclarationSyntax view)
    {
        var attribute = view.AttributeLists
            .SelectMany(x => x.Attributes)
            .First(x => AttributeNames.Contains(x.Name.ToString()));

        var fileArg = attribute.ArgumentList!.Arguments[0];
        var fileExp = fileArg.Expression;

        return model.GetConstantValue(fileExp).ToString();
    }

    private enum SymbolType
    {
        None = 0,
        Activity = 1,
        Fragment = 2,
        View = 3
    }

    private static SymbolType GetSymbolType(INamedTypeSymbol symbol)
    {
        if (symbol.ContainsAttribute())
        {
            if (symbol.IsActivity()) return SymbolType.Activity;
            if (symbol.IsFragment()) return SymbolType.Fragment;
            if (symbol.IsView()) return SymbolType.View;
        }
        return SymbolType.None;
    }

    const string defaultType = "global::Android.Widget.";
    const string xmlNamespace = "http://schemas.android.com/apk/res/android";
    const string xmlLocalName = "id";
    static readonly XName androidId = XName.Get(xmlLocalName, xmlNamespace);

    private static void ProcessFile(AdditionalText file, GeneratorExecutionContext context,
        string @namespace, string @class, bool isFragment, Dictionary<string, string> mappings)
    {
        var text = file.GetText(context.CancellationToken)?.ToString();

        if (string.IsNullOrWhiteSpace(text)) return;

        var doc = XDocument.Parse(text);

        var props = doc
            .Descendants()
            .Where(x => x.Attributes(androidId).Any())
            .Select(element =>
            {
                var type = element.Name.ToString();

                if (!type.Contains('.'))
                {
                    type = defaultType + type;
                }
                else
                {
                    var lastFullstop = type.LastIndexOf('.');
                    var mapping = mappings[type.Substring(0, lastFullstop)];
                    var control = type.Substring(lastFullstop);

                    type = "global::" + mapping + control;
                }

                // substring removes the '@+id/' part of the value.
                var id = element.Attribute(androidId)!.Value.Substring(5);

                return (type, id);
            })
            .ToArray();

        var sb = new StringBuilder().Render(@namespace, @class, props, isFragment);

        context.AddSource($"{@class}.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
    }
}
