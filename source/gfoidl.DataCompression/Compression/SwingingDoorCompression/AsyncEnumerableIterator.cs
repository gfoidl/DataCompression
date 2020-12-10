using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace gfoidl.DataCompression.Internal.SwingingDoor
{
    internal sealed class AsyncEnumerableIterator : SwingingDoorCompressionIterator
    {
        public void SetData(SwingingDoorCompression swingingDoorCompression, IAsyncEnumerable<DataPoint> source)
        {
            this.SetData(swingingDoorCompression as Compression, source);
            _swingingDoorCompression = swingingDoorCompression;
        }
        //---------------------------------------------------------------------
        public override DataPointIterator Clone() => throw new NotSupportedException();
        public override bool MoveNext()           => throw new NotSupportedException();
        public override DataPoint[] ToArray()     => throw new NotSupportedException();
        public override List<DataPoint> ToList()  => throw new NotSupportedException();
        //---------------------------------------------------------------------
        protected override void DisposeCore()
        {
            Debug.Assert(_swingingDoorCompression is not null);

            ref AsyncEnumerableIterator? cache = ref _swingingDoorCompression._cachedAsyncEnumerableIterator;
            Interlocked.CompareExchange(ref cache, this, null);

            base.DisposeCore();
        }
    }
}
