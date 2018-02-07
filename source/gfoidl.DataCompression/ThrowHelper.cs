using System;

namespace gfoidl.DataCompression
{
    internal static class ThrowHelper
    {
        public static void ThrowArgumentNull(Argument argument)    => throw new ArgumentNullException(argument.ToString());
        public static void ThrowArgumentOutOfRange(string argName) => throw new ArgumentOutOfRangeException(argName);
        public static void ThrowNotSupported()                     => throw new NotSupportedException();
        public static void ThrowIfDisposed(string objName)         => throw new ObjectDisposedException(objName);
        public static void ThrowInvalidOperation(Reason reason)    => throw new InvalidOperationException(GetReasonString(reason));
        //---------------------------------------------------------------------
        public enum Argument
        {
            data
        }
        //---------------------------------------------------------------------
        public enum Reason
        {
            ShouldNotHappen
        }
        //---------------------------------------------------------------------
        private static string GetReasonString(Reason reason)
        {
            switch (reason)
            {
                case Reason.ShouldNotHappen:
                    return Strings.Should_not_happen;
                default:
                    return string.Empty;
            }
        }
    }
}
