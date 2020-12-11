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
        private int                _lastArchivedIndex;
        private int                _incomingIndex;
        //-----------------------------------------------------------------
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
        //-----------------------------------------------------------------
        public override bool MoveNext()
        {
            Debug.Assert(_list is not null);

            switch (_state)
            {
                case 0:
                    _incomingIndex = 0;
                    _incoming      = _list[0];
                    _current       = _incoming;

                    if (_list.Count < 2)
                    {
                        _state = -1;
                        return true;
                    }

                    this.Init(0, _incoming, ref _snapShotIndex);
                    _state         = 1;
                    _incomingIndex = 1;
                    return true;
                case 1:
                    TList source           = _list;
                    int snapShotIndex      = _snapShotIndex;
                    int incomingIndex      = _incomingIndex;
                    int lastArchivedIndex  = _lastArchivedIndex;

                    while (true)
                    {
                        // Actually a while loop, but so the range check can be eliminated
                        // https://github.com/dotnet/coreclr/issues/15476
                        if ((uint)incomingIndex     >= (uint)source.Count
                         || (uint)snapShotIndex     >= (uint)source.Count
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
                    incomingIndex--;
                    if (incomingIndex != _lastArchivedIndex)    // sentinel check
                    {
                        _current = source[incomingIndex];
                        return true;
                    }
                    return false;
                case 2:
                    incomingIndex  = _incomingIndex;
                    incomingIndex  = this.HandleSkipMinDeltaX(_list, incomingIndex, _snapShotIndex);
                    _current       = _list[incomingIndex];
                    _state         = 1;
                    this.Init(incomingIndex, _current, ref _snapShotIndex);
                    _incomingIndex = incomingIndex + 1;
                    return true;
                case InitialState:
                    ThrowHelper.ThrowInvalidOperation(ThrowHelper.ExceptionResource.GetEnumerator_must_be_called_first);
                    return false;
                case DisposedState:
                    ThrowHelper.ThrowIfDisposed(ThrowHelper.ExceptionArgument.iterator);
                    return false;
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
        //-----------------------------------------------------------------
        public override List<DataPoint> ToList()
        {
            Debug.Assert(_list is not null);

            TList source = _list;

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
        protected internal override void Init(in DataPoint incoming, ref DataPoint snapShot)                                             => _wrapperIterator!.Init(incoming, ref snapShot);
        //---------------------------------------------------------------------
        protected internal override void Init(int incomingIndex, in DataPoint incoming, ref int snapShotIndex)
        {
            Debug.Assert(_wrapperIterator is not null);

            _wrapperIterator.Init(incomingIndex, incoming, ref snapShotIndex);
            _lastArchivedIndex = incomingIndex;
        }
        //---------------------------------------------------------------------
        private void BuildCollection<TBuilder>(TList source, ref TBuilder builder)
            where TBuilder : ICollectionBuilder<DataPoint>
        {
            int incomingIndex = 0;
            int snapShotIndex = 0;

            if ((uint)incomingIndex >= (uint)source.Count) return;

            DataPoint incoming = source[incomingIndex];
            _lastArchived      = incoming;
            builder.Add(incoming);
            this.Init(0, incoming, ref snapShotIndex);

            incomingIndex = 1;

            // Is actually a for loop, but the JIT doesn't elide the bound check
            // due to SkipMinDeltaX. I.e. w/o SkipMinDeltaX the bound check gets
            // eliminated.
            while (true)
            {
                if ((uint)incomingIndex >= (uint)source.Count)
                    break;

                incoming        = source[incomingIndex];
                ref var archive = ref this.IsPointToArchive(incoming, _lastArchived);

                if (!archive.Archive)
                {
                    this.UpdateFilters(incoming, _lastArchived);
                    snapShotIndex = incomingIndex++;
                    continue;
                }

                if (!archive.MaxDelta && (uint)snapShotIndex < (uint)source.Count)
                {
                    DataPoint snapShot = source[snapShotIndex];
                    builder.Add(snapShot);
                    _lastArchived = snapShot;
                }

                incomingIndex = this.HandleSkipMinDeltaX(source, incomingIndex, snapShotIndex);

                incoming      = source[incomingIndex];
                builder.Add(incoming);
                _lastArchived = incoming;
                this.Init(incomingIndex, incoming, ref snapShotIndex);

                incomingIndex++;
            }

            incomingIndex--;
            if (incomingIndex != _lastArchivedIndex && (uint)incomingIndex < (uint)source.Count)
            {
                builder.Add(source[incomingIndex]);
            }
        }
        //---------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int HandleSkipMinDeltaX(TList source, int incomingIndex, int snapShotIndex)
        {
            Debug.Assert(_algorithm is not null);

            return _algorithm._minDeltaXHasValue
                ? this.SkipMinDeltaX(source, incomingIndex, snapShotIndex)
                : incomingIndex;
        }
        //---------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.NoInlining)]
        private int SkipMinDeltaX(TList source, int incomingIndex, int snapShotIndex)
        {
            Debug.Assert(_algorithm is not null);
            Debug.Assert(_algorithm.MinDeltaX.HasValue);

            if ((uint)snapShotIndex < (uint)source.Count)
            {
                double snapShotX = source[snapShotIndex].X;
                double minDeltaX = _algorithm._minDeltaX;

                // A for loop won't elide the bound checks, although incomingIndex < source.Count
                // Sometimes the JIT shows huge room for improvement ;-)
                while (true)
                {
                    if ((uint)incomingIndex >= (uint)source.Count)
                        break;

                    DataPoint incoming = source[incomingIndex];

                    if ((incoming.X - snapShotX) > minDeltaX)
                    {
                        _incoming = incoming;
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

            _snapShotIndex     = -1;
            _lastArchivedIndex = -1;
            _incomingIndex     = -1;

            base.DisposeCore();
        }
    }
}
