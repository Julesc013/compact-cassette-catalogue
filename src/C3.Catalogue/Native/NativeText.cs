using System;

namespace C3.Catalogue.Native
{
    internal static class NativeText
    {
        public static string Required(string value, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("A non-whitespace value is required.", parameterName);
            }

            return value;
        }

        public static string Optional(string value)
        {
            return value ?? string.Empty;
        }
    }
}
