﻿using System.Collections.Generic;
using System.Threading;

namespace gfoidl.DataCompression
{
    /// <summary>
    /// Defines the interface for the compression algorithms.
    /// </summary>
    public abstract class Compression : ICompression
    {
        /// <summary>
        /// Performs the compression / filtering of the input data.
        /// </summary>
        /// <param name="data">Input data</param>
        /// <returns>The compressed / filtered data.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="data" /> is <c>null</c>.
        /// </exception>
        public DataPointIterator Process(IEnumerable<DataPoint>? data)
        {
            if (data is null) ThrowHelper.ThrowArgumentNull(ThrowHelper.ExceptionArgument.data);

            return this.ProcessCore(data);
        }
        //---------------------------------------------------------------------
#if NETCOREAPP
        /// <summary>
        /// Performs the compression / filtering of the input data.
        /// </summary>
        /// <param name="data">Input data</param>
        /// <returns>The compressed / filtered data.</returns>
        public IAsyncEnumerable<DataPoint> ProcessAsync(IAsyncEnumerable<DataPoint>? data, CancellationToken ct = default)
        {
            if (data is null) ThrowHelper.ThrowArgumentNull(ThrowHelper.ExceptionArgument.data);

            return this.ProcessAsyncCore(data, ct);
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
#if NETCOREAPP
        /// <summary>
        /// Implementation of the compression / filtering.
        /// </summary>
        /// <param name="data">Input data</param>
        /// <returns>The compressed / filtered data.</returns>
        protected abstract IAsyncEnumerable<DataPoint> ProcessAsyncCore(IAsyncEnumerable<DataPoint> data, CancellationToken ct);
#endif
    }
}
