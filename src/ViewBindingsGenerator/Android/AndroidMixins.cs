using System;

namespace P41.ViewBindingsGenerator;

internal static class AndroidMixins
{
    private static readonly string AttributeNameFull = nameof(AndroidBindingAttribute);
    private static readonly string AttributeName = AttributeNameFull.Remove(AttributeNameFull.IndexOf("Attribute"));

    public static bool IsAndroidAttribute(string name) => name == AttributeName || name == AttributeNameFull;

    /// <summary>
    /// Get the AdditioanlText in in a dictionary with key the filename with the extension.
    /// </summary>
    /// <param name="additionalFiles"></param>
    /// <returns></returns>
    public static Dictionary<string, AdditionalText> GetXmlFiles(this ImmutableArray<AdditionalText> additionalFiles)
    {
        var files = new Dictionary<string, AdditionalText>();
        var count = additionalFiles.Length;

        for (int i = 0; i < count; i++)
        {
            var file = additionalFiles[i];
            var path = file.Path;

            if (!path.EndsWith(".xml")) continue;

            files.Add(Path.GetFileName(path), file);
        }
        return files;
    }

    /// <summary>
    /// Get 'NamespaceMappingAttribute' Java to Managed mappings to use for replacement in xml files.
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    public static Dictionary<string, string> GetNamespaceMappings(this Compilation compilation)
    {
        var mappings = new Dictionary<string, string>();
        var assemblies = compilation.SourceModule.ReferencedAssemblySymbols;
        var count = assemblies.Length;

        for (int i = 0; i < count; i++)
        {
            var assembly = assemblies[i];

            if (!assembly.Name.Contains("Android")) continue;

            var attrs = assembly.GetAttributes();
            for (int j = 0; j < attrs.Length; j++)
            {
                var attr = attrs[j];
                if (attr.AttributeClass is { Name: "NamespaceMappingAttribute" })
                {
                    var x = attr.ToString().Split('"');
                    var java = x[1];

                    if (mappings.ContainsKey(java)) continue;

                    var managed = x[3];
                    mappings.Add(java, managed);
                }
            }
        }
        return mappings;
    }

    public static AndroidSymbolType GetAndroidType(this INamedTypeSymbol symbol)
    {
        if (symbol.ContainsAttribute())
        {
            if (OfBaseType(symbol, static str => str.Contains("App.Activity")))
            {
                return AndroidSymbolType.Activity;
            }
            if (OfBaseType(symbol, static str => str.Contains("App.Fragment")))
            {
                return AndroidSymbolType.Fragment;
            }
            if (OfBaseType(symbol, static str => str.Contains("Views.View")))
            {
                return AndroidSymbolType.View;
            }
        }
        return AndroidSymbolType.None;

        static bool OfBaseType(INamedTypeSymbol symbol, Func<string, bool> predicate)
        {
            var baseType = symbol.BaseType;
            var typeName = baseType?.ToDisplayString();

            while (typeName is { Length: > 0 } && typeName != "object")
            {
                if (typeName.Contains("Android") && predicate(typeName)) return true;

                baseType = baseType?.BaseType;
                typeName = baseType?.ToDisplayString();
            }
            return false;
        }
    }
}
