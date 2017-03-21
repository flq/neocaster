using System;

namespace NeoCaster
{
    /// <summary>
    /// A few things you may want to change
    /// </summary>
    public class Config
    {
        /// <summary>
        /// This function is used to get a result of how property names of .NET types
        /// would/should end up in Neo4J. By default, the names get "camelized", i.e.
        /// "FirstName" becomes "firstName".
        /// </summary>
        public static Func<string, string> DotnetToNeoPropNames =
            propName => propName.Substring(0, 1).ToLowerInvariant() + propName.Substring(1);

    }
}