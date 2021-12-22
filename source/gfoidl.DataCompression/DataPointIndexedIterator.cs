// (c) gfoidl, all rights reserved

using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using gfoidl.DataCompression.Builders;

namespace gfoidl.DataCompression
{
    internal sealed class DataPointIndexedIterator<TList> : DataPointIterator
        where TList : notnull, IList<DataPoint>
    {
        private DataPointIterator? _wrapperIterator;
        private TList?             _list;
        private int                _snapShotIndex;
        private int                _previousSnapshotIndex;
        private int                _lastArchivedIndex;
        private int                _incomingIndex;
        //---------------------------------------------------------------------
        private int SnapShotIndex
        {
            get => _snapShotIndex;
            set
            {
                _previousSnapshotIndex = _snapShotIndex;
                _snapShotIndex         = value;
            }
        }
        //---------------------------------------------------------------------
        public void SetData(Compression compression, DataPointIterator wrappedIterator, TList source)
        {
            Debug.Assert(wrappedIterator is not null);
            Debug.Assert(source          is not null);

            this.SetData(compression);
            _wrapperIterator = wrappedIterator;
            _list            = source;
        }
        //---------------------------------------------------------------------
        public override DataPointIterator Clone()
        {
            Debug.Assert(_algorithm       is not null);
            Debug.Assert(_wrapperIterator is not null);
            Debug.Assert(_list            is not null);

            DataPointIndexedIterator<TList> clone = new();
            clone.SetData(_algorithm, _wrapperIterator, _list);

            return clone;
        }
        //---------------------------------------------------------------------
        public override bool MoveNext()
        {
            Debug.Assert(_list is not null);
            Debug.Assert(_list.Count > 0, "Case _list.Count == 0 should be handled by EmptyDataPointIterator");

            switch (_state)
            {
                case StartOfDataState:
                {
                    _incoming     = _list[0];
                    _lastArchived = _incoming;

                    if (_list.Count < 2)
                    {
                        _state = EndOfDataState;
                        return true;
                    }

                    this.Init(_incoming);
                    _lastArchivedIndex     = 0;
                    this.SnapShotIndex     = 0;
                    _state                 = IterateState;
                    _incomingIndex         = 1;
                    return true;
                }
                case IterateState:
                {
                    TList source          = _list;
                    int incomingIndex     = _incomingIndex;
                    int lastArchivedIndex = _lastArchivedIndex;

                    while (true)
                    {
                        // Actually a while loop, but so the range check can be eliminated
                        // https://github.com/dotnet/coreclr/issues/15476
                        if ((uint)incomingIndex     >= (uint)source.Count
                         || (uint)lastArchivedIndex >= (uint)source.Count)
                        {
                            break;
                        }

                        _incoming       = source[incomingIndex];
                        _lastArchived   = source[lastArchivedIndex];
                        ref var archive = ref this.IsPointToArchive(_incoming, _lastArchived);

                        if (!archive.Archive)
                        {
                            this.UpdateFilters(_incoming, _lastArchived);
                            this.SnapShotIndex = incomingIndex++;
                            continue;
                        }

                        if (!archive.MaxDelta && _lastArchivedIndex != this.SnapShotIndex && (uint)this.SnapShotIndex < (uint)source.Count)
                        {
                            _lastArchived      = source[this.SnapShotIndex];
                            _lastArchivedIndex = this.SnapShotIndex;
                            this.SnapShotIndex = incomingIndex;
                            _incomingIndex     = incomingIndex;
                            _state             = _stateAfterArchive;
                            return true;
                        }

                        _incomingIndex = incomingIndex;
                        goto case ArchivePointState;
                    }

                    incomingIndex--;
                    if (incomingIndex != _lastArchivedIndex)    // sentinel check
                    {
                        _incomingIndex = incomingIndex;

                        if (_previousSnapshotIndex != _lastArchivedIndex)
                        {
                            // Construct a door from the last archived point to the
                            // incoming (final point), and check whether the penultimate
                            // point is to archive or not.
                            this.Init(_incoming);
                            this.UpdateFilters(_incoming, _lastArchived);
                            _previousSnapShot = _list[_previousSnapshotIndex];
                            ref var archive   = ref this.IsPointToArchive(_previousSnapShot, _lastArchived);

                            if (archive.Archive)
                            {
                                _lastArchived = _previousSnapShot;
                                _state        = EndOfDataState;
                                return true;
                            }
                        }

                        goto case EndOfDataState;
                    }

                    goto default;
                }
                case ArchiveIncomingState:
                {
                    int incomingIndex  = _incomingIndex;
                    _lastArchived      = _list[incomingIndex];
                    _lastArchivedIndex = incomingIndex;
                    _state             = PostArchiveState;
                    return true;
                }
                case PostArchiveState:
                {
                    int incomingIndex = _incomingIndex;
                    this.Init(_incoming);
                    this.UpdateFilters(_incoming, _lastArchived);
                    _incomingIndex = this.HandleSkipMinDeltaX(incomingIndex, this.SnapShotIndex);
                    goto case IterateState;
                }
                case ArchivePointState:
                {
                    int incomingIndex  = _incomingIndex;
                    _lastArchived      = _list[incomingIndex];
                    _lastArchivedIndex = incomingIndex;
                    this.SnapShotIndex = incomingIndex;
                    _state             = IterateState;
                    this.Init(_incoming);
                    incomingIndex      = this.HandleSkipMinDeltaX(incomingIndex, this.SnapShotIndex);
                    _incomingIndex     = incomingIndex + 1;
                    return true;
                }
                case EndOfDataState:
                {
                    if ((uint)_incomingIndex < (uint)_list.Count)
                    {
                        DataPoint incoming = _list[_incomingIndex];
                        if (incoming != _lastArchived)      // sentinel check
                        {
                            _lastArchived = _list[_incomingIndex];
                            _state = FinalState;
                            return true;
                        }
                    }

                    goto default;
                }
                case InitialState:
                {
                    ThrowHelper.ThrowInvalidOperation(ThrowHelper.ExceptionResource.GetEnumerator_must_be_called_first);
                    return false;
                }
                case DisposedState:
                {
                    ThrowHelper.ThrowIfDisposed(ThrowHelper.ExceptionArgument.iterator);
                    return false;
                }
                default:
                    return false;
            }
        }
        //---------------------------------------------------------------------
        public override DataPoint[] ToArray()
        {
            Debug.Assert(_list is not null);

            TList source = _list;

            Debug.Assert(source.Count > 0);
            if (source.Count == 1 && 0 < (uint)source.Count)
                return new[] { source[0] };

            var arrayBuilder = new ArrayBuilder<DataPoint>(true);
            this.BuildCollection(source, ref arrayBuilder);

            return arrayBuilder.ToArray();
        }
        //---------------------------------------------------------------------
        public override List<DataPoint> ToList()
        {
            Debug.Assert(_list is not null);

            TList source = _list!;

            Debug.Assert(source.Count > 0);
            if (source.Count == 1 && 0 < (uint)source.Count)
                return new List<DataPoint> { source[0] };

            var listBuilder = new ListBuilder<DataPoint>(true);
            this.BuildCollection(source, ref listBuilder);

            return listBuilder.ToList();
        }
        //---------------------------------------------------------------------
        protected internal override ref (bool Archive, bool MaxDelta) IsPointToArchive(in DataPoint incoming, in DataPoint lastArchived) => ref _wrapperIterator!.IsPointToArchive(incoming, lastArchived);
        protected internal override void UpdateFilters(in DataPoint incoming, in DataPoint lastArchived)                                 => _wrapperIterator!.UpdateFilters(incoming, lastArchived);
        protected internal override void Init(in DataPoint incoming)                                                                     => _wrapperIterator!.Init(incoming);
        //---------------------------------------------------------------------
        private void BuildCollection<TBuilder>(TList source, ref TBuilder builder)
            where TBuilder : ICollectionBuilder<DataPoint>
        {
            int incomingIndex     = 0;
            this.SnapShotIndex    = 0;
            int lastArchivedIndex = 0;

            if ((uint)incomingIndex >= (uint)source.Count) return;

            DataPoint incoming = source[0];
            _lastArchived      = incoming;
            incomingIndex      = 1;
            builder.Add(incoming);
            this.Init(incoming);

            // Is actually a for loop, but the JIT doesn't elide the bound check
            // due to SkipMinDeltaX. I.e. w/o SkipMinDeltaX the bound check gets
            // eliminated.
            while (true)
            {
                if ((uint)incomingIndex     >= (uint)source.Count
                 || (uint)lastArchivedIndex >= (uint)source.Count)
                {
                    break;
                }

                incoming        = source[incomingIndex];
                _lastArchived   = source[lastArchivedIndex];
                ref var archive = ref this.IsPointToArchive(incoming, _lastArchived);

                if (!archive.Archive)
                {
                    this.UpdateFilters(incoming, _lastArchived);
                    this.SnapShotIndex = incomingIndex++;
                    continue;
                }

                if (!archive.MaxDelta && lastArchivedIndex != this.SnapShotIndex && (uint)this.SnapShotIndex < (uint)source.Count)
                {
                    DataPoint snapShot = source[this.SnapShotIndex];
                    _lastArchived      = snapShot;
                    lastArchivedIndex  = this.SnapShotIndex;
                    this.SnapShotIndex = incomingIndex;
                    builder.Add(snapShot);

                    if (_archiveIncoming)
                    {
                        builder.Add(incoming);
                        _lastArchived     = incoming;
                        lastArchivedIndex = incomingIndex;
                    }

                    this.Init(incoming);
                    this.UpdateFilters(incoming, _lastArchived);
                    incomingIndex++;
                    incomingIndex = this.HandleSkipMinDeltaX(incomingIndex, this.SnapShotIndex);
                    continue;
                }

                _lastArchived      = incoming;
                lastArchivedIndex  = incomingIndex;
                this.SnapShotIndex = incomingIndex;
                builder.Add(incoming);
                this.Init(incoming);
                incomingIndex = this.HandleSkipMinDeltaX(incomingIndex, this.SnapShotIndex);
                incomingIndex++;
            }

            incomingIndex--;
            if (incomingIndex != lastArchivedIndex && (uint)incomingIndex < (uint)source.Count)
            {
                if (_previousSnapshotIndex != lastArchivedIndex && (uint)_previousSnapshotIndex < (uint)source.Count)
                {
                    // Construct a door from the last archived point to the
                    // incoming (final point), and check whether the penultimate
                    // point is to archive or not.
                    this.Init(incoming);
                    this.UpdateFilters(incoming, _lastArchived);
                    _previousSnapShot = source[_previousSnapshotIndex];
                    ref var archive = ref this.IsPointToArchive(_previousSnapShot, _lastArchived);

                    if (archive.Archive)
                    {
                        builder.Add(_previousSnapShot);
                    }
                }

                builder.Add(source[incomingIndex]);
            }
        }
        //---------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int HandleSkipMinDeltaX(int incomingIndex, int snapShotIndex)
        {
            Debug.Assert(_algorithm is not null);

            return _minDeltaX.HasValue
                ? this.SkipMinDeltaX(incomingIndex, snapShotIndex)
                : incomingIndex;
        }
        //---------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.NoInlining)]
        private int SkipMinDeltaX(int incomingIndex, int snapShotIndex)
        {
            Debug.Assert(_list is not null);
            Debug.Assert(_minDeltaX.HasValue);

            TList source = _list;

            if ((uint)snapShotIndex < (uint)source.Count)
            {
                double snapShotX = source[snapShotIndex].X;
                double minDeltaX = _minDeltaX.GetValueOrDefault();

                // A for loop won't elide the bound checks, although incomingIndex < source.Count
                // Sometimes the JIT shows huge room for improvement ;-)
                while (true)
                {
                    if ((uint)incomingIndex >= (uint)source.Count)
                        break;

                    DataPoint tmp = source[incomingIndex];

                    if ((tmp.X - snapShotX) >= minDeltaX)
                    {
                        incomingIndex++;
                        break;
                    }

                    incomingIndex++;
                }
            }

            return incomingIndex;
        }
        //---------------------------------------------------------------------
        protected override void DisposeCore()
        {
            if (_wrapperIterator is not null)
            {
                _wrapperIterator.Dispose();
                _wrapperIterator = null;
            }

            _snapShotIndex         = -1;
            _previousSnapshotIndex = -1;
            _lastArchivedIndex     = -1;
            _incomingIndex         = -1;

            base.DisposeCore();
        }
    }
}
