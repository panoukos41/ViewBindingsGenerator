using Microsoft.CodeAnalysis;
using System.Linq;

namespace P41.ViewBindingsGenerator;

internal static class Extensions
{
    public static bool IsCancelled(this GeneratorExecutionContext context)
    {
        return context.CancellationToken.IsCancellationRequested;
    }

    public static bool ContainsAttribute(this INamedTypeSymbol symbol)
    {
        return symbol.GetAttributes().Any(attr => attr.AttributeClass?.ToDisplayString() == typeof(GenerateBindingsAttribute).FullName);
    }

    public static AttributeData GetAttribute(this INamedTypeSymbol symbol)
    {
        return symbol
            .GetAttributes()
            .First(attr => attr.AttributeClass?.ToDisplayString() == typeof(GenerateBindingsAttribute).FullName);
    }

    public static string GetNamespace(this INamedTypeSymbol symbol)
    {
        return symbol.ContainingNamespace.Name;
    }

    public static string GetClass(this INamedTypeSymbol symbol)
    {
        return symbol.Name;
    }
}
