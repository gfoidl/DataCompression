using System;
using System.Collections.Generic;

namespace gfoidl.DataCompression
{
    /// <summary>
    /// Defines the interface for the compression algorithms.
    /// </summary>
    public abstract class Compression : ICompression
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected internal readonly double _maxDeltaX;
        protected internal readonly bool   _minDeltaXHasValue;
        protected internal readonly double _minDeltaX;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        //---------------------------------------------------------------------
        /// <summary>
        /// Creates a new instance of <see cref="Compression" />.
        /// </summary>
        /// <param name="maxDeltaX">
        /// Length of x before for sure a value gets recoreded. See <see cref="MaxDeltaX" />.
        /// </param>
        /// <param name="minDeltaX">
        /// Length of x/time within no value gets recorded (after the last archived value).
        /// See <see cref="MinDeltaX" />.
        /// </param>
        protected Compression(double? maxDeltaX = null, double? minDeltaX = null)
        {
            _maxDeltaX = maxDeltaX ?? double.MaxValue;

            if (minDeltaX.HasValue)
            {
                _minDeltaXHasValue = true;
                _minDeltaX         = minDeltaX.Value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Length of x before for sure a value gets recorded.
        /// </summary>
        /// <remarks>
        /// Cf. ExMax in documentation.<br />
        /// When specified as <see cref="DateTime" /> the <see cref="DateTime.Ticks" />
        /// are used.
        /// <para>
        /// When value is <c>null</c>, no value -- except the first and last -- are
        /// guaranteed to be recorded.
        /// </para>
        /// </remarks>
        public double? MaxDeltaX => _maxDeltaX == double.MaxValue ? (double?)null : _maxDeltaX;
        //---------------------------------------------------------------------
        /// <summary>
        /// Length of x/time within no value gets recorded (after the last archived value)
        /// </summary>
        public double? MinDeltaX => _minDeltaXHasValue ? _minDeltaX : (double?)null;
        //---------------------------------------------------------------------
        /// <summary>
        /// Performs the compression / filtering of the input data.
        /// </summary>
        /// <param name="data">Input data</param>
        /// <returns>The compressed / filtered data.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="data" /> is <c>null</c>.
        /// </exception>
        public DataPointIterator Process(IEnumerable<DataPoint> data)
        {
            if (data is null) ThrowHelper.ThrowArgumentNull(ThrowHelper.ExceptionArgument.data);

            return this.ProcessCore(data);
        }
        //---------------------------------------------------------------------
#if NETSTANDARD2_1
        /// <summary>
        /// Performs the compression / filtering of the input data.
        /// </summary>
        /// <param name="data">Input data</param>
        /// <returns>The compressed / filtered data.</returns>
        public DataPointIterator ProcessAsync(IAsyncEnumerable<DataPoint> data)
        {
            if (data is null) ThrowHelper.ThrowArgumentNull(ThrowHelper.ExceptionArgument.data);

            return this.ProcessAsyncCore(data);
        }
#endif
        //---------------------------------------------------------------------
        /// <summary>
        /// Implementation of the compression / filtering.
        /// </summary>
        /// <param name="data">Input data</param>
        /// <returns>The compressed / filtered data.</returns>
        protected abstract DataPointIterator ProcessCore(IEnumerable<DataPoint> data);
        //---------------------------------------------------------------------
#if NETSTANDARD2_1
        /// <summary>
        /// Implementation of the compression / filtering.
        /// </summary>
        /// <param name="data">Input data</param>
        /// <returns>The compressed / filtered data.</returns>
        protected abstract DataPointIterator ProcessAsyncCore(IAsyncEnumerable<DataPoint> data);
#endif
    }
}
