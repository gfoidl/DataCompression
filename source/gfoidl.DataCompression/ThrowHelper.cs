using System;

namespace gfoidl.DataCompression
{
    internal static class ThrowHelper
    {
        public static void ThrowArgumentNull(string argName) => throw new ArgumentNullException(argName);
    }
}
