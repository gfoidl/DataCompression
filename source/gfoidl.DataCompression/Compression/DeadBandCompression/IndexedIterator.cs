// (c) gfoidl, all rights reserved

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace gfoidl.DataCompression.Internal.DeadBand
{
    internal sealed class IndexedIterator<TList> : DeadBandCompressionIterator
        where TList : IList<DataPoint>
    {
        private readonly DataPointIndexedIterator<TList> _inner = new();
        private TList? _list;
        //---------------------------------------------------------------------
        public void SetData(DeadBandCompression deadBandCompression, TList source)
        {
            Debug.Assert(source is not null);

            this.SetData(deadBandCompression);
            _deadBandCompression = deadBandCompression;
            _list                = source;

            _inner.SetData(deadBandCompression, this, source);
        }
        //---------------------------------------------------------------------
        public override DataPointIterator Clone()
        {
            Debug.Assert(_deadBandCompression is not null);
            Debug.Assert(_list                is not null);

            IndexedIterator<TList> clone = new();
            clone.SetData(_deadBandCompression, _list);

            return clone;
        }
        //---------------------------------------------------------------------
        public override DataPointIterator GetEnumerator() => _inner!.GetEnumerator();
        public override DataPoint[] ToArray()             => _inner!.ToArray();
        public override List<DataPoint> ToList()          => _inner!.ToList();
        public override bool MoveNext()                   => throw new InvalidOperationException("Should operate on _inner");
        //---------------------------------------------------------------------
#if NETSTANDARD2_1
        public override ValueTask<DataPoint[]> ToArrayAsync(CancellationToken ct)    => throw new NotSupportedException();
        public override ValueTask<List<DataPoint>> ToListAsync(CancellationToken ct) => throw new NotSupportedException();
#endif
        //---------------------------------------------------------------------
        protected override void DisposeCore()
        {
            Debug.Assert(_deadBandCompression is not null);

            //_inner?.Dispose();        !!! don't dispose _inner, as _inner disposes this instance

            _list = default;

            ref DataPointIterator? cache = ref _deadBandCompression._cachedIndexedIterator;
            Interlocked.CompareExchange(ref cache, this, null);

            base.DisposeCore();
        }
    }
}
