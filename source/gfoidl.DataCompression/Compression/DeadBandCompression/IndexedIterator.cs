using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using gfoidl.DataCompression.Builders;

namespace gfoidl.DataCompression.Internal.DeadBand
{
    internal sealed class IndexedIterator<TList> : DeadBandCompressionIterator
        where TList : IList<DataPoint>
    {
        private readonly TList _list;
        private int            _snapShotIndex;
        private int            _lastArchivedIndex;
        private int            _incomingIndex;
        //-----------------------------------------------------------------
        public IndexedIterator(DeadBandCompression deadBandCompression, TList source)
            : base(deadBandCompression)
            => _list = source;
        //-----------------------------------------------------------------
        public override DataPointIterator Clone() => new IndexedIterator<TList>(_deadBandCompression, _list);
        //-----------------------------------------------------------------
        public override bool MoveNext()
        {
            switch (_state)
            {
                case 0:
                    _snapShotIndex     = 0;
                    _lastArchivedIndex = 0;
                    _incomingIndex     = 0;
                    _current           = _list[0];

                    if (_list.Count < 2)
                    {
                        _state = -1;
                        return true;
                    }

                    this.GetBounding(_current);
                    _state         = 1;
                    _incomingIndex = 1;
                    return true;
                case 1:
                    TList source      = _list;
                    int snapShotIndex = _snapShotIndex;
                    int incomingIndex = _incomingIndex;

                    while (true)
                    {
                        // Actually a while loop, but so the range check can be eliminated
                        // https://github.com/dotnet/coreclr/issues/15476
                        if ((uint)incomingIndex >= (uint)source.Count || (uint)snapShotIndex >= (uint)source.Count)
                            break;

                        ref var archive = ref this.IsPointToArchive(incomingIndex);

                        if (!archive.Archive)
                        {
                            snapShotIndex = incomingIndex++;
                            continue;
                        }

                        if (!archive.MaxDelta && _lastArchivedIndex != snapShotIndex)
                        {
                            _current       = source[snapShotIndex];
                            _state         = 2;
                            _snapShotIndex = snapShotIndex;
                            _incomingIndex = incomingIndex;
                            return true;
                        }

                        _snapShotIndex = snapShotIndex;
                        _incomingIndex = incomingIndex;
                        goto case 2;
                    }

                    _state = -1;
                    if (incomingIndex - 1 != _lastArchivedIndex)   // sentinel-check
                    {
                        _current = source[incomingIndex - 1];
                        return true;
                    }
                    return false;
                case 2:
                    _current = _list[_incomingIndex];
                    _state   = 1;
                    this.UpdatePoints(_incomingIndex, _current, ref _snapShotIndex);
                    _incomingIndex++;
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
            TList source = _list;

            Debug.Assert(source.Count > 0);
            if (source.Count == 1 && 0 < (uint)source.Count)
                return new[] { source[0] };

            var arrayBuilder = new ArrayBuilder<DataPoint>(true);
            this.BuildCollection(source, ref arrayBuilder);

            return arrayBuilder.ToArray();
        }
        //-----------------------------------------------------------------
        public override List<DataPoint> ToList()
        {
            TList source = _list;

            Debug.Assert(source.Count > 0);
            if (source.Count == 1 && 0 < (uint)source.Count)
                return new List<DataPoint> { source[0] };

            var listBuilder = new ListBuilder<DataPoint>(true);
            this.BuildCollection(source, ref listBuilder);

            return listBuilder.ToList();
        }
        //-----------------------------------------------------------------
        private void BuildCollection<TBuilder>(TList source, ref TBuilder builder)
            where TBuilder : ICollectionBuilder<DataPoint>
        {
            int snapShotIndex = 0;

            if ((uint)snapShotIndex >= (uint)source.Count) return;

            DataPoint snapShot = source[snapShotIndex];
            builder.Add(snapShot);
            this.GetBounding(snapShot);

            int incomingIndex = 1;
            for (; incomingIndex < source.Count; ++incomingIndex)
            {
                ref var archive = ref this.IsPointToArchive(incomingIndex);

                if (!archive.Archive)
                {
                    snapShotIndex = incomingIndex;
                    continue;
                }

                if (!archive.MaxDelta && (uint)snapShotIndex < (uint)source.Count)
                    builder.Add(source[snapShotIndex]);

                builder.Add(source[incomingIndex]);
                this.UpdatePoints(incomingIndex, source[incomingIndex], ref snapShotIndex);
            }

            incomingIndex--;
            if ((uint)incomingIndex < (uint)source.Count)
                builder.Add(source[incomingIndex]);
        }
        //-----------------------------------------------------------------
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ref (bool Archive, bool MaxDelta) IsPointToArchive(int incomingIndex)
        {
            TList source     = _list;
            int lastArchived = _lastArchivedIndex;

            if ((uint)incomingIndex >= (uint)source.Count || (uint)lastArchived >= (uint)source.Count)
                ThrowHelper.ThrowInvalidOperation(ThrowHelper.ExceptionResource.Should_not_happen);

            double lastArchivedX = source[lastArchived].X;
            DataPoint incoming   = source[incomingIndex];

            ref (bool Archive, bool MaxDelta) archive = ref _archive;

            if (!this.IsMaxDeltaX(ref archive, incoming.X, lastArchivedX))
            {
                archive.Archive = incoming.Y < _bounding.Min || _bounding.Max < incoming.Y;
            }

            return ref archive;
        }
        //-----------------------------------------------------------------
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdatePoints(int incomingIndex, in DataPoint incoming, ref int snapShotIndex)
        {
            snapShotIndex      = incomingIndex;
            _lastArchivedIndex = incomingIndex;

            if (!_archive.MaxDelta) this.GetBounding(incoming);
        }
        //---------------------------------------------------------------------
        protected override void Init(in DataPoint incoming, ref DataPoint snapShot)                                             => throw new NotSupportedException();
        protected override ref (bool Archive, bool MaxDelta) IsPointToArchive(in DataPoint incoming, in DataPoint lastArchived) => throw new NotSupportedException();
        //---------------------------------------------------------------------
#if NETSTANDARD2_1
        public override ValueTask<bool> MoveNextAsync()          => throw new NotSupportedException();
        public override ValueTask<DataPoint[]> ToArrayAsync()    => throw new NotSupportedException();
        public override ValueTask<List<DataPoint>> ToListAsync() => throw new NotSupportedException();
#endif
    }
}
