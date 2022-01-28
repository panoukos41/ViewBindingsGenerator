using System.Xml.Linq;
using static P41.ViewBindingsGenerator.AndroidMixins;

namespace P41.ViewBindingsGenerator;

internal class AndroidBindingsSyntaxReceiver : ISyntaxReceiver
{
    public Dictionary<ClassDeclarationSyntax, AttributeSyntax> ClassAttributePairs { get; } = new();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not ClassDeclarationSyntax classSyntax) return;
        if (classSyntax.AttributeLists.Count is 0) return;

        // todo: stop using LINQ

        var attr = classSyntax.AttributeLists
            .SelectMany(static s => s.Attributes)
            .Where(static attr => IsAndroidAttribute(attr.Name.ToString()))
            .FirstOrDefault();

        if (attr is not null)
        {
            ClassAttributePairs.Add(classSyntax, attr);
        }
    }
}

/// <summary></summary>
[Generator]
public class AndroidGenerator : ISourceGenerator
{
    private const string defaultType = "global::Android.Widget.";
    private const string xmlNamespace = "http://schemas.android.com/apk/res/android";
    private const string xmlLocalName = "id";
    private static readonly XName androidId = XName.Get(xmlLocalName, xmlNamespace);

    /// <inheritdoc/>
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(static () => new AndroidBindingsSyntaxReceiver());
        context.RegisterForPostInitialization(static c => c.AddAttributes());
    }

    /// <inheritdoc/>
    public void Execute(GeneratorExecutionContext context) => Execute(new AndroidGeneratorContext(context));

    private static void Execute(AndroidGeneratorContext context)
    {
        if (context.IsCancellationRequested) return;

        var compilation = context.Compilation;
        var files = context.Files;
        var pairs = context.ClassAttributePairs;

        foreach (var (@class, attr) in pairs)
        {
            var model = compilation.GetSemanticModel(@class.SyntaxTree);
            var filename = model.GetConstantValue(attr.ArgumentList!.Arguments[0].Expression).ToString();

            if (!files.ContainsKey(filename)) continue;

            var symbol = model.GetDeclaredSymbol(@class)!;
            var type = symbol.GetAndroidType();

            Process(context, files[filename], symbol, type);
        }
    }

    private static void Process(AndroidGeneratorContext context, AdditionalText file, INamedTypeSymbol symbol, AndroidSymbolType type)
    {
        if (type is AndroidSymbolType.None) return;

        var text = file.GetText(context.CancellationToken)?.ToString();

        if (string.IsNullOrWhiteSpace(text)) return;

        var mappings = context.Mappings;
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

                    type = $"global::{mapping}{control}";
                }

                // substring removes the '@+id/' part of the value.
                var id = element.Attribute(androidId)!.Value.Substring(5);

                return (type, id);
            });

        var writer = new ClassWriter(symbol);
        var accessor = type is AndroidSymbolType.Fragment ? "View" : "this";

        writer.Code(code =>
        {
            foreach (var (type, id) in props)
            {
                code.Line($"/// <summary>");
                code.Line($"/// Find the view with id: {id}. This will be null if the View on the activity or fragment is not set!");
                code.Line($"/// </summary>");
                code.Line($"public {type} {id} => {accessor}.FindViewById<{type}>(Resource.Id.{id});");
                code.Line();
            }
        });

        context.AddSource($"{writer.Namespace}.{writer.Class}.g.cs", writer.ToString());
    }
}
