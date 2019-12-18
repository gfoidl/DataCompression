using System;
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
        double? MaxDeltaX { get; }
        //---------------------------------------------------------------------
        /// <summary>
        /// Length of x/time within no value gets recorded (after the last archived value)
        /// </summary>
        double? MinDeltaX { get; }
        //---------------------------------------------------------------------
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
        /// <param name="ct">The token for cancellation.</param>
        /// <returns>The compressed / filtered data.</returns>
        DataPointIterator ProcessAsync(IAsyncEnumerable<DataPoint>? data, CancellationToken ct = default);
#endif
    }
}
