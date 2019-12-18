using System;
using System.Collections.Generic;
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
        //-----------------------------------------------------------------
        public override DataPointIterator Clone() => new SequentialEnumerableIterator(_swingingDoorCompression, _source, _enumerator);
        //-----------------------------------------------------------------
#if NETSTANDARD2_1
        public override ValueTask<bool> MoveNextAsync()          => throw new NotSupportedException();
        public override ValueTask<DataPoint[]> ToArrayAsync()    => throw new NotSupportedException();
        public override ValueTask<List<DataPoint>> ToListAsync() => throw new NotSupportedException();
#endif
    }
}
