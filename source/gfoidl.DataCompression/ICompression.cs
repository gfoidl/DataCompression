// (c) gfoidl, all rights reserved

using System;
using System.Collections.Generic;

namespace gfoidl.DataCompression
{
    /// <summary>
    /// Defines the interface for the compression algorithms.
    /// </summary>
    public interface ICompression
    {
        /// <summary>
        /// When set to <c>true</c> the incoming is value archived in addition
        /// to the last snapshot.
        /// </summary>
        /// <remarks>
        /// For instance in the <see cref="DeadBandCompression" /> the last snapshot
        /// is archived, as well as the current incoming value, therefore this property
        /// is set to <c>true</c>.
        /// In the <see cref="SwingingDoorCompression" /> only the last snapshot is
        /// archived, so this property is set to <c>false</c>.
        /// </remarks>
        bool ArchiveIncoming { get; }
        //-------------------------------------------------------------------------
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
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="data" /> is <c>null</c>.
        /// </exception>
        DataPointIterator Process(IEnumerable<DataPoint> data);
        //---------------------------------------------------------------------
#if NETSTANDARD2_1
        /// <summary>
        /// Performs the compression / filtering of the input data.
        /// </summary>
        /// <param name="data">Input data</param>
        /// <returns>The compressed / filtered data.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="data" /> is <c>null</c>.
        /// </exception>
        DataPointIterator ProcessAsync(IAsyncEnumerable<DataPoint> data);
#endif
    }
}
