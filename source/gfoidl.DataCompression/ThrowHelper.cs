using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace gfoidl.DataCompression
{
    internal static class ThrowHelper
    {
        [DoesNotReturn] public static void ThrowArgumentNull(ExceptionArgument argument)       => throw new ArgumentNullException(GetArgumentName(argument));
        [DoesNotReturn] public static void ThrowArgumentOutOfRange(ExceptionArgument argument) => throw new ArgumentOutOfRangeException(GetArgumentName(argument));
        [DoesNotReturn] public static void ThrowArgument(ExceptionResource resource)           => throw new ArgumentException(GetResourceText(resource));
        [DoesNotReturn] public static void ThrowNotSupported()                                 => throw new NotSupportedException();
        [DoesNotReturn] public static void ThrowIfDisposed(ExceptionArgument argument)         => throw new ObjectDisposedException(argument.ToString());
        [DoesNotReturn] public static void ThrowInvalidOperation(ExceptionResource resource)   => throw new InvalidOperationException(GetResourceText(resource));
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
        {
            string? tmp = Strings.ResourceManager.GetString(GetResourceName(resource), Strings.Culture);

            Debug.Assert(tmp != null);
            return tmp!;
        }
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
//-----------------------------------------------------------------------------
#if !NETSTANDARD2_1
namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    internal sealed class DoesNotReturnAttribute : Attribute
    { }
}
#endif
