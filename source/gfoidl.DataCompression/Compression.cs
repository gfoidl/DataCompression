// (c) gfoidl, all rights reserved

using System;
using System.Collections.Generic;

namespace gfoidl.DataCompression
{
    /// <summary>
    /// Defines the interface for the compression algorithms.
    /// </summary>
    public abstract class Compression : ICompression
    {
        private protected readonly double? _maxDeltaX;
        private protected readonly double? _minDeltaX;
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
            _maxDeltaX = maxDeltaX;
            _minDeltaX = minDeltaX;
        }
        //---------------------------------------------------------------------
        /// <inheritdoc />
        public abstract bool ArchiveIncoming { get; }
        //---------------------------------------------------------------------
        /// <inheritdoc />
        public double? MaxDeltaX => _maxDeltaX;
        //---------------------------------------------------------------------
        /// <inheritdoc />
        public double? MinDeltaX => _minDeltaX;
        //---------------------------------------------------------------------
        /// <inheritdoc />
        public DataPointIterator Process(IEnumerable<DataPoint> data)
        {
            if (data is null) ThrowHelper.ThrowArgumentNull(ThrowHelper.ExceptionArgument.data);

            return this.ProcessCore(data);
        }
        //---------------------------------------------------------------------
#if NETSTANDARD2_1
        /// <inheritdoc />
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
