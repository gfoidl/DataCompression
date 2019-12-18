﻿using System.Collections.Generic;
using System.Threading;

namespace gfoidl.DataCompression
{
    /// <summary>
    /// A filter that performs no compression
    /// </summary>
    public partial class NoCompression : Compression
    {
        internal static readonly NoCompression s_instance = new NoCompression();
        //---------------------------------------------------------------------
        /// <summary>
        /// Implementation of the compression / filtering.
        /// </summary>
        /// <param name="data">Input data</param>
        /// <returns>The compressed / filtered data.</returns>
        protected override DataPointIterator ProcessCore(IEnumerable<DataPoint> data)
            => new EnumerableIterator(this, data);
        //---------------------------------------------------------------------
#if NETSTANDARD2_1
        /// <summary>
        /// Implementation of the compression / filtering.
        /// </summary>
        /// <param name="data">Input data</param>
        /// <param name="ct">The token for cancellation.</param>
        /// <returns>The compressed / filtered data.</returns>
        protected override DataPointIterator ProcessAsyncCore(IAsyncEnumerable<DataPoint> data, CancellationToken ct)
            => new AsyncEnumerableIterator(this, data, ct);
#endif
    }
}
