using System;
using System.Diagnostics;

namespace gfoidl.DataCompression
{
    internal static class ThrowHelper
    {
        public static void ThrowArgumentNull(ExceptionArgument argument)       => throw new ArgumentNullException(GetArgumentName(argument));
        public static void ThrowArgumentOutOfRange(ExceptionArgument argument) => throw new ArgumentOutOfRangeException(GetArgumentName(argument));
        public static void ThrowArgument(ExceptionResource resource)           => throw new ArgumentException(GetResourceText(resource));
        public static void ThrowNotSupported()                                 => throw new NotSupportedException();
        public static void ThrowIfDisposed(ExceptionArgument argument)         => throw new ObjectDisposedException(argument.ToString());
        public static void ThrowInvalidOperation(ExceptionResource resource)   => throw new InvalidOperationException(GetResourceText(resource));
        //---------------------------------------------------------------------
        private static string GetArgumentName(ExceptionArgument argument)
        {
            Debug.Assert(
                Enum.IsDefined(typeof(ExceptionArgument), argument),
                "The enum value is not defined, please check the 'ExceptionArgument' enum.");

            return argument.ToString();
        }
        //---------------------------------------------------------------------
        private static string GetResourceName(ExceptionResource resource)
        {
            Debug.Assert(
                Enum.IsDefined(typeof(ExceptionResource), resource),
                "The enum value is not defined, please check the 'ExceptionResource' enum.");

            return resource.ToString();
        }
        //---------------------------------------------------------------------
        private static string GetResourceText(ExceptionResource resource)
            => Strings.ResourceManager.GetString(GetResourceName(resource), Strings.Culture);
        //---------------------------------------------------------------------
        public enum ExceptionArgument
        {
            data,
            iterator
        }
        //---------------------------------------------------------------------
        public enum ExceptionResource
        {
            GetEnumerator_must_be_called_first,
            Gradient_A_eq_B,
            Should_not_happen
        }
    }
}
