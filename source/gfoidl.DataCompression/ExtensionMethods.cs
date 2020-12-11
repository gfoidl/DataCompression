// (c) gfoidl, all rights reserved

using System;
using System.Collections.Generic;

namespace gfoidl.DataCompression
{
    /// <summary>
    /// Extension methods for easy access and fluent interface to compression methods.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// A filter that performs no compression.
        /// </summary>
        /// <param name="data">Input data</param>
        /// <returns>The unmodified input data.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="data" /> is <c>null</c>
        /// </exception>
        public static DataPointIterator NoCompression(this IEnumerable<DataPoint> data)
        {
            if (data is null) ThrowHelper.ThrowArgumentNull(ThrowHelper.ExceptionArgument.data);

            var compression = new NoCompression();
            return compression.Process(data);
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// A filter that performs dead band compression.
        /// </summary>
        /// <param name="data">Input data</param>
        /// <param name="instrumentPrecision">(Absolut) precision of the instrument</param>
        /// <param name="maxDeltaX">
        /// Length of x before for sure a value gets recoreded. See <see cref="Compression.MaxDeltaX" />.
        /// </param>
        /// <param name="minDeltaX">
        /// Length of x/time within no value gets recorded (after the last archived value).
        /// See <see cref="Compression.MinDeltaX" />.
        /// </param>
        /// <returns>Dead band compressed / filtered data.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="data" /> is <c>null</c>
        /// </exception>
        public static DataPointIterator DeadBandCompression(this IEnumerable<DataPoint> data, double instrumentPrecision, double? maxDeltaX = null, double? minDeltaX = null)
        {
            if (data is null) ThrowHelper.ThrowArgumentNull(ThrowHelper.ExceptionArgument.data);

            var compression = new DeadBandCompression(instrumentPrecision, maxDeltaX, minDeltaX);
            return compression.Process(data);
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// A filter that performs dead band compression.
        /// </summary>
        /// <param name="data">Input data</param>
        /// <param name="instrumentPrecision">(Absolut) precision of the instrument</param>
        /// <param name="maxTime">Length of time before for sure a value gets recoreded</param>
        /// <param name="minTime">Length of time within no value gets recorded (after the last archived value)</param>
        /// <returns>Dead band compressed / filtered data.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="data" /> is <c>null</c>
        /// </exception>
        public static DataPointIterator DeadBandCompression(this IEnumerable<DataPoint> data, double instrumentPrecision, TimeSpan maxTime, TimeSpan? minTime = null)
        {
            if (data is null) ThrowHelper.ThrowArgumentNull(ThrowHelper.ExceptionArgument.data);

            var compression = new DeadBandCompression(instrumentPrecision, maxTime, minTime);
            return compression.Process(data);
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// A filter that performs swinging door compression.
        /// </summary>
        /// <param name="data">Input data</param>
        /// <param name="compressionDeviation">
        /// (Absolut) Compression deviation applied to the y values to calculate the
        /// min and max slopes.
        /// </param>
        /// <param name="maxDeltaX">
        /// Length of x before for sure a value gets recoreded. See <see cref="Compression.MaxDeltaX" />.
        /// </param>
        /// <param name="minDeltaX">
        /// Length of x/time within no value gets recorded (after the last archived value).
        /// See <see cref="Compression.MinDeltaX" />.
        /// </param>
        /// <returns>swinging door compressed / filtered data.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="data" /> is <c>null</c>
        /// </exception>
        public static DataPointIterator SwingingDoorCompression(this IEnumerable<DataPoint> data, double compressionDeviation, double? maxDeltaX = null, double? minDeltaX = null)
        {
            if (data is null) ThrowHelper.ThrowArgumentNull(ThrowHelper.ExceptionArgument.data);

            var compression = new SwingingDoorCompression(compressionDeviation, maxDeltaX, minDeltaX);
            return compression.Process(data);
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// A filter that performs swinging door compression.
        /// </summary>
        /// <param name="data">Input data</param>
        /// <param name="compressionDeviation">
        /// (Absolut) Compression deviation applied to the y values to calculate the
        /// min and max slopes.
        /// </param>
        /// <param name="maxTime">Length of time before for sure a value gets recoreded</param>
        /// <param name="minTime">Length of time within no value gets recorded (after the last archived value)</param>
        /// <returns>swinging door compressed / filtered data.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="data" /> is <c>null</c>
        /// </exception>
        public static DataPointIterator SwingingDoorCompression(this IEnumerable<DataPoint> data, double compressionDeviation, TimeSpan maxTime, TimeSpan? minTime = null)
        {
            if (data is null) ThrowHelper.ThrowArgumentNull(ThrowHelper.ExceptionArgument.data);

            var compression = new SwingingDoorCompression(compressionDeviation, maxTime, minTime);
            return compression.Process(data);
        }
        //---------------------------------------------------------------------
#if NETSTANDARD2_1
        /// <summary>
        /// A filter that performs no compression.
        /// </summary>
        /// <param name="data">Input data</param>
        /// <returns>The unmodified input data.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="data" /> is <c>null</c>
        /// </exception>
        public static DataPointIterator NoCompressionAsync(this IAsyncEnumerable<DataPoint> data)
        {
            if (data is null) ThrowHelper.ThrowArgumentNull(ThrowHelper.ExceptionArgument.data);

            var compression = new NoCompression();
            return compression.ProcessAsync(data);
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// A filter that performs dead band compression.
        /// </summary>
        /// <param name="data">Input data</param>
        /// <param name="instrumentPrecision">(Absolut) precision of the instrument</param>
        /// <param name="maxDeltaX">
        /// Length of x before for sure a value gets recoreded. See <see cref="Compression.MaxDeltaX" />.
        /// </param>
        /// <param name="minDeltaX">
        /// Length of x/time within no value gets recorded (after the last archived value).
        /// See <see cref="Compression.MinDeltaX" />.
        /// </param>
        /// <returns>Dead band compressed / filtered data.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="data" /> is <c>null</c>
        /// </exception>
        public static DataPointIterator DeadBandCompressionAsync(this IAsyncEnumerable<DataPoint> data, double instrumentPrecision, double? maxDeltaX = null, double? minDeltaX = null)
        {
            if (data is null) ThrowHelper.ThrowArgumentNull(ThrowHelper.ExceptionArgument.data);

            var compression = new DeadBandCompression(instrumentPrecision, maxDeltaX, minDeltaX);
            return compression.ProcessAsync(data);
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// A filter that performs dead band compression.
        /// </summary>
        /// <param name="data">Input data</param>
        /// <param name="instrumentPrecision">(Absolut) precision of the instrument</param>
        /// <param name="maxTime">Length of time before for sure a value gets recoreded</param>
        /// <param name="minTime">Length of time within no value gets recorded (after the last archived value)</param>
        /// <returns>Dead band compressed / filtered data.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="data" /> is <c>null</c>
        /// </exception>
        public static DataPointIterator DeadBandCompressionAsync(this IAsyncEnumerable<DataPoint> data, double instrumentPrecision, TimeSpan maxTime, TimeSpan? minTime = null)
        {
            if (data is null) ThrowHelper.ThrowArgumentNull(ThrowHelper.ExceptionArgument.data);

            var compression = new DeadBandCompression(instrumentPrecision, maxTime, minTime);
            return compression.ProcessAsync(data);
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// A filter that performs swinging door compression.
        /// </summary>
        /// <param name="data">Input data</param>
        /// <param name="compressionDeviation">
        /// (Absolut) Compression deviation applied to the y values to calculate the
        /// min and max slopes.
        /// </param>
        /// <param name="maxDeltaX">
        /// Length of x before for sure a value gets recoreded. See <see cref="Compression.MaxDeltaX" />.
        /// </param>
        /// <param name="minDeltaX">
        /// Length of x/time within no value gets recorded (after the last archived value).
        /// See <see cref="Compression.MinDeltaX" />.
        /// </param>
        /// <returns>swinging door compressed / filtered data.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="data" /> is <c>null</c>
        /// </exception>
        public static DataPointIterator SwingingDoorCompressionAsync(this IAsyncEnumerable<DataPoint> data, double compressionDeviation, double? maxDeltaX = null, double? minDeltaX = null)
        {
            if (data is null) ThrowHelper.ThrowArgumentNull(ThrowHelper.ExceptionArgument.data);

            var compression = new SwingingDoorCompression(compressionDeviation, maxDeltaX, minDeltaX);
            return compression.ProcessAsync(data);
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// A filter that performs swinging door compression.
        /// </summary>
        /// <param name="data">Input data</param>
        /// <param name="compressionDeviation">
        /// (Absolut) Compression deviation applied to the y values to calculate the
        /// min and max slopes.
        /// </param>
        /// <param name="maxTime">Length of time before for sure a value gets recoreded</param>
        /// <param name="minTime">Length of time within no value gets recorded (after the last archived value)</param>
        /// <returns>swinging door compressed / filtered data.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="data" /> is <c>null</c>
        /// </exception>
        public static DataPointIterator SwingingDoorCompressionAsync(this IAsyncEnumerable<DataPoint> data, double compressionDeviation, TimeSpan maxTime, TimeSpan? minTime = null)
        {
            if (data is null) ThrowHelper.ThrowArgumentNull(ThrowHelper.ExceptionArgument.data);

            var compression = new SwingingDoorCompression(compressionDeviation, maxTime, minTime);
            return compression.ProcessAsync(data);
        }
#endif
    }
}
