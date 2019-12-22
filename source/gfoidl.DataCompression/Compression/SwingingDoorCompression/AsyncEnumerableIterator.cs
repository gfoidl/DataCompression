using System;
using System.Collections.Generic;

namespace gfoidl.DataCompression.Internal.SwingingDoor
{
    internal sealed class AsyncEnumerableIterator : SwingingDoorCompressionIterator
    {
        public AsyncEnumerableIterator(SwingingDoorCompression swingingDoorCompression, IAsyncEnumerable<DataPoint> source)
            : base(swingingDoorCompression)
            => _asyncSource = source;
        //---------------------------------------------------------------------
        public override DataPointIterator Clone() => throw new NotSupportedException();
        public override bool MoveNext()           => throw new NotSupportedException();
        public override DataPoint[] ToArray()     => throw new NotSupportedException();
        public override List<DataPoint> ToList()  => throw new NotSupportedException();
    }
}
