using System;

namespace P41.ViewBindingsGenerator
{
    /// <summary>
    /// Attribute used by the generator to generate the View.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class GenerateBindingsAttribute : Attribute
    {
        /// <summary>
        /// The xml file to generate a View from.
        /// </summary>
        public string XmlFile { get; }

        /// <summary>
        /// Initialize a new <see cref="GenerateBindingsAttribute"/> instance.
        /// </summary>
        /// <param name="xmlFile">The file to generate a View from.</param>
        public GenerateBindingsAttribute(string xmlFile)
        {
            XmlFile = xmlFile;
        }
    }
}