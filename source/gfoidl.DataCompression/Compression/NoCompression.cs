using System.Collections.Generic;

namespace gfoidl.DataCompression
{
    /// <summary>
    /// A filter that performs no compression
    /// </summary>
    public class NoCompression : Compression
    {
        /// <summary>
        /// Implementation of the compression / filtering.
        /// </summary>
        /// <param name="data">Input data</param>
        /// <returns>The compressed / filtered data.</returns>
        protected override IEnumerable<DataPoint> ProcessCore(IEnumerable<DataPoint> data) => data;
    }
}
