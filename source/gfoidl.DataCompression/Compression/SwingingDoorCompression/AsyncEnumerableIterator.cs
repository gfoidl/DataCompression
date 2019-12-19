using System;
using System.Collections.Generic;
using System.Threading;

namespace gfoidl.DataCompression.Internal.SwingingDoor
{
    internal sealed class AsyncEnumerableIterator : SwingingDoorCompressionIterator
    {
        public AsyncEnumerableIterator(
            SwingingDoorCompression      swingingDoorCompression,
            IAsyncEnumerable<DataPoint>  source,
            IAsyncEnumerator<DataPoint>? enumerator        = null,
            CancellationToken            cancellationToken = default)
            : base(swingingDoorCompression)
        {
            if (cancellationToken != default) _cancellationToken = cancellationToken;
            _asyncSource     = source;
            _asyncEnumerator = enumerator ?? source.GetAsyncEnumerator(_cancellationToken);
        }
        //---------------------------------------------------------------------
        public override DataPointIterator Clone() => throw new NotSupportedException();
        public override bool MoveNext()           => throw new NotSupportedException();
        public override DataPoint[] ToArray()     => throw new NotSupportedException();
        public override List<DataPoint> ToList()  => throw new NotSupportedException();
    }
}
