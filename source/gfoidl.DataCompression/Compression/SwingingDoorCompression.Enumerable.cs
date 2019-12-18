using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using gfoidl.DataCompression.Builders;

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
            public override bool MoveNext()
            {
                if (_state == 2 && _swingingDoorCompression._minDeltaXHasValue)
                {
                    this.SkipMinDeltaX(_snapShot);
                }

                return base.MoveNext();
            }
            //-----------------------------------------------------------------
            public override DataPoint[] ToArray()
            {
                Debug.Assert(_source != null);

                IEnumerator<DataPoint> enumerator = _source.GetEnumerator();

                var arrayBuilder = new ArrayBuilder<DataPoint>(true);
                this.BuildCollection(enumerator, ref arrayBuilder);

                return arrayBuilder.ToArray();
            }
            //-----------------------------------------------------------------
            public override List<DataPoint> ToList()
            {
                Debug.Assert(_source != null);

                IEnumerator<DataPoint> enumerator = _source.GetEnumerator();

                var listBuilder = new ListBuilder<DataPoint>(true);
                this.BuildCollection(enumerator, ref listBuilder);

                return listBuilder.ToList();
            }
            //-----------------------------------------------------------------
            private void BuildCollection<TBuilder>(IEnumerator<DataPoint> enumerator, ref TBuilder builder)
                where TBuilder : ICollectionBuilder<DataPoint>
            {
                if (!enumerator.MoveNext()) return;

                DataPoint snapShot = enumerator.Current;
                _lastArchived      = snapShot;
                DataPoint incoming = snapShot;          // sentinel, nullable would be possible but to much work around

                builder.Add(snapShot);
                this.OpenNewDoor(snapShot);

                while (enumerator.MoveNext())
                {
                    incoming        = enumerator.Current;
                    this.IsPointToArchive(incoming, _lastArchived);
                    ref var archive = ref _archive;

                    if (!archive.Archive)
                    {
                        this.CloseTheDoor(incoming, _lastArchived);
                        snapShot = incoming;
                        continue;
                    }

                    if (!archive.MaxDelta && _lastArchived != snapShot)
                        builder.Add(snapShot);

                    if (_swingingDoorCompression._minDeltaXHasValue)
                    {
                        this.SkipMinDeltaX(snapShot);
                        incoming = _incoming;
                    }

                    builder.Add(incoming);
                    this.OpenNewDoor(incoming);
                }

                if (incoming != _lastArchived)          // sentinel-check
                    builder.Add(incoming);
            }
            //-----------------------------------------------------------------
            [MethodImpl(MethodImplOptions.NoInlining)]
            private void SkipMinDeltaX(in DataPoint snapShot)
            {
                Debug.Assert(_enumerator != null);

                double snapShot_x = snapShot.X;
                double minDeltaX  = _swingingDoorCompression._minDeltaX;

                while (_enumerator.MoveNext())
                {
                    DataPoint tmp = _enumerator.Current;

                    if ((tmp.X - snapShot_x) > minDeltaX)
                    {
                        _incoming = tmp;
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
