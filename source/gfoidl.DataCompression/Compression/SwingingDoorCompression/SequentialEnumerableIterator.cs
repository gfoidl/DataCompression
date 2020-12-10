using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace gfoidl.DataCompression.Internal.SwingingDoor
{
    internal sealed class SequentialEnumerableIterator : SwingingDoorCompressionIterator
    {
        public SequentialEnumerableIterator(SwingingDoorCompression swingingDoorCompression, IEnumerable<DataPoint>? source)
            : base(swingingDoorCompression)
        {
            _source     = source ?? throw new ArgumentNullException(nameof(source));
            _enumerator = source.GetEnumerator();
        }
        //---------------------------------------------------------------------
        public override DataPointIterator Clone() => new SequentialEnumerableIterator(_swingingDoorCompression!, _source);
        //---------------------------------------------------------------------
#if NETSTANDARD2_1
        public override ValueTask<DataPoint[]> ToArrayAsync(CancellationToken ct)    => throw new NotSupportedException();
        public override ValueTask<List<DataPoint>> ToListAsync(CancellationToken ct) => throw new NotSupportedException();
#endif
    }
}
