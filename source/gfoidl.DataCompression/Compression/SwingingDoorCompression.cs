using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using gfoidl.DataCompression.Wrappers;

namespace gfoidl.DataCompression
{
    /// <summary>
    /// Swinging door compression.
    /// </summary>
    /// <remarks>
    /// See documentation for further information.
    /// </remarks>
    public partial class SwingingDoorCompression : Compression
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
        private readonly double _maxDeltaX;
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
        private readonly bool   _minDeltaXHasValue;
        private readonly double _minDeltaX;
        /// <summary>
        /// Length of x/time within no value gets recorded (after the last archived value)
        /// </summary>
        public double? MinDeltaX => _minDeltaXHasValue ? _minDeltaX : (double?)null;
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

            if (minDeltaX.HasValue)
            {
                _minDeltaXHasValue = true;
                _minDeltaX         = minDeltaX.Value;
            }
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

            return new EnumerableIterator(this, data);
        }
        //---------------------------------------------------------------------
        private abstract class SwingingDoorCompressionIterator : DataPointIterator
        {
            protected static readonly (double Max, double Min) s_newDoor = (double.PositiveInfinity, double.NegativeInfinity);
            //---------------------------------------------------------------------
            protected readonly SwingingDoorCompression _swingingDoorCompression;
            protected (double Max, double Min)         _slope;
            protected (bool Archive, bool MaxDelta)    _archive;
            protected DataPoint                        _lastArchived;
            protected DataPoint                        _incoming;
            //---------------------------------------------------------------------
            protected SwingingDoorCompressionIterator(SwingingDoorCompression swingingDoorCompression)
                => _swingingDoorCompression = swingingDoorCompression;
            //-----------------------------------------------------------------
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected void IsPointToArchive(in DataPoint incoming, in DataPoint lastArchived)
            {
                if ((incoming.X - lastArchived.X) >= (_swingingDoorCompression._maxDeltaX))
                {
                    _archive.Archive  = true;
                    _archive.MaxDelta = true;
                }
                else
                {
                    // Better to compare via gradient (1 calculation) than comparing to allowed y-values (2 calcuations)
                    // Obviously, the result should be the same ;-)
                    double slopeToIncoming = lastArchived.Gradient(incoming);

                    _archive.Archive  = slopeToIncoming < _slope.Min || _slope.Max < slopeToIncoming;
                    _archive.MaxDelta = false;
                }
            }
            //-----------------------------------------------------------------
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected void CloseTheDoor(in DataPoint incoming, in DataPoint lastArchived)
            {
                double upperSlope = lastArchived.Gradient(incoming,  _swingingDoorCompression.CompressionDeviation);
                double lowerSlope = lastArchived.Gradient(incoming, -_swingingDoorCompression.CompressionDeviation);

                if (upperSlope < _slope.Max) _slope.Max = upperSlope;
                if (lowerSlope > _slope.Min) _slope.Min = lowerSlope;
            }
            //-----------------------------------------------------------------
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected void OpenNewDoor(in DataPoint incoming)
            {
                _lastArchived = incoming;
                _slope        = s_newDoor;
            }
        }
        //---------------------------------------------------------------------
        private abstract class SwingingDoorCompressionEnumerableIterator: SwingingDoorCompressionIterator
        {
            protected DataPoint _snapShot;
            //-----------------------------------------------------------------
            protected SwingingDoorCompressionEnumerableIterator(SwingingDoorCompression swingingDoorCompression)
                : base(swingingDoorCompression)
            { }
        }
    }
}
