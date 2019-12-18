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
    public partial class DeadBandCompression : Compression
    {
        /// <summary>
        /// (Absolut) precision of the instrument.
        /// </summary>
        /// <remarks>
        /// Cf. ExDev in documentation.
        /// </remarks>
        public double InstrumentPrecision { get; }
        //---------------------------------------------------------------------
        /// <summary>
        /// Creates a new instance of dead band compression.
        /// </summary>
        /// <param name="instrumentPrecision">
        /// (Absolut) precision of the instrument. Cf. ExDev in documentation.
        /// </param>
        /// <param name="maxDeltaX">
        /// Length of x before for sure a value gets recoreded. See <see cref="Compression.MaxDeltaX" />.
        /// </param>
        /// <param name="minDeltaX">
        /// Length of x/time within no value gets recorded (after the last archived value).
        /// See <see cref="Compression.MinDeltaX" />.
        /// </param>
        public DeadBandCompression(double instrumentPrecision, double? maxDeltaX = null, double? minDeltaX = null)
            : base(maxDeltaX, minDeltaX)
            => this.InstrumentPrecision = instrumentPrecision;
        //---------------------------------------------------------------------
        /// <summary>
        /// Creates a new instance of dead band compression.
        /// </summary>
        /// <param name="instrumentPrecision">
        /// (Absolut) precision of the instrument. Cf. ExDev in documentation.
        /// </param>
        /// <param name="maxTime">Length of time before for sure a value gets recoreded</param>
        /// <param name="minTime">Length of time within no value gets recorded (after the last archived value)</param>
        public DeadBandCompression(double instrumentPrecision, TimeSpan maxTime, TimeSpan? minTime)
            : this(instrumentPrecision, maxTime.Ticks, minTime?.Ticks)
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
        private abstract class DeadBandCompressionIterator : DataPointIterator
        {
            protected readonly DeadBandCompression  _deadBandCompression;
            protected (double Min, double Max)      _bounding;
            protected (bool Archive, bool MaxDelta) _archive;
            //-----------------------------------------------------------------
            protected DeadBandCompressionIterator(DeadBandCompression deadBandCompression)
                : base(deadBandCompression)
                => _deadBandCompression = deadBandCompression;
            //---------------------------------------------------------------------
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected void GetBounding(in DataPoint dataPoint)
            {
                double y = dataPoint.Y;

                // Produces better code than updating _bounding directly
                ref (double Min, double Max) bounding = ref _bounding;

                bounding.Min = y - _deadBandCompression.InstrumentPrecision;
                bounding.Max = y + _deadBandCompression.InstrumentPrecision;
            }
        }
        //---------------------------------------------------------------------
        private abstract class DeadBandCompressionEnumerableIterator : DeadBandCompressionIterator
        {
            protected DeadBandCompressionEnumerableIterator(DeadBandCompression deadBandCompression)
                : base(deadBandCompression)
            { }
            //-----------------------------------------------------------------
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override ref (bool Archive, bool MaxDelta) IsPointToArchive(in DataPoint incoming, in DataPoint lastArchived)
            {
                ref (bool Archive, bool MaxDelta) archive = ref _archive;

                if (!this.IsMaxDeltaX(ref archive, incoming, lastArchived))
                {
                    archive.Archive = incoming.Y < _bounding.Min || _bounding.Max < incoming.Y;
                }

                return ref archive;
            }
            //-----------------------------------------------------------------
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected void UpdatePoints(in DataPoint incoming, ref DataPoint snapShot)
            {
                _lastArchived = incoming;
                snapShot      = incoming;

                if (!_archive.MaxDelta) this.GetBounding(snapShot);
            }
        }
    }
}
