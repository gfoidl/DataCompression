// (c) gfoidl, all rights reserved

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace gfoidl.DataCompression.Internal.SwingingDoor
{
    internal sealed class IndexedIterator<TList> : SwingingDoorCompressionIterator
        where TList : IList<DataPoint>
    {
        private readonly DataPointIndexedIterator<TList> _inner = new();
        private TList? _list;
        //---------------------------------------------------------------------
        public void SetData(SwingingDoorCompression swingingDoorCompression, TList source)
        {
            Debug.Assert(source is not null);

            this.SetData(swingingDoorCompression);
            _swingingDoorCompression = swingingDoorCompression;
            _list                    = source;

            _inner.SetData(swingingDoorCompression, this, source);
        }
        //---------------------------------------------------------------------
        public override DataPointIterator Clone()
        {
            Debug.Assert(_swingingDoorCompression is not null);
            Debug.Assert(_list                    is not null);

            IndexedIterator<TList> clone = new();
            clone.SetData(_swingingDoorCompression, _list);

            return clone;
        }
        //---------------------------------------------------------------------
        public override DataPointIterator GetEnumerator() => _inner!.GetEnumerator();
        public override DataPoint[] ToArray()             => _inner!.ToArray();
        public override List<DataPoint> ToList()          => _inner!.ToList();
        public override bool MoveNext()                   => throw new InvalidOperationException("Should operate on _inner");
        //---------------------------------------------------------------------
        protected internal override void Init(int incomingIndex, in DataPoint incoming, ref int snapShotIndex) => this.OpenNewDoor();
        //---------------------------------------------------------------------
#if NETSTANDARD2_1
        public override ValueTask<DataPoint[]> ToArrayAsync(CancellationToken ct)    => throw new NotSupportedException();
        public override ValueTask<List<DataPoint>> ToListAsync(CancellationToken ct) => throw new NotSupportedException();
#endif
        //---------------------------------------------------------------------
        protected override void DisposeCore()
        {
            Debug.Assert(_swingingDoorCompression is not null);

            //_inner?.Dispose();        !!! don't dispose _inner, as _inner disposes this instance

            _list = default;

            ref DataPointIterator? cache = ref _swingingDoorCompression._cachedIndexedIterator;
            Interlocked.CompareExchange(ref cache, this, null);

            base.DisposeCore();
        }
    }
}
