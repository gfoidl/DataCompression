using System;
using System.Collections.Generic;
using System.Threading;

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
        public static DataPointIterator NoCompression(this IEnumerable<DataPoint>? data)
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
        /// Length of x before for sure a value gets recoreded. See <see cref="DeadBandCompression.MaxDeltaX" />.
        /// </param>
        /// <returns>Dead band compressed / filtered data.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="data" /> is <c>null</c>
        /// </exception>
        public static DataPointIterator DeadBandCompression(this IEnumerable<DataPoint>? data, double instrumentPrecision, double? maxDeltaX = null)
        {
            if (data is null) ThrowHelper.ThrowArgumentNull(ThrowHelper.ExceptionArgument.data);

            var compression = new DeadBandCompression(instrumentPrecision, maxDeltaX);
            return compression.Process(data);
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// A filter that performs dead band compression.
        /// </summary>
        /// <param name="data">Input data</param>
        /// <param name="instrumentPrecision">(Absolut) precision of the instrument</param>
        /// <param name="maxTime">Length of time before for sure a value gets recoreded</param>
        /// <returns>Dead band compressed / filtered data.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="data" /> is <c>null</c>
        /// </exception>
        public static DataPointIterator DeadBandCompression(this IEnumerable<DataPoint>? data, double instrumentPrecision, TimeSpan maxTime)
        {
            if (data is null) ThrowHelper.ThrowArgumentNull(ThrowHelper.ExceptionArgument.data);

            var compression = new DeadBandCompression(instrumentPrecision, maxTime);
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
        /// Length of x before for sure a value gets recoreded. See <see cref="SwingingDoorCompression.MaxDeltaX" />.
        /// </param>
        /// <param name="minDeltaX">
        /// Length of x/time within no value gets recorded (after the last archived value).
        /// See <see cref="SwingingDoorCompression.MinDeltaX" />.
        /// </param>
        /// <returns>swinging door compressed / filtered data.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="data" /> is <c>null</c>
        /// </exception>
        public static DataPointIterator SwingingDoorCompression(this IEnumerable<DataPoint>? data, double compressionDeviation, double? maxDeltaX = null, double? minDeltaX = null)
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
        public static DataPointIterator SwingingDoorCompression(this IEnumerable<DataPoint>? data, double compressionDeviation, TimeSpan maxTime, TimeSpan? minTime)
        {
            if (data is null) ThrowHelper.ThrowArgumentNull(ThrowHelper.ExceptionArgument.data);

            var compression = new SwingingDoorCompression(compressionDeviation, maxTime, minTime);
            return compression.Process(data);
        }
        //---------------------------------------------------------------------
#if NETCOREAPP
        /// <summary>
        /// A filter that performs no compression.
        /// </summary>
        /// <param name="data">Input data</param>
        /// <returns>The unmodified input data.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="data" /> is <c>null</c>
        /// </exception>
        public static IAsyncEnumerable<DataPoint> NoCompression(this IAsyncEnumerable<DataPoint>? data, CancellationToken ct = default)
        {
            if (data is null) ThrowHelper.ThrowArgumentNull(ThrowHelper.ExceptionArgument.data);

            var compression = new NoCompression();
            return compression.ProcessAsync(data, ct);
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// A filter that performs dead band compression.
        /// </summary>
        /// <param name="data">Input data</param>
        /// <param name="instrumentPrecision">(Absolut) precision of the instrument</param>
        /// <param name="maxDeltaX">
        /// Length of x before for sure a value gets recoreded. See <see cref="DeadBandCompression.MaxDeltaX" />.
        /// </param>
        /// <returns>Dead band compressed / filtered data.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="data" /> is <c>null</c>
        /// </exception>
        public static IAsyncEnumerable<DataPoint> DeadBandCompression(this IAsyncEnumerable<DataPoint>? data, double instrumentPrecision, double? maxDeltaX = null, CancellationToken ct = default)
        {
            if (data is null) ThrowHelper.ThrowArgumentNull(ThrowHelper.ExceptionArgument.data);

            var compression = new DeadBandCompression(instrumentPrecision, maxDeltaX);
            return compression.ProcessAsync(data, ct);
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// A filter that performs dead band compression.
        /// </summary>
        /// <param name="data">Input data</param>
        /// <param name="instrumentPrecision">(Absolut) precision of the instrument</param>
        /// <param name="maxTime">Length of time before for sure a value gets recoreded</param>
        /// <returns>Dead band compressed / filtered data.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="data" /> is <c>null</c>
        /// </exception>
        public static IAsyncEnumerable<DataPoint> DeadBandCompression(this IAsyncEnumerable<DataPoint>? data, double instrumentPrecision, TimeSpan maxTime, CancellationToken ct = default)
        {
            if (data is null) ThrowHelper.ThrowArgumentNull(ThrowHelper.ExceptionArgument.data);

            var compression = new DeadBandCompression(instrumentPrecision, maxTime);
            return compression.ProcessAsync(data, ct);
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
        /// Length of x before for sure a value gets recoreded. See <see cref="SwingingDoorCompression.MaxDeltaX" />.
        /// </param>
        /// <param name="minDeltaX">
        /// Length of x/time within no value gets recorded (after the last archived value).
        /// See <see cref="SwingingDoorCompression.MinDeltaX" />.
        /// </param>
        /// <returns>swinging door compressed / filtered data.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="data" /> is <c>null</c>
        /// </exception>
        public static IAsyncEnumerable<DataPoint> SwingingDoorCompression(this IAsyncEnumerable<DataPoint>? data, double compressionDeviation, double? maxDeltaX = null, double? minDeltaX = null, CancellationToken ct = default)
        {
            if (data is null) ThrowHelper.ThrowArgumentNull(ThrowHelper.ExceptionArgument.data);

            var compression = new SwingingDoorCompression(compressionDeviation, maxDeltaX, minDeltaX);
            return compression.ProcessAsync(data, ct);
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
        public static IAsyncEnumerable<DataPoint> SwingingDoorCompression(this IAsyncEnumerable<DataPoint>? data, double compressionDeviation, TimeSpan maxTime, TimeSpan? minTime, CancellationToken ct = default)
        {
            if (data is null) ThrowHelper.ThrowArgumentNull(ThrowHelper.ExceptionArgument.data);

            var compression = new SwingingDoorCompression(compressionDeviation, maxTime, minTime);
            return compression.ProcessAsync(data, ct);
        }
#endif
    }
}
