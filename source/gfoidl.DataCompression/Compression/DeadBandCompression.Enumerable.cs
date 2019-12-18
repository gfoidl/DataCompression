using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using gfoidl.DataCompression.Builders;

namespace gfoidl.DataCompression
{
    public partial class DeadBandCompression
    {
        private sealed class EnumerableIterator : DeadBandCompressionEnumerableIterator
        {
            public EnumerableIterator(
                DeadBandCompression deadBandCompression,
                IEnumerable<DataPoint>? source,
                IEnumerator<DataPoint>? enumerator = null)
                : base(deadBandCompression)
            {
                _source     = source     ?? throw new ArgumentNullException(nameof(source));
                _enumerator = enumerator ?? source.GetEnumerator();
            }
            //-----------------------------------------------------------------
            public override DataPointIterator Clone() => new EnumerableIterator(_deadBandCompression, _source, _enumerator);
            //-----------------------------------------------------------------
            public override DataPoint[] ToArray()
            {
                Debug.Assert(_source != null);

                IEnumerator<DataPoint> enumerator = _source.GetEnumerator();
                var arrayBuilder                  = new ArrayBuilder<DataPoint>(true);
                this.BuildCollection(enumerator, ref arrayBuilder);

                return arrayBuilder.ToArray();
            }
            //-----------------------------------------------------------------
            public override List<DataPoint> ToList()
            {
                Debug.Assert(_source != null);

                IEnumerator<DataPoint> enumerator = _source.GetEnumerator();
                var listBuilder                   = new ListBuilder<DataPoint>(true);
                this.BuildCollection(enumerator, ref listBuilder);

                return listBuilder.ToList();
            }
            //-----------------------------------------------------------------
            public void BuildCollection<TBuilder>(IEnumerator<DataPoint> enumerator, ref TBuilder builder)
                where TBuilder : ICollectionBuilder<DataPoint>
            {
                if (!enumerator.MoveNext()) return;

                DataPoint snapShot = enumerator.Current;
                _lastArchived      = snapShot;
                DataPoint incoming = snapShot;          // sentinel, nullable would be possible but to much work around

                builder.Add(snapShot);
                this.GetBounding(snapShot);

                while (enumerator.MoveNext())
                {
                    incoming        = enumerator.Current;
                    ref var archive = ref this.IsPointToArchive(incoming, _lastArchived);

                    if (!archive.Archive)
                    {
                        snapShot = incoming;
                        continue;
                    }

                    if (!archive.MaxDelta && _lastArchived != snapShot)
                        builder.Add(snapShot);

                    builder.Add(incoming);
                    this.UpdatePoints(incoming, ref snapShot);
                }

                if (incoming != _lastArchived)          // sentinel-check
                    builder.Add(incoming);
            }
            //-----------------------------------------------------------------
            protected override void Init(in DataPoint incoming, ref DataPoint snapShot) => this.UpdatePoints(incoming, ref snapShot);
            //-----------------------------------------------------------------
#if NETSTANDARD2_1
            public override ValueTask<bool> MoveNextAsync()          => throw new NotSupportedException();
            public override ValueTask<DataPoint[]> ToArrayAsync()    => throw new NotSupportedException();
            public override ValueTask<List<DataPoint>> ToListAsync() => throw new NotSupportedException();
#endif
        }
    }
}
