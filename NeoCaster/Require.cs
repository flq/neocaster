using System;

namespace NeoCaster
{
    /// <summary>
    /// Pre / Postcondition-checker
    /// </summary>
    internal static class Require
    {
        public static void NotNull(object obj, string parameterName)
        {
            if (obj == null)
                throw new ArgumentNullException(parameterName);
        }
    }
}