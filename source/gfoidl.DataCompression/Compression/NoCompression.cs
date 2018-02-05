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
        protected override DataPointIterator ProcessCore(IEnumerable<DataPoint> data)
            => new EnumerableIterator(data.GetEnumerator());
        //---------------------------------------------------------------------
        private sealed class EnumerableIterator : DataPointIterator
        {
            private readonly IEnumerator<DataPoint> _enumerator;
            //-----------------------------------------------------------------
            public EnumerableIterator(IEnumerator<DataPoint> enumerator) => _enumerator = enumerator;
            //-----------------------------------------------------------------
            public override DataPointIterator Clone() => new EnumerableIterator(_enumerator);
            //-----------------------------------------------------------------
            public override bool MoveNext()
            {
                if (_enumerator.MoveNext())
                {
                    _current = _enumerator.Current;
                    return true;
                }

                return false;
            }
        }
    }
}
