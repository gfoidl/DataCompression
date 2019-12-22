using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace gfoidl.DataCompression.Internal.SwingingDoor
{
    internal sealed class SequentialEnumerableIterator : SwingingDoorCompressionIterator
    {
        public SequentialEnumerableIterator(
            SwingingDoorCompression swingingDoorCompression,
            IEnumerable<DataPoint>? source,
            IEnumerator<DataPoint>? enumerator = null)
            : base(swingingDoorCompression)
        {
            _source     = source     ?? throw new ArgumentNullException(nameof(source));
            _enumerator = enumerator ?? source.GetEnumerator();
        }
        //---------------------------------------------------------------------
        public override DataPointIterator Clone() => new SequentialEnumerableIterator(_swingingDoorCompression, _source, _enumerator);
        //---------------------------------------------------------------------
#if NETSTANDARD2_1
        public override ValueTask<DataPoint[]> ToArrayAsync(CancellationToken ct)    => throw new NotSupportedException();
        public override ValueTask<List<DataPoint>> ToListAsync(CancellationToken ct) => throw new NotSupportedException();
#endif
    }
}
