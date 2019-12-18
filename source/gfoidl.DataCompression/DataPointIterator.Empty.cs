using System;
using System.Collections.Generic;

namespace gfoidl.DataCompression
{
    public abstract partial class DataPointIterator
    {
        /// <summary>
        /// An <see cref="DataPointIterator" /> that represents an empty 
        /// collection.
        /// </summary>
        private sealed partial class EmptyIterator : DataPointIterator
        {
            public override DataPointIterator Clone() => this;
            public override bool MoveNext()           => false;
            public override DataPoint[] ToArray()     => Array.Empty<DataPoint>();
            public override List<DataPoint> ToList()  => new List<DataPoint>();
            //---------------------------------------------------------------------
            protected override void Init(in DataPoint incoming, ref DataPoint snapShot)                                             => throw new NotSupportedException();
            protected override ref (bool Archive, bool MaxDelta) IsPointToArchive(in DataPoint incoming, in DataPoint lastArchived) => throw new NotSupportedException();
        }
    }
}
