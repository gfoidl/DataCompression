using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace gfoidl.DataCompression.Internal.DeadBand
{
    internal sealed class IndexedIterator<TList> : DeadBandCompressionIterator
        where TList : IList<DataPoint>
    {
        private readonly TList                           _list;
        private readonly DataPointIndexedIterator<TList> _inner;
        //---------------------------------------------------------------------
        public IndexedIterator(DeadBandCompression deadBandCompression, TList source)
            : base(deadBandCompression)
        {
            _list  = source;
            _inner = new DataPointIndexedIterator<TList>(deadBandCompression, this, source);
        }
        //---------------------------------------------------------------------
        public override DataPointIterator Clone()         => new IndexedIterator<TList>(_deadBandCompression, _list);
        public override DataPointIterator GetEnumerator() => _inner.GetEnumerator();
        public override DataPoint[] ToArray()             => _inner.ToArray();
        public override List<DataPoint> ToList()          => _inner.ToList();
        public override bool MoveNext()                   => throw new InvalidOperationException("Should operate on _inner");
        //---------------------------------------------------------------------
        public override void Dispose()
        {
            base.Dispose();
            _inner.Dispose();
        }
        //---------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdatePoints(int incomingIndex, in DataPoint incoming, ref int snapShotIndex)
        {
            snapShotIndex = incomingIndex;

            if (!_archive.MaxDelta) this.GetBounding(incoming);
        }
        //---------------------------------------------------------------------
        protected internal override void Init(int incomingIndex, in DataPoint incoming, ref int snapShotIndex) => this.UpdatePoints(incomingIndex, incoming, ref snapShotIndex);
        protected internal override void Init(in DataPoint incoming, ref DataPoint snapShot)                   => throw new NotSupportedException();
        //---------------------------------------------------------------------
#if NETSTANDARD2_1
        public override ValueTask<bool> MoveNextAsync()          => throw new NotSupportedException();
        public override ValueTask<DataPoint[]> ToArrayAsync()    => throw new NotSupportedException();
        public override ValueTask<List<DataPoint>> ToListAsync() => throw new NotSupportedException();
#endif
    }
}
