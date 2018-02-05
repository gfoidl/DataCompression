using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using gfoidl.DataCompression.Wrappers;

namespace gfoidl.DataCompression
{
    /// <summary>
    /// Dead band compression.
    /// </summary>
    /// <remarks>
    /// See documentation for further information.
    /// </remarks>
    public class DeadBandCompression : Compression
    {
        /// <summary>
        /// (Absolut) precision of the instrument.
        /// </summary>
        /// <remarks>
        /// Cf. ExDev in documentation.
        /// </remarks>
        public double InstrumentPrecision { get; }
        //---------------------------------------------------------------------
        private double _maxDeltaX;
        /// <summary>
        /// Length of x before for sure a value gets recorded.
        /// </summary>
        /// <remarks>
        /// Cf. ExMax in documentation.<br />
        /// When specified as <see cref="DateTime" /> the <see cref="DateTime.Ticks" />
        /// are used.
        /// <para>
        /// When value is <c>null</c>, no value -- except the first and last -- are
        /// guaranteed to be recorded.
        /// </para>
        /// </remarks>
        public double? MaxDeltaX => _maxDeltaX == double.MaxValue ? (double?)null : _maxDeltaX;
        //---------------------------------------------------------------------
        /// <summary>
        /// Creates a new instance of dead band compression.
        /// </summary>
        /// <param name="instrumentPrecision">
        /// (Absolut) precision of the instrument. Cf. ExDev in documentation.
        /// </param>
        /// <param name="maxDeltaX">
        /// Length of x before for sure a value gets recoreded. See <see cref="MaxDeltaX"/>.
        /// </param>
        public DeadBandCompression(double instrumentPrecision, double? maxDeltaX = null)
        {
            this.InstrumentPrecision = instrumentPrecision;
            _maxDeltaX               = maxDeltaX ?? double.MaxValue;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Creates a new instance of dead band compression.
        /// </summary>
        /// <param name="instrumentPrecision">
        /// (Absolut) precision of the instrument. Cf. ExDev in documentation.
        /// </param>
        /// <param name="maxTime">Length of time before for sure a value gets recoreded</param>
        public DeadBandCompression(double instrumentPrecision, in TimeSpan maxTime)
        : this(instrumentPrecision, maxTime.Ticks)
        { }
        //---------------------------------------------------------------------
        /// <summary>
        /// Implementation of the compression / filtering.
        /// </summary>
        /// <typeparam name="TList">The type of the enumeration / list.</typeparam>
        /// <param name="data">Input data</param>
        /// <returns>The compressed / filtered data.</returns>
        protected override IEnumerable<DataPoint> ProcessCore(IEnumerable<DataPoint> data)
        {
            if (data is ArrayWrapper<DataPoint> arrayWrapper)
            {
                return arrayWrapper.Count == 0
                    ? DataPointIterator.Empty
                    : new IndexedIterator<ArrayWrapper<DataPoint>>(this, arrayWrapper);
            }

            if (data is ListWrapper<DataPoint> listWrapper)
            {
                return listWrapper.Count == 0
                    ? DataPointIterator.Empty
                    : new IndexedIterator<ListWrapper<DataPoint>>(this, listWrapper);
            }

            if (data is DataPoint[] array)
            {
                return array.Length == 0
                    ? DataPointIterator.Empty
                    : new IndexedIterator<ArrayWrapper<DataPoint>>(this, new ArrayWrapper<DataPoint>(array));
            }

            if (data is List<DataPoint> list)
            {
                return list.Count == 0
                    ? DataPointIterator.Empty
                    : new IndexedIterator<ListWrapper<DataPoint>>(this, new ListWrapper<DataPoint>(list));
            }

            return this.ProcessCoreImpl(data.GetEnumerator());
        }
        //---------------------------------------------------------------------
        private IEnumerable<DataPoint> ProcessCoreImpl(IEnumerator<DataPoint> dataEnumerator)
        {
            if (!dataEnumerator.MoveNext()) yield break;

            DataPoint snapShot     = dataEnumerator.Current;
            DataPoint lastArchived = snapShot;
            DataPoint incoming     = snapShot;      // sentinel, nullable would be possible but to much work around
            yield return snapShot;

            (double Min, double Max) bounding = this.GetBounding(snapShot);

            while (dataEnumerator.MoveNext())
            {
                incoming    = dataEnumerator.Current;
                var archive = this.IsPointToArchive(lastArchived, bounding, incoming);

                if (!archive.Archive)
                {
                    snapShot = incoming;
                    continue;
                }

                if (!archive.MaxDelta)
                    yield return snapShot;

                yield return incoming;

                this.UpdatePoints(ref snapShot, ref lastArchived, incoming, archive.MaxDelta, ref bounding);
            }

            if (incoming != lastArchived)
                yield return incoming;
        }
        //---------------------------------------------------------------------
        private (double Min, double Max) GetBounding(in DataPoint snapShot)
        {
            double min = snapShot.Y - this.InstrumentPrecision;
            double max = snapShot.Y + this.InstrumentPrecision;

            return (min, max);
        }
        //---------------------------------------------------------------------
        private (bool Archive, bool MaxDelta) IsPointToArchive(
            in DataPoint lastArchived,
            in (double Min, double Max) bounding,
            in DataPoint incoming)
        {
            if ((incoming.X - lastArchived.X) >= (_maxDeltaX)) return (true, true);

            return (incoming.Y < bounding.Min || bounding.Max < incoming.Y, false);
        }
        //---------------------------------------------------------------------
        private void UpdatePoints(
            ref DataPoint snapShot,
            ref DataPoint lastArchived,
            in DataPoint incoming,
            bool maxDelta,
            ref (double, double) bounding)
        {
            snapShot     = incoming;
            lastArchived = incoming;

            if (!maxDelta) bounding = this.GetBounding(snapShot);
        }
        //---------------------------------------------------------------------
        private sealed class IndexedIterator<TList> : DataPointIterator where TList : IList<DataPoint>
        {
            private readonly DeadBandCompression  _deadBandCompression;
            private readonly TList                _source;
            private int                           _snapShotIndex;
            private int                           _lastArchivedIndex;
            private int                           _incomingIndex;
            private int                           _currentIndex;
            private (double Min, double Max)      _bounding;
            private (bool Archive, bool MaxDelta) _archive;
            //---------------------------------------------------------------------
            public override DataPoint Current => _source[_currentIndex];
            //---------------------------------------------------------------------
            public override ref DataPoint CurrentByRef
            {
                get
                {
                    _current = this.Current;
                    return ref _current;
                }
            }
            //-----------------------------------------------------------------
            public IndexedIterator(DeadBandCompression deadBandCompression, TList source)
            {
                _deadBandCompression = deadBandCompression;
                _source              = source;
            }
            //-----------------------------------------------------------------
            public override DataPointIterator Clone() => new IndexedIterator<TList>(_deadBandCompression, _source);
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
                        _incomingIndex     = default;
                        _currentIndex      = 0;

                        if (_source.Count < 2)
                        {
                            _state = -1;
                            return true;
                        }

                        this.GetBounding(0);
                        _state         = 1;
                        _incomingIndex = 1;
                        return true;
                    case 1:
                        int count         = _source.Count;
                        int snapShotIndex = _snapShotIndex;
                        int incomingIndex = _incomingIndex;

                        while (incomingIndex < count)
                        {
                            ref var archive = ref this.IsPointToArchive(incomingIndex);

                            if (!archive.Archive)
                            {
                                snapShotIndex = incomingIndex++;
                                continue;
                            }

                            if (!archive.MaxDelta)
                            {
                                _currentIndex  = snapShotIndex;
                                _state         = 2;
                                _snapShotIndex = snapShotIndex;
                                _incomingIndex = incomingIndex;
                                return true;
                            }

                            _snapShotIndex = snapShotIndex;
                            _incomingIndex = incomingIndex;
                            goto case 2;
                        }

                        _currentIndex = incomingIndex - 1;
                        _state        = -1;
                        return true;
                    case 2:
                        _currentIndex = _incomingIndex;
                        _state        = 1;
                        this.UpdatePoints();
                        _incomingIndex++;
                        return true;
                    case DisposedState:
                        ThrowHelper.ThrowIfDisposed(nameof(DataPointIterator));
                        return false;
                }
            }
            //-----------------------------------------------------------------
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void GetBounding(int snapShotIndex)
            {
                double y = _source[snapShotIndex].Y;

                // Produces better code than updating _bouding directly
                ref (double Min, double Max) bounding = ref _bounding;

                bounding.Min = y - _deadBandCompression.InstrumentPrecision;
                bounding.Max = y + _deadBandCompression.InstrumentPrecision;
            }
            //-----------------------------------------------------------------
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private ref (bool Archive, bool MaxDelta) IsPointToArchive(int incomingIndex)
            {
                TList source     = _source;
                int lastArchived = _lastArchivedIndex;

                if ((uint)incomingIndex >= (uint)source.Count || (uint)lastArchived >= (uint)source.Count)
                    ThrowHelper.ThrowArgumentOutOfRange("incomingIndex or lastArchived");

                double lastArchived_x = source[lastArchived].X;
                DataPoint incoming    = source[incomingIndex];

                ref (bool Archive, bool MaxDelta) archive = ref _archive;

                if ((incoming.X - lastArchived_x) >= (_deadBandCompression._maxDeltaX))
                {
                    archive.Archive  = true;
                    archive.MaxDelta = true;
                }
                else
                {
                    archive.Archive  = incoming.Y < _bounding.Min || _bounding.Max < incoming.Y;
                    archive.MaxDelta = false;
                }

                return ref archive;
            }
            //-----------------------------------------------------------------
            private void UpdatePoints()
            {
                int incoming       = _incomingIndex;
                _snapShotIndex     = incoming;
                _lastArchivedIndex = incoming;

                if (!_archive.MaxDelta) this.GetBounding(incoming);
            }
        }
    }
}
