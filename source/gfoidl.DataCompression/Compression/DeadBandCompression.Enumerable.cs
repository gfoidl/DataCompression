using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using gfoidl.DataCompression.Builders;

namespace gfoidl.DataCompression
{
    public partial class DeadBandCompression
    {
        private sealed class EnumerableIterator : DeadBandCompressionEnumerableIterator
        {
            private readonly IEnumerable<DataPoint> _source;
            private readonly IEnumerator<DataPoint> _enumerator;
            //-----------------------------------------------------------------
            public EnumerableIterator(
                DeadBandCompression deadBandCompression,
                IEnumerable<DataPoint> source,
                IEnumerator<DataPoint>? enumerator = null)
                : base(deadBandCompression)
            {
                _source     = source;
                _enumerator = enumerator ?? source.GetEnumerator();
            }
            //-----------------------------------------------------------------
            public override DataPointIterator Clone() => new EnumerableIterator(_deadBandCompression, _source, _enumerator);
            //-----------------------------------------------------------------
            public override bool MoveNext()
            {
                switch (_state)
                {
                    case 0:
                        if (!_enumerator.MoveNext()) return false;
                        _snapShot     = _enumerator.Current;
                        _lastArchived = _snapShot;
                        _incoming     = _snapShot;          // sentinel, nullable would be possible but to much work around
                        _current      = _snapShot;
                        this.GetBounding(_snapShot);
                        _state        = 1;
                        return true;
                    case 1:
                        while (_enumerator.MoveNext())
                        {
                            _incoming       = _enumerator.Current;
                            ref var archive = ref this.IsPointToArchive(_incoming);

                            if (!archive.Archive)
                            {
                                _snapShot = _incoming;
                                continue;
                            }

                            if (!archive.MaxDelta && _lastArchived != _snapShot)
                            {
                                _current = _snapShot;
                                _state   = 2;
                                return true;
                            }

                            goto case 2;
                        }

                        _state = -1;
                        if (_incoming != _lastArchived)     // sentinel-check
                        {
                            _current = _incoming;
                            return true;
                        }
                        return false;
                    case 2:
                        _current = _incoming;
                        _state   = 1;
                        this.UpdatePoints(_incoming, ref _snapShot);
                        return true;
                    case InitialState:
                        ThrowHelper.ThrowInvalidOperation(ThrowHelper.ExceptionResource.GetEnumerator_must_be_called_first);
                        return false;
                    case DisposedState:
                        ThrowHelper.ThrowIfDisposed(ThrowHelper.ExceptionArgument.iterator);
                        return false;
                    default:
                        this.Dispose();
                        return false;
                }
            }
            //-----------------------------------------------------------------
            public override DataPoint[] ToArray()
            {
                IEnumerator<DataPoint> enumerator = _source.GetEnumerator();
                var arrayBuilder                  = new ArrayBuilder<DataPoint>(true);
                this.BuildCollection(enumerator, ref arrayBuilder);

                return arrayBuilder.ToArray();
            }
            //---------------------------------------------------------------------
            public override List<DataPoint> ToList()
            {
                IEnumerator<DataPoint> enumerator = _source.GetEnumerator();
                var listBuilder                   = new ListBuilder<DataPoint>(true);
                this.BuildCollection(enumerator, ref listBuilder);

                return listBuilder.ToList();
            }
            //---------------------------------------------------------------------
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
                    ref var archive = ref this.IsPointToArchive(incoming);

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
            //---------------------------------------------------------------------
#if NETSTANDARD2_1
            public override ValueTask<bool> MoveNextAsync()          => throw new NotSupportedException();
            public override ValueTask<DataPoint[]> ToArrayAsync()    => throw new NotSupportedException();
            public override ValueTask<List<DataPoint>> ToListAsync() => throw new NotSupportedException();
#endif
        }
    }
}
