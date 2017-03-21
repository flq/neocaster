using System;

namespace NeoCaster
{
    public static class Utilities
    {
        public static string ToBase64Guid(this Guid guid)
        {
            // The replacements make the string url/uri/path friendly.
            return Convert.ToBase64String(guid.ToByteArray()).Replace("/", "_").Replace("+", "-").Substring(0, 22);
        }
    }
}