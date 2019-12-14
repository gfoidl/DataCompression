using System.Collections.Generic;
using System.Threading;

namespace gfoidl.DataCompression
{
    /// <summary>
    /// Defines the interface for the compression algorithms.
    /// </summary>
    public interface ICompression
    {
        /// <summary>
        /// Performs the compression / filtering of the input data.
        /// </summary>
        /// <param name="data">Input data</param>
        /// <returns>The compressed / filtered data.</returns>
        DataPointIterator Process(IEnumerable<DataPoint>? data);
        //---------------------------------------------------------------------
#if NETSTANDARD2_1
        /// <summary>
        /// Performs the compression / filtering of the input data.
        /// </summary>
        /// <param name="data">Input data</param>
        /// <returns>The compressed / filtered data.</returns>
        DataPointAsyncIterator ProcessAsync(IAsyncEnumerable<DataPoint>? data, CancellationToken ct = default);
#endif
    }
}
