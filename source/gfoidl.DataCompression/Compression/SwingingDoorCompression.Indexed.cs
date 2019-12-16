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
        private sealed class IndexedIterator<TList> : SwingingDoorCompressionIterator
            where TList : IList<DataPoint>
        {
            private readonly TList _source;
            private int            _snapShotIndex;
            private int            _lastArchivedIndex;
            private int            _incomingIndex;
            //-----------------------------------------------------------------
            public IndexedIterator(SwingingDoorCompression swingingDoorCompression, TList source)
                : base(swingingDoorCompression)
                => _source = source;
            //-----------------------------------------------------------------
            public override DataPointIterator Clone() => new IndexedIterator<TList>(_swingingDoorCompression, _source);
            //-----------------------------------------------------------------
            public override bool MoveNext()
            {
                switch (_state)
                {
                    default:
                        this.Dispose();
                        return false;
                    case 0:
                        _snapShotIndex     = 0;
                        _lastArchivedIndex = 0;
                        _incomingIndex     = 0;
                        _current           = _source[0];
                        _incoming          = _current;

                        if (_source.Count < 2)
                        {
                            _state = -1;
                            return true;
                        }

                        this.OpenNewDoor(0, _incoming);
                        _state         = 1;
                        _incomingIndex = 1;
                        return true;
                    case 1:
                        TList source      = _source;
                        int snapShotIndex = _snapShotIndex;
                        int incomingIndex = _incomingIndex;

                        while (true)
                        {
                            // Actually a while loop, but so the range check can be eliminated
                            // https://github.com/dotnet/coreclr/issues/15476
                            if ((uint)incomingIndex >= (uint)source.Count || (uint)snapShotIndex >= (uint)source.Count)
                                break;

                            _incoming       = source[incomingIndex];
                            this.IsPointToArchive(_incoming, _lastArchived);
                            ref var archive = ref _archive;

                            if (!archive.Archive)
                            {
                                this.CloseTheDoor(_incoming, _lastArchived);
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
                        if (incomingIndex != _lastArchivedIndex)
                        {
                            _current = source[incomingIndex];
                            return true;
                        }
                        return false;
                    case 2:
                        incomingIndex = _incomingIndex;

                        if (_swingingDoorCompression._minDeltaXHasValue)
                            incomingIndex = this.SkipMinDeltaX(_source, _snapShotIndex, incomingIndex);

                        _current       = _source[incomingIndex];
                        _state         = 1;
                        this.OpenNewDoor(incomingIndex, _incoming);
                        _incomingIndex = incomingIndex + 1;
                        return true;
                    case InitialState:
                        ThrowHelper.ThrowInvalidOperation(ThrowHelper.ExceptionResource.GetEnumerator_must_be_called_first);
                        return false;
                    case DisposedState:
                        ThrowHelper.ThrowIfDisposed(ThrowHelper.ExceptionArgument.iterator);
                        return false;
                }
            }
            //-----------------------------------------------------------------
            public override DataPoint[] ToArray()
            {
                TList source    = _source;
                const int index = 0;

                Debug.Assert(source.Count > 0);

                if (source.Count == 1 && (uint)index < (uint)source.Count)
                    return new[] { source[index] };

                var arrayBuilder = new ArrayBuilder<DataPoint>(true);
                this.BuildCollection(source, ref arrayBuilder);

                return arrayBuilder.ToArray();
            }
            //-----------------------------------------------------------------
            public override List<DataPoint> ToList()
            {
                TList source    = _source;
                const int index = 0;

                Debug.Assert(source.Count > 0);

                if (source.Count == 1 && (uint)index < (uint)source.Count)
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
                this.OpenNewDoor(0, snapShot);

                int incomingIndex = 1;

                // Is actually a for loop, but the JIT doesn't elide the bound check
                // due to SkipMinDeltaX. I.e. w/o SkipMinDeltaX the bound check gets
                // eliminated.
                while (true)
                {
                    if ((uint)incomingIndex >= (uint)source.Count) break;

                    DataPoint incoming = source[incomingIndex];
                    this.IsPointToArchive(incoming, _lastArchived);
                    ref var archive    = ref _archive;

                    if (!archive.Archive)
                    {
                        this.CloseTheDoor(incoming, _lastArchived);
                        snapShotIndex = incomingIndex++;
                        continue;
                    }

                    if (!archive.MaxDelta && (uint)snapShotIndex < (uint)source.Count)
                        builder.Add(source[snapShotIndex]);

                    if (_swingingDoorCompression._minDeltaXHasValue)
                    {
                        incomingIndex = this.SkipMinDeltaX(source, snapShotIndex, incomingIndex);
                        incoming      = _incoming;
                    }

                    builder.Add(incoming);
                    this.OpenNewDoor(incomingIndex, incoming);

                    incomingIndex++;
                }

                incomingIndex--;
                if (incomingIndex != _lastArchivedIndex && (uint)incomingIndex < (uint)source.Count)
                    builder.Add(source[incomingIndex]);
            }
            //-----------------------------------------------------------------
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void OpenNewDoor(int incomingIndex, in DataPoint incoming)
            {
                _lastArchivedIndex = incomingIndex;
                this.OpenNewDoor(incoming);
            }
            //-----------------------------------------------------------------
            [MethodImpl(MethodImplOptions.NoInlining)]
            private int SkipMinDeltaX(TList source, int snapShotIndex, int incomingIndex)
            {
                if ((uint)snapShotIndex < (uint)source.Count)
                {
                    double snapShot_x = source[snapShotIndex].X;
                    double minDeltaX  = _swingingDoorCompression._minDeltaX;

                    // A for loop won't elide the bound checks, although incomingIndex < source.Count
                    // Sometimes the JIT shows huge room for improvement ;-)
                    while (true)
                    {
                        if ((uint)incomingIndex >= (uint)source.Count) break;

                        DataPoint incoming = source[incomingIndex];

                        if ((incoming.X - snapShot_x) > minDeltaX)
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
#if NETSTANDARD2_1
            public override ValueTask<bool> MoveNextAsync()          => throw new NotSupportedException();
            public override ValueTask<DataPoint[]> ToArrayAsync()    => throw new NotSupportedException();
            public override ValueTask<List<DataPoint>> ToListAsync() => throw new NotSupportedException();
#endif
        }
    }
}
