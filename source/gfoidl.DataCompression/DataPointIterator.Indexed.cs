using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace gfoidl.DataCompression
{
    /// <summary>
    /// Base class for an indexed <see cref="DataPointIterator" />.
    /// </summary>
    /// <typeparam name="TList">The type of the list-wrapper.</typeparam>
    public abstract class DataPointIndexedIterator<TList> : DataPointIterator
        where TList : notnull, IList<DataPoint>
    {
#pragma warning disable CS1591
        protected readonly TList _list;
        protected int _snapShotIndex;
        protected int _lastArchivedIndex;
        protected int _incomingIndex;
#pragma warning restore CS1591
        //-----------------------------------------------------------------
        protected DataPointIndexedIterator(Compression compression, TList source)
            : base(compression)
            => _list = source;
        //-----------------------------------------------------------------
        public override bool MoveNext()
        {
            switch (_state)
            {
                case 0:
                    _incomingIndex     = 0;
                    _lastArchivedIndex = 0;
                    _incoming          = _list[0];
                    _current           = _incoming;

                    if (_list.Count < 2)
                    {
                        _state = -1;
                        return true;
                    }

                    this.Init(0, ref _snapShotIndex);
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

                        _incoming = source[incomingIndex];
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
                    incomingIndex  = this.HandleSpecialCaseAfterArchivedPoint(_list, incomingIndex, _snapShotIndex);
                    _current       = _list[incomingIndex];
                    _state         = 1;
                    this.Init(incomingIndex, ref _snapShotIndex);
                    _incomingIndex = incomingIndex + 1;
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
        //---------------------------------------------------------------------
        protected abstract void Init(int incomingIndex, ref int snapShotIndex);
        //---------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int HandleSpecialCaseAfterArchivedPoint(TList source, int incomingIndex, int snapShotIndex)
        {
            return _algorithm._minDeltaXHasValue
                ? this.SkipMinDeltaX(source, incomingIndex, snapShotIndex)
                : incomingIndex;
        }
        //---------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.NoInlining)]
        private int SkipMinDeltaX(TList source, int incomingIndex, int snapShotIndex)
        {
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
    }
}
