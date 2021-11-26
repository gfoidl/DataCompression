// (c) gfoidl, all rights reserved

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace gfoidl.DataCompression.Internal.DeadBand
{
    internal sealed class SequentialEnumerableIterator : DeadBandCompressionIterator
    {
        public void SetData(DeadBandCompression deadBandCompression, IEnumerable<DataPoint> source)
        {
            this.SetData(deadBandCompression as Compression, source);
            _deadBandCompression = deadBandCompression;
        }
        //---------------------------------------------------------------------
        public override DataPointIterator Clone()
        {
            Debug.Assert(_deadBandCompression is not null);
            Debug.Assert(_source              is not null);

            SequentialEnumerableIterator clone = new();
            clone.SetData(_deadBandCompression, _source);

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
            Debug.Assert(_deadBandCompression is not null);

            ref SequentialEnumerableIterator? cache = ref _deadBandCompression._cachedSequentialEnumerableIterator;
            Interlocked.CompareExchange(ref cache, this, null);

            base.DisposeCore();
        }
    }
}
