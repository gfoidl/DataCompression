using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace gfoidl.DataCompression.Internal.DeadBand
{
    internal sealed class AsyncEnumerableIterator : DeadBandCompressionIterator
    {
        public void SetData(DeadBandCompression deadBandCompression, IAsyncEnumerable<DataPoint> source)
        {
            this.SetData(deadBandCompression as Compression, source);
            _deadBandCompression = deadBandCompression;
        }
        //---------------------------------------------------------------------
        public override DataPointIterator Clone() => throw new NotSupportedException();
        public override bool MoveNext()           => throw new NotSupportedException();
        public override DataPoint[] ToArray()     => throw new NotSupportedException();
        public override List<DataPoint> ToList()  => throw new NotSupportedException();
        //---------------------------------------------------------------------
        protected override void DisposeCore()
        {
            Debug.Assert(_deadBandCompression is not null);

            ref AsyncEnumerableIterator? cache = ref _deadBandCompression._cachedAsyncEnumerableIterator;
            Interlocked.CompareExchange(ref cache, this, null);

            base.DisposeCore();
        }
    }
}
