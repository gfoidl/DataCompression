using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using gfoidl.DataCompression.Wrappers;

namespace gfoidl.DataCompression
{
    /// <summary>
    /// Swinging door compression.
    /// </summary>
    /// <remarks>
    /// See documentation for further information.
    /// </remarks>
    public partial class SwingingDoorCompression : Compression
    {
        /// <summary>
        /// Implementation of the compression / filtering.
        /// </summary>
        /// <param name="data">Input data</param>
        /// <returns>The compressed / filtered data.</returns>
        protected override IAsyncEnumerable<DataPoint> ProcessAsyncCore(IAsyncEnumerable<DataPoint> data, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
