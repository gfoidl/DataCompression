using System;
using System.Runtime.CompilerServices;

namespace gfoidl.DataCompression
{
    internal static class ThrowHelper
    {
        public static void ThrowArgument(string message, string argName) => throw CreateArgument(message, argName);
        public static void ThrowArgumentNull(string argName)             => throw CreateArgumentNull(argName);
        public static void ThrowArgumentOutOfRange(string argName)       => throw CreateArgumentOutOfRange(argName);
        //---------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Exception CreateArgument(string message, string argName) => new ArgumentException(message, argName);
        //---------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Exception CreateArgumentNull(string argName) => new ArgumentNullException(argName);
        //---------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Exception CreateArgumentOutOfRange(string argName) => new ArgumentOutOfRangeException(argName);
    }
}