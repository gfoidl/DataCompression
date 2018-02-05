using System;

namespace gfoidl.DataCompression
{
    internal static class ThrowHelper
    {
        public static void ThrowArgumentNull(string argName)       => throw new ArgumentNullException(argName);
        public static void ThrowArgumentOutOfRange(string argName) => throw new ArgumentOutOfRangeException(argName);
        public static void ThrowNotSupported()                     => throw new NotSupportedException();
        public static void ThrowIfDisposed(string objName)         => throw new ObjectDisposedException(objName);
    }
}
