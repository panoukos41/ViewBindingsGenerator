using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace P41.ViewBindingsGenerator;

#if DEBUG
/// <summary>
/// Class to help with debugging. Use to log messages that will later be flushed in a logs.g.cs file.
/// </summary>
/// <remarks>
/// This class should not be referneced when a feature is finished!
/// </remarks>
internal static class Log
{
    public static List<string> Logs { get; } = new();

    public static void Print(string msg) => Logs.Add("//\t" + msg);

    public static void Print(int num) => Logs.Add("//\t" + num);

    public static void Print(IEnumerable<string> msgs) => Logs.AddRange(msgs.Select(msg => "//\t" + msg));

    public static void Print(Exception ex) => Logs.Add("//\t" + ex.Message);

    public static void FlushLogs(GeneratorExecutionContext context)
    {
        context.AddSource($"logs.g.cs", SourceText.From(string.Join("\n", Logs), Encoding.UTF8));
    }
}
#endif
