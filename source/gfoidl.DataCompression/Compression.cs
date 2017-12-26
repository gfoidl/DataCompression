using System.Collections.Generic;

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
        /// <typeparam name="TList">The type of the enumeration / list.</typeparam>
        /// <param name="data">Input data</param>
        /// <returns>The compressed / filtered data.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="data" /> is <c>null</c>.
        /// </exception>
        public IEnumerable<DataPoint> Process<TList>(in TList data) where TList : IEnumerable<DataPoint>
        {
            if (data == null) ThrowHelper.ThrowArgumentNull(nameof(data));

            return this.ProcessCore(data);
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Implementation of the compression / filtering.
        /// </summary>
        /// <typeparam name="TList">The type of the enumeration / list.</typeparam>
        /// <param name="data">Input data</param>
        /// <returns>The compressed / filtered data.</returns>
        protected abstract IEnumerable<DataPoint> ProcessCore<TList>(in TList data) where TList : IEnumerable<DataPoint>;
    }
}