global using Microsoft.CodeAnalysis;
global using Microsoft.CodeAnalysis.CSharp;
global using Microsoft.CodeAnalysis.CSharp.Syntax;
global using Microsoft.CodeAnalysis.Diagnostics;
global using System.Buffers;
global using System.Collections.Generic;
global using System.Collections.Immutable;
global using System.Diagnostics;
global using System.IO;
global using System.Linq;
global using System.Reflection;
global using System.Text;

namespace System.Collections.Generic
{
    internal static class CollectionExtensions
    {
        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> pair, out TKey key, out TValue value)
        {
            key = pair.Key;
            value = pair.Value;
        }
    }
}

namespace Microsoft.CodeAnalysis
{
    internal static class GeneratorPostInitializationContextExtensions
    {
        public static void AddAttributes(this GeneratorPostInitializationContext context)
        {
            using var manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("P41.ViewBindingsGenerator.Attributes.cs");
            using var reader = new StreamReader(manifestResourceStream);
            context.AddSource("Attributes.g.cs", reader.ReadToEnd());
        }
    }
}

namespace P41.ViewBindingsGenerator
{
    internal static class Extensions
    {
        public static bool IsCancelled(this GeneratorExecutionContext context)
        {
            return context.CancellationToken.IsCancellationRequested;
        }

        public static bool ContainsAttribute(this INamedTypeSymbol symbol)
        {
            return symbol.GetAttributes().Any(attr => attr.AttributeClass?.ToDisplayString() == typeof(AndroidBindingAttribute).FullName);
        }

        public static AttributeData GetAttribute(this INamedTypeSymbol symbol)
        {
            return symbol
                .GetAttributes()
                .First(attr => attr.AttributeClass?.ToDisplayString() == typeof(AndroidBindingAttribute).FullName);
        }

        public static string GetNamespace(this INamedTypeSymbol symbol)
        {
            return string.Join(".", symbol.ContainingNamespace.ConstituentNamespaces);
        }

        public static string GetAccessibility(this INamedTypeSymbol symbol)
        {
            return symbol.DeclaredAccessibility switch
            {
                Accessibility.NotApplicable => "internal",
                Accessibility.Private => "private",
                Accessibility.ProtectedAndInternal => "internal protected",
                Accessibility.Protected => "protected",
                Accessibility.Internal => "internal",
                Accessibility.ProtectedOrInternal => "internal",
                Accessibility.Public => "public",
                _ => "internal"
            };
        }

        public static string GetClass(this INamedTypeSymbol symbol)
        {
            return symbol.Name;
        }
    }
}
