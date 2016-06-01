using System;

namespace CardTable
{
    public static class StringExtensionMethods
    {
        public static string Overwrite(this string destination, int index, string subString)
        {
            if (destination == null)
            {
                throw new ArgumentException("Destination must not be null");
            }

            if (subString == null)
            {
                throw new ArgumentException("Substring must not be null");
            }

            int destinationLength = destination.Length;
            int subStringLength = subString.Length;

            char[] chars = destination.ToCharArray();

            for (int i = 0; i < subStringLength; i++)
            {
                if (i + index >= 0 && i + index < destinationLength)
                {
                    chars[i + index] = subString[i];
                }
            }

            return new string(chars);
        }
    }
}