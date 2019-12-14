using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using gfoidl.DataCompression.Builders;

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
            => new EnumerableIterator(data);
        //---------------------------------------------------------------------
#if NETCOREAPP
        /// <summary>
        /// Implementation of the compression / filtering.
        /// </summary>
        /// <param name="data">Input data</param>
        /// <returns>The compressed / filtered data.</returns>
        protected override async IAsyncEnumerable<DataPoint> ProcessAsyncCore(
            IAsyncEnumerable<DataPoint> data,
            [EnumeratorCancellation] CancellationToken ct)
        {
            await foreach (DataPoint dataPoint in data)
            {
                if (ct.IsCancellationRequested)
                    break;

                yield return dataPoint;
            }
        }
#endif
        //---------------------------------------------------------------------
        private sealed class EnumerableIterator : DataPointIterator
        {
            private readonly IEnumerable<DataPoint> _enumerable;
            private readonly IEnumerator<DataPoint> _enumerator;
            //-----------------------------------------------------------------
            public EnumerableIterator(IEnumerable<DataPoint> enumerable)
            {
                _enumerable = enumerable;
                _enumerator = enumerable.GetEnumerator();
            }
            //-----------------------------------------------------------------
            public override DataPointIterator Clone() => new EnumerableIterator(_enumerable);
            //-----------------------------------------------------------------
            public override bool MoveNext()
            {
                if (_state == InitialState)
                    ThrowHelper.ThrowInvalidOperation(ThrowHelper.ExceptionResource.GetEnumerator_must_be_called_first);

                if (_enumerator.MoveNext())
                {
                    _current = _enumerator.Current;
                    return true;
                }

                return false;
            }
            //-----------------------------------------------------------------
            public override DataPoint[] ToArray()
            {
                var arrayBuilder = new ArrayBuilder<DataPoint>(true);
                arrayBuilder.AddRange(_enumerable);

                return arrayBuilder.ToArray();
            }
            //---------------------------------------------------------------------
            public override List<DataPoint> ToList() => new List<DataPoint>(_enumerable);
            //---------------------------------------------------------------------
            public override void Dispose()
            {
                base.Dispose();
                _enumerator.Dispose();
            }
        }
    }
}
