using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace gfoidl.DataCompression
{
    internal sealed class EmptyDataPointIterator : DataPointIterator
    {
        public override DataPointIterator Clone() => this;
        public override bool MoveNext()           => false;
        public override DataPoint[] ToArray()     => Array.Empty<DataPoint>();
        public override List<DataPoint> ToList()  => new List<DataPoint>();
        //---------------------------------------------------------------------
#if NETSTANDARD2_1
        public override ValueTask<DataPoint[]> ToArrayAsync(CancellationToken ct)    => new ValueTask<DataPoint[]>(Array.Empty<DataPoint>());
        public override ValueTask<List<DataPoint>> ToListAsync(CancellationToken ct) => new ValueTask<List<DataPoint>>(new List<DataPoint>());
#endif
        //---------------------------------------------------------------------
        protected internal override void Init(in DataPoint incoming, ref DataPoint snapShot)                                             => throw new NotSupportedException();
        protected internal override void Init(int incomingIndex, in DataPoint incoming, ref int snapShotIndex)                           => throw new NotSupportedException();
        protected internal override ref (bool Archive, bool MaxDelta) IsPointToArchive(in DataPoint incoming, in DataPoint lastArchived) => throw new NotSupportedException();
    }
}
