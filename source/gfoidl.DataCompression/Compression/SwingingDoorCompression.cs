using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using gfoidl.DataCompression.Wrappers;
using gfoidl.Stochastics.Builders;

namespace gfoidl.DataCompression
{
    /// <summary>
    /// Swinging door compression.
    /// </summary>
    /// <remarks>
    /// See documentation for further information.
    /// </remarks>
    public class SwingingDoorCompression : Compression
    {
        /// <summary>
        /// (Absolut) Compression deviation applied to the y values to calculate the
        /// min and max slopes.
        /// </summary>
        /// <remarks>
        /// Cf. CompDev in documentation.
        /// </remarks>
        public double CompressionDeviation { get; }
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
        /// Length of x/time within no value gets recorded (after the last archived value)
        /// </summary>
        public double? MinDeltaX { get; }
        //---------------------------------------------------------------------
        /// <summary>
        /// Creates a new instance of swinging door compression.
        /// </summary>
        /// <param name="compressionDeviation">
        /// (Absolut) Compression deviation applied to the y values to calculate the
        /// min and max slopes. Cf. CompDev in documentation.
        /// </param>
        /// <param name="maxDeltaX">
        /// Length of x before for sure a value gets recoreded. See <see cref="MaxDeltaX" />.
        /// </param>
        /// <param name="minDeltaX">
        /// Length of x/time within no value gets recorded (after the last archived value).
        /// See <see cref="MinDeltaX" />.
        /// </param>
        public SwingingDoorCompression(double compressionDeviation, double? maxDeltaX = null, double? minDeltaX = null)
        {
            this.CompressionDeviation = compressionDeviation;
            _maxDeltaX                = maxDeltaX ?? double.MaxValue;
            this.MinDeltaX            = minDeltaX;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Creates a new instance of swinging door compression.
        /// </summary>
        /// <param name="compressionDeviation">
        /// (Absolut) Compression deviation applied to the y values to calculate the
        /// min and max slopes. Cf. CompDev in documentation.
        /// </param>
        /// <param name="maxTime">Length of time before for sure a value gets recoreded</param>
        /// <param name="minTime">Length of time within no value gets recorded (after the last archived value)</param>
        public SwingingDoorCompression(double compressionDeviation, TimeSpan maxTime, TimeSpan? minTime)
            : this(compressionDeviation, maxTime.Ticks, minTime?.Ticks)
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
        private abstract class SwingingDoorCompressionIterator : DataPointIterator
        {
            protected readonly SwingingDoorCompression _swingingDoorCompression;
            protected (double Max, double Min)         _slope;
            protected (bool Archive, bool MaxDelta)    _archive;
            protected DataPoint                        _lastArchived;
            protected DataPoint                        _incoming;
            //-----------------------------------------------------------------
            protected SwingingDoorCompressionIterator(SwingingDoorCompression swingingDoorCompression)
                => _swingingDoorCompression = swingingDoorCompression;
            //-----------------------------------------------------------------
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected void IsPointToArchive()
            {
                if ((_incoming.X - _lastArchived.X) >= (_swingingDoorCompression._maxDeltaX))
                {
                    _archive.Archive  = true;
                    _archive.MaxDelta = true;
                }
                else
                {
                    // Better to compare via gradient (1 calculation) than comparing to allowed y-values (2 calcuations)
                    // Obviously, the result should be the same ;-)
                    double slopeToIncoming = _lastArchived.Gradient(_incoming);

                    _archive.Archive  = slopeToIncoming < _slope.Min || _slope.Max < slopeToIncoming;
                    _archive.MaxDelta = false;
                }
            }
            //-----------------------------------------------------------------
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected void CloseTheDoor()
            {
                double upperSlope = _lastArchived.Gradient(_incoming, _swingingDoorCompression.CompressionDeviation);
                double lowerSlope = _lastArchived.Gradient(_incoming, -_swingingDoorCompression.CompressionDeviation);

                if (upperSlope < _slope.Max) _slope.Max = upperSlope;
                if (lowerSlope > _slope.Min) _slope.Min = lowerSlope;
            }
            //-----------------------------------------------------------------
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected void OpenNewDoor()
            {
                _lastArchived = _incoming;

                _slope.Max = double.PositiveInfinity;
                _slope.Min = double.NegativeInfinity;
            }
        }
        //---------------------------------------------------------------------
        private sealed class EnumerableIterator : SwingingDoorCompressionIterator
        {
            private readonly IEnumerable<DataPoint> _source;
            private readonly IEnumerator<DataPoint> _enumerator;
            private DataPoint                       _snapShot;
            //-----------------------------------------------------------------
            public EnumerableIterator(
                SwingingDoorCompression swingingDoorCompression,
                IEnumerable<DataPoint> source,
                IEnumerator<DataPoint> enumerator)
                : base(swingingDoorCompression)
            {
                _source     = source;
                _enumerator = enumerator;
            }
            //-----------------------------------------------------------------
            public override DataPointIterator Clone() => new EnumerableIterator(_swingingDoorCompression, _source, _enumerator);
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
                        _incoming     = _snapShot;          // sentinel, nullable would be possible but to much work around
                        _current      = _snapShot;
                        this.OpenNewDoor();
                        _state        = 1;
                        return true;
                    case 1:
                        while (_enumerator.MoveNext())
                        {
                            _incoming       = _enumerator.Current;
                            this.IsPointToArchive();
                            ref var archive = ref _archive;

                            if (!archive.Archive)
                            {
                                this.CloseTheDoor();
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
                        if (_incoming != _lastArchived)     // sentinel-check
                        {
                            _current = _incoming;
                            return true;
                        }
                        return false;
                    case 2:
                        if (_swingingDoorCompression.MinDeltaX.HasValue)
                        {
                            double snapShot_x = _snapShot.X;
                            double minDeltaX  = _swingingDoorCompression.MinDeltaX.Value;

                            while (_enumerator.MoveNext())
                            {
                                _incoming = _enumerator.Current;
                                if ((_incoming.X - snapShot_x) > minDeltaX)
                                    break;
                            }
                        }

                        _current = _incoming;
                        _state   = 1;
                        this.OpenNewDoor();
                        return true;
                    case DisposedState:
                        ThrowHelper.ThrowIfDisposed(nameof(DataPointIterator));
                        return false;
                }
            }
            //---------------------------------------------------------------------
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
                _incoming          = snapShot;          // sentinel, nullable would be possible but to much work around

                var arrayBuilder = new ArrayBuilder<DataPoint>(true);
                arrayBuilder.Add(snapShot);
                this.OpenNewDoor();

                while (enumerator.MoveNext())
                {
                    _incoming       = enumerator.Current;
                    this.IsPointToArchive();
                    ref var archive = ref _archive;

                    if (!archive.Archive)
                    {
                        this.CloseTheDoor();
                        snapShot = _incoming;
                        continue;
                    }

                    if (!archive.MaxDelta)
                        arrayBuilder.Add(snapShot);

                    if (_swingingDoorCompression.MinDeltaX.HasValue)
                    {
                        double snapShot_x = snapShot.X;
                        double minDeltaX  = _swingingDoorCompression.MinDeltaX.Value;

                        while (enumerator.MoveNext())
                        {
                            _incoming = enumerator.Current;
                            if ((_incoming.X - snapShot_x) > minDeltaX)
                                break;
                        }
                    }

                    arrayBuilder.Add(_incoming);
                    this.OpenNewDoor();
                }

                if (_incoming != _lastArchived)          // sentinel-check
                    arrayBuilder.Add(_incoming);

                return arrayBuilder.ToArray();
            }
        }
        //---------------------------------------------------------------------
        private sealed class IndexedIterator<TList> : SwingingDoorCompressionIterator where TList : IList<DataPoint>
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
                        _incomingIndex     = default;
                        _current           = _source[0];
                        _incoming          = _current;

                        if (_source.Count < 2)
                        {
                            _state = -1;
                            return true;
                        }

                        this.OpenNewDoor(0);
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

                            _incoming = source[incomingIndex];

                            this.IsPointToArchive();
                            ref var archive = ref _archive;

                            if (!archive.Archive)
                            {
                                this.CloseTheDoor();
                                snapShotIndex = incomingIndex++;
                                continue;
                            }

                            if (!archive.MaxDelta)
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
                        source        = _source;
                        incomingIndex = _incomingIndex;

                        if (_swingingDoorCompression.MinDeltaX.HasValue)
                        {
                            double snapShot_x = source[_snapShotIndex].X;
                            double minDeltaX  = _swingingDoorCompression.MinDeltaX.Value;

                            for (; incomingIndex < source.Count; ++incomingIndex)
                            {
                                DataPoint incoming = source[incomingIndex];
                                if ((incoming.X - snapShot_x) > minDeltaX)
                                    break;
                            }
                        }

                        _current       = source[incomingIndex];
                        _state         = 1;
                        this.OpenNewDoor(incomingIndex);
                        _incomingIndex = incomingIndex + 1;
                        return true;
                }
            }
            //---------------------------------------------------------------------
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

                // Is actually the snapshot, but this in an optimization for OpenNewDoor
                _incoming = source[snapShotIndex];

                if (source.Count < 2)
                    return new[] { _incoming };

                var arrayBuilder = new ArrayBuilder<DataPoint>(true);
                arrayBuilder.Add(_incoming);
                this.OpenNewDoor(0);

                int incomingIndex = 1;
                for (; incomingIndex < source.Count; ++incomingIndex)
                {
                    _incoming       = source[incomingIndex];
                    this.IsPointToArchive();
                    ref var archive = ref _archive;

                    if (!archive.Archive)
                    {
                        this.CloseTheDoor();
                        snapShotIndex = incomingIndex;
                        continue;
                    }

                    if (!archive.MaxDelta && (uint)snapShotIndex < (uint)source.Count)
                        arrayBuilder.Add(source[snapShotIndex]);

                    if (_swingingDoorCompression.MinDeltaX.HasValue)
                    {
                        double snapShot_x = source[snapShotIndex].X;
                        double minDeltaX  = _swingingDoorCompression.MinDeltaX.Value;

                        for (; incomingIndex < source.Count; ++incomingIndex)
                        {
                            DataPoint incoming = source[incomingIndex];
                            if ((incoming.X - snapShot_x) > minDeltaX)
                            {
                                _incoming = incoming;
                                break;
                            }
                        }
                    }

                    arrayBuilder.Add(_incoming);
                    this.OpenNewDoor(incomingIndex);
                }

                incomingIndex--;
                if (incomingIndex != _lastArchivedIndex && (uint)incomingIndex < (uint)source.Count)
                    arrayBuilder.Add(source[incomingIndex]);

                return arrayBuilder.ToArray();
            }
            //-----------------------------------------------------------------
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void OpenNewDoor(int incomingIndex)
            {
                _lastArchivedIndex = incomingIndex;

                this.OpenNewDoor();
            }
        }
    }
}
