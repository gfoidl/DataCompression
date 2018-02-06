using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using gfoidl.DataCompression.Wrappers;
using gfoidl.Stochastics.Builders;

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
        /// <param name="data">Input data</param>
        /// <returns>The compressed / filtered data.</returns>
        protected override DataPointIterator ProcessCore(IEnumerable<DataPoint> data)
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

            if (data is IList<DataPoint> ilist)
            {
                return ilist.Count == 0
                    ? DataPointIterator.Empty
                    : new IndexedIterator<IList<DataPoint>>(this, ilist);
            }

            IEnumerator<DataPoint> enumerator = data.GetEnumerator();
            return enumerator.MoveNext()
                ? new EnumerableIterator(this, data, enumerator)
                : DataPointIterator.Empty;
        }
        //---------------------------------------------------------------------
        private abstract class DeadBandCompressionIterator : DataPointIterator
        {
            protected readonly DeadBandCompression  _deadBandCompression;
            protected (double Min, double Max)      _bounding;
            protected (bool Archive, bool MaxDelta) _archive;
            //-----------------------------------------------------------------
            protected DeadBandCompressionIterator(DeadBandCompression deadBandCompression)
                => _deadBandCompression = deadBandCompression;
            //---------------------------------------------------------------------
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected void GetBounding(in DataPoint dataPoint)
            {
                double y = dataPoint.Y;

                // Produces better code than updating _bouding directly
                ref (double Min, double Max) bounding = ref _bounding;

                bounding.Min = y - _deadBandCompression.InstrumentPrecision;
                bounding.Max = y + _deadBandCompression.InstrumentPrecision;
            }
        }
        //---------------------------------------------------------------------
        private sealed class EnumerableIterator : DeadBandCompressionIterator
        {
            private readonly IEnumerable<DataPoint> _source;
            private readonly IEnumerator<DataPoint> _enumerator;
            private DataPoint                       _snapShot;
            private DataPoint                       _lastArchived;
            private DataPoint                       _incoming;
            //-----------------------------------------------------------------
            public EnumerableIterator(
                DeadBandCompression deadBandCompression,
                IEnumerable<DataPoint> source,
                IEnumerator<DataPoint> enumerator)
                : base(deadBandCompression)
            {
                _source     = source;
                _enumerator = enumerator;
            }
            //-----------------------------------------------------------------
            public override DataPointIterator Clone() => new EnumerableIterator(_deadBandCompression, _source, _enumerator);
            //-----------------------------------------------------------------
            public override bool MoveNext()
            {
                switch (_state)
                {
                    default:
                        this.Dispose();
                        return false;
                    case 0:
                        _snapShot     = _enumerator.Current;
                        _lastArchived = _snapShot;
                        _incoming     = _snapShot;      // sentinel, nullable would be possible but to much work around
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

                            if (!archive.MaxDelta)
                            {
                                _current = _snapShot;
                                _state   = 2;
                                return true;
                            }

                            goto case 2;
                        }

                        _state = -1;
                        if (_incoming != _lastArchived)
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
                    case DisposedState:
                        ThrowHelper.ThrowIfDisposed(nameof(DataPointIterator));
                        return false;
                }
            }
            //-----------------------------------------------------------------
            /// <summary>
            /// Returns an array of the compressed <see cref="DataPoint" />s.
            /// </summary>
            /// <returns>An array of the compressed <see cref="DataPoint" />s.</returns>
            public override DataPoint[] ToArray()
            {
                IEnumerator<DataPoint> enumerator = _source.GetEnumerator();

                if (!enumerator.MoveNext())
                    return Array.Empty<DataPoint>();

                DataPoint snapShot = enumerator.Current;
                _lastArchived      = snapShot;
                DataPoint incoming = snapShot;          // sentinel, nullable would be possible but to much work around

                var arrayBuilder = new ArrayBuilder<DataPoint>(true);
                arrayBuilder.Add(snapShot);
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

                    if (!archive.MaxDelta)
                        arrayBuilder.Add(snapShot);

                    arrayBuilder.Add(incoming);
                    this.UpdatePoints(incoming, ref snapShot);
                }

                if (incoming != _lastArchived)          // sentinel-check
                    arrayBuilder.Add(incoming);

                return arrayBuilder.ToArray();
            }
            //-----------------------------------------------------------------
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private ref (bool Archive, bool MaxDelta) IsPointToArchive(in DataPoint incoming)
            {
                ref (bool Archive, bool MaxDelta) archive = ref _archive;

                if ((incoming.X - _lastArchived.X) >= (_deadBandCompression._maxDeltaX))
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
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void UpdatePoints(in DataPoint incoming, ref DataPoint snapShot)
            {
                _lastArchived = incoming;
                snapShot      = incoming;

                if (!_archive.MaxDelta) this.GetBounding(snapShot);
            }
        }
        //---------------------------------------------------------------------
        private sealed class IndexedIterator<TList> : DeadBandCompressionIterator where TList : IList<DataPoint>
        {
            private readonly TList _source;
            private int            _snapShotIndex;
            private int            _lastArchivedIndex;
            private int            _incomingIndex;
            //-----------------------------------------------------------------
            public IndexedIterator(DeadBandCompression deadBandCompression, TList source)
                : base(deadBandCompression)
                => _source = source;
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
                        _current           = _source[0];

                        if (_source.Count < 2)
                        {
                            _state = -1;
                            return true;
                        }

                        this.GetBounding(_current);
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

                            ref var archive = ref this.IsPointToArchive(incomingIndex);

                            if (!archive.Archive)
                            {
                                snapShotIndex = incomingIndex++;
                                continue;
                            }

                            if (!archive.MaxDelta)
                            {
                                _current = source[snapShotIndex];
                                _state = 2;
                                _snapShotIndex = snapShotIndex;
                                _incomingIndex = incomingIndex;
                                return true;
                            }

                            _snapShotIndex = snapShotIndex;
                            _incomingIndex = incomingIndex;
                            goto case 2;
                        }

                        _current = source[incomingIndex - 1];
                        _state   = -1;
                        return true;
                    case 2:
                        _current = _source[_incomingIndex];
                        _state   = 1;
                        this.UpdatePoints(_incomingIndex, _current, ref _snapShotIndex);
                        _incomingIndex++;
                        return true;
                    case DisposedState:
                        ThrowHelper.ThrowIfDisposed(nameof(DataPointIterator));
                        return false;
                }
            }
            //-----------------------------------------------------------------
            /// <summary>
            /// Returns an array of the compressed <see cref="DataPoint" />s.
            /// </summary>
            /// <returns>An array of the compressed <see cref="DataPoint" />s.</returns>
            public override DataPoint[] ToArray()
            {
                TList source      = _source;
                int snapShotIndex = 0;

                if ((uint)snapShotIndex >= (uint)source.Count)
                    return Array.Empty<DataPoint>();

                DataPoint snapShot = source[snapShotIndex];

                if (source.Count < 2)
                    return new[] { snapShot };

                var arrayBuilder = new ArrayBuilder<DataPoint>(true);
                arrayBuilder.Add(snapShot);
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
                        arrayBuilder.Add(source[snapShotIndex]);

                    arrayBuilder.Add(source[incomingIndex]);
                    this.UpdatePoints(incomingIndex, source[incomingIndex], ref snapShotIndex);
                }

                incomingIndex--;
                if ((uint)incomingIndex < (uint)source.Count)
                    arrayBuilder.Add(source[incomingIndex]);

                return arrayBuilder.ToArray();
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
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void UpdatePoints(int incomingIndex, in DataPoint incoming, ref int snapShotIndex)
            {
                snapShotIndex      = incomingIndex;
                _lastArchivedIndex = incomingIndex;

                if (!_archive.MaxDelta) this.GetBounding(incoming);
            }
        }
    }
}
