using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace gfoidl.DataCompression.Internal.SwingingDoor
{
    internal sealed class SequentialEnumerableIterator : SwingingDoorCompressionIterator
    {
        public void SetData(SwingingDoorCompression swingingDoorCompression, IEnumerable<DataPoint> source)
        {
            this.SetData(swingingDoorCompression as Compression, source);
            _swingingDoorCompression = swingingDoorCompression;
        }
        //---------------------------------------------------------------------
        public override DataPointIterator Clone()
        {
            Debug.Assert(_swingingDoorCompression is not null);
            Debug.Assert(_source                  is not null);

            SequentialEnumerableIterator clone = new();
            clone.SetData(_swingingDoorCompression, _source);

            return clone;
        }
        //---------------------------------------------------------------------
#if NETSTANDARD2_1
        public override ValueTask<DataPoint[]> ToArrayAsync(CancellationToken ct)    => throw new NotSupportedException();
        public override ValueTask<List<DataPoint>> ToListAsync(CancellationToken ct) => throw new NotSupportedException();
#endif
        //---------------------------------------------------------------------
        protected override void DisposeCore()
        {
            Debug.Assert(_swingingDoorCompression is not null);

            ref SequentialEnumerableIterator? cache = ref _swingingDoorCompression._cachedSequentialEnumerableIterator;
            Interlocked.CompareExchange(ref cache, this, null);

            base.DisposeCore();
        }
    }
}
