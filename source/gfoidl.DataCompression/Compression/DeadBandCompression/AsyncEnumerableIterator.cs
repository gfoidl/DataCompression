using System;
using System.Collections.Generic;

namespace gfoidl.DataCompression.Internal.DeadBand
{
    internal sealed class AsyncEnumerableIterator : DeadBandCompressionIterator
    {
        public AsyncEnumerableIterator(DeadBandCompression deadBandCompression, IAsyncEnumerable<DataPoint> source)
            : base(deadBandCompression)
            => _asyncSource = source;
        //---------------------------------------------------------------------
        public override DataPointIterator Clone() => throw new NotSupportedException();
        public override bool MoveNext()           => throw new NotSupportedException();
        public override DataPoint[] ToArray()     => throw new NotSupportedException();
        public override List<DataPoint> ToList()  => throw new NotSupportedException();
    }
}
