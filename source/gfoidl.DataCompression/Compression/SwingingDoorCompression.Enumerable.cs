using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace gfoidl.DataCompression
{
    public partial class SwingingDoorCompression
    {
        private sealed class EnumerableIterator : SwingingDoorCompressionEnumerableIterator
        {
            public EnumerableIterator(
                SwingingDoorCompression swingingDoorCompression,
                IEnumerable<DataPoint>? source,
                IEnumerator<DataPoint>? enumerator = null)
                : base(swingingDoorCompression)
            {
                _source     = source     ?? throw new ArgumentNullException(nameof(source));
                _enumerator = enumerator ?? source.GetEnumerator();
            }
            //-----------------------------------------------------------------
            public override DataPointIterator Clone() => new EnumerableIterator(_swingingDoorCompression, _source, _enumerator);
            //-----------------------------------------------------------------
            protected override ref DataPoint HandleSpecialCaseAfterArchivedPoint(IEnumerator<DataPoint> enumerator, ref DataPoint incoming, in DataPoint snapShot)
            {
                if (_swingingDoorCompression._minDeltaXHasValue)
                {
                    this.SkipMinDeltaX(enumerator, ref incoming, snapShot);
                }

                return ref incoming;
            }
            //-----------------------------------------------------------------
            [MethodImpl(MethodImplOptions.NoInlining)]
            private void SkipMinDeltaX(IEnumerator<DataPoint> enumerator, ref DataPoint incoming, in DataPoint snapShot)
            {
                double snapShotX = snapShot.X;
                double minDeltaX = _swingingDoorCompression._minDeltaX;

                while (enumerator.MoveNext())
                {
                    DataPoint tmp = enumerator.Current;

                    if ((tmp.X - snapShotX) > minDeltaX)
                    {
                        incoming = tmp;
                        break;
                    }
                }
            }
            //-----------------------------------------------------------------
            protected override void Init(in DataPoint incoming, ref DataPoint snapShot)             => this.OpenNewDoor(incoming);
            protected override void UpdateFilters(in DataPoint incoming, in DataPoint lastArchived) => this.CloseTheDoor(incoming, lastArchived);
            //-----------------------------------------------------------------
#if NETSTANDARD2_1
            public override ValueTask<bool> MoveNextAsync()          => throw new NotSupportedException();
            public override ValueTask<DataPoint[]> ToArrayAsync()    => throw new NotSupportedException();
            public override ValueTask<List<DataPoint>> ToListAsync() => throw new NotSupportedException();
#endif
        }
    }
}
