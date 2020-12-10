using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace gfoidl.DataCompression.Internal.SwingingDoor
{
    internal sealed class IndexedIterator<TList> : SwingingDoorCompressionIterator
        where TList : IList<DataPoint>
    {
        private TList?                           _list;
        private DataPointIndexedIterator<TList>? _inner;
        //---------------------------------------------------------------------
        public void SetData(SwingingDoorCompression swingingDoorCompression, TList source)
        {
            Debug.Assert(source is not null);

            this.SetData(swingingDoorCompression);
            _swingingDoorCompression = swingingDoorCompression;
            _list                    = source;

            // It's a best effort solution, but we may miss a cached item -- we don't care ;-)
            ref DataPointIterator? cached = ref swingingDoorCompression._cachedIndexedIterator;
            DataPointIterator? iter       = Interlocked.Exchange(ref cached, null);

            if (iter is DataPointIndexedIterator<TList> inner)
            {
                _inner = inner;
            }
            else
            {
                Interlocked.CompareExchange(ref cached, iter, null);
                _inner = new DataPointIndexedIterator<TList>();
            }

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OpenNewDoor(int incomingIndex, in DataPoint incoming, ref int snapShotIndex)
        {
            snapShotIndex = incomingIndex;

            this.OpenNewDoor(incoming);
        }
        //---------------------------------------------------------------------
        protected internal override void Init(int incomingIndex, in DataPoint incoming, ref int snapShotIndex) => this.OpenNewDoor(incomingIndex, incoming, ref snapShotIndex);
        protected internal override void Init(in DataPoint incoming, ref DataPoint snapShot)                   => throw new NotSupportedException();
        //---------------------------------------------------------------------
#if NETSTANDARD2_1
        public override ValueTask<DataPoint[]> ToArrayAsync(CancellationToken ct)    => throw new NotSupportedException();
        public override ValueTask<List<DataPoint>> ToListAsync(CancellationToken ct) => throw new NotSupportedException();
#endif
        //---------------------------------------------------------------------
        protected override void DisposeCore()
        {
            Debug.Assert(_swingingDoorCompression is not null);

            _list = default;
            _inner?.Dispose();

            ref DataPointIterator? cache = ref _swingingDoorCompression._cachedIndexedIterator;
            Interlocked.CompareExchange(ref cache, this, null);

            base.DisposeCore();
        }
    }
}
