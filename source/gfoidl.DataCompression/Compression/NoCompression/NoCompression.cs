// (c) gfoidl, all rights reserved

using System.Collections.Generic;
using gfoidl.DataCompression.Internal.NoCompression;

namespace gfoidl.DataCompression
{
    /// <summary>
    /// A filter that performs no compression
    /// </summary>
    public sealed class NoCompression : Compression
    {
        internal static readonly NoCompression s_instance = new NoCompression();
        //---------------------------------------------------------------------
        /// <summary>
        /// Implementation of the compression / filtering.
        /// </summary>
        /// <param name="data">Input data</param>
        /// <returns>The compressed / filtered data.</returns>
        protected override DataPointIterator ProcessCore(IEnumerable<DataPoint> data)
        {
            EnumerableIterator iter = new();
            iter.SetData(this, data);

            return iter;
        }
        //---------------------------------------------------------------------
#if NETSTANDARD2_1
        /// <summary>
        /// Implementation of the compression / filtering.
        /// </summary>
        /// <param name="data">Input data</param>
        /// <returns>The compressed / filtered data.</returns>
        protected override DataPointIterator ProcessAsyncCore(IAsyncEnumerable<DataPoint> data)
        {
            AsyncEnumerableIterator iter = new();
            iter.SetData(this, data);

            return iter;
        }
#endif
    }
}
