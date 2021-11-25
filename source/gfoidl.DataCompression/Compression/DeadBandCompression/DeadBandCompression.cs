// (c) gfoidl, all rights reserved

using System;
using System.Collections.Generic;
using System.Threading;
using gfoidl.DataCompression.Internal.DeadBand;
using gfoidl.DataCompression.Wrappers;

namespace gfoidl.DataCompression
{
    /// <summary>
    /// Dead band compression.
    /// </summary>
    /// <remarks>
    /// See documentation for further information.
    /// </remarks>
    public sealed class DeadBandCompression : Compression
    {
#if NETSTANDARD2_1
        internal AsyncEnumerableIterator?      _cachedAsyncEnumerableIterator;
#endif
        internal SequentialEnumerableIterator? _cachedSequentialEnumerableIterator;
        internal DataPointIterator?            _cachedIndexedIterator;       // due to generics use base class
        //---------------------------------------------------------------------
        /// <summary>
        /// (Absolut) precision of the instrument.
        /// </summary>
        /// <remarks>
        /// Cf. ExDev in documentation.
        /// </remarks>
        public double InstrumentPrecision { get; set; }
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
        /// <inheritdoc/>
        public override bool ArchiveIncoming => true;
        //---------------------------------------------------------------------
        /// <inheritdoc />
        protected override DataPointIterator ProcessCore(IEnumerable<DataPoint> data)
        {
            if (data is ArrayWrapper<DataPoint> arrayWrapper)
            {
                if (arrayWrapper.Count == 0) return DataPointIterator.Empty;

                DataPointIterator? cached = Interlocked.Exchange(ref _cachedIndexedIterator, null);

                if (cached is not IndexedIterator<ArrayWrapper<DataPoint>> iter)
                {
                    Interlocked.CompareExchange(ref _cachedIndexedIterator, cached, null);
                    iter = new IndexedIterator<ArrayWrapper<DataPoint>>();
                }

                iter.SetData(this, arrayWrapper);
                return iter;
            }

            if (data is ListWrapper<DataPoint> listWrapper)
            {
                if (listWrapper.Count == 0) return DataPointIterator.Empty;

                DataPointIterator? cached = Interlocked.Exchange(ref _cachedIndexedIterator, null);

                if (cached is not IndexedIterator<ListWrapper<DataPoint>> iter)
                {
                    Interlocked.CompareExchange(ref _cachedIndexedIterator, cached, null);
                    iter = new IndexedIterator<ListWrapper<DataPoint>>();
                }

                iter.SetData(this, listWrapper);
                return iter;
            }

            if (data is DataPoint[] array)
            {
                if (array.Length == 0) return DataPointIterator.Empty;

                DataPointIterator? cached = Interlocked.Exchange(ref _cachedIndexedIterator, null);

                if (cached is not IndexedIterator<ArrayWrapper<DataPoint>> iter)
                {
                    Interlocked.CompareExchange(ref _cachedIndexedIterator, cached, null);
                    iter = new IndexedIterator<ArrayWrapper<DataPoint>>();
                }

                iter.SetData(this, new ArrayWrapper<DataPoint>(array));
                return iter;
            }

            if (data is List<DataPoint> list)
            {
                if (list.Count == 0) return DataPointIterator.Empty;

                DataPointIterator? cached = Interlocked.Exchange(ref _cachedIndexedIterator, null);

                if (cached is not IndexedIterator<ListWrapper<DataPoint>> iter)
                {
                    Interlocked.CompareExchange(ref _cachedIndexedIterator, cached, null);
                    iter = new IndexedIterator<ListWrapper<DataPoint>>();
                }

                iter.SetData(this, new ListWrapper<DataPoint>(list));
                return iter;
            }

            if (data is IList<DataPoint> ilist)
            {
                if (ilist.Count == 0) return DataPointIterator.Empty;

                DataPointIterator? cached = Interlocked.Exchange(ref _cachedIndexedIterator, null);

                if (cached is not IndexedIterator<IList<DataPoint>> iter)
                {
                    Interlocked.CompareExchange(ref _cachedIndexedIterator, cached, null);
                    iter = new IndexedIterator<IList<DataPoint>>();
                }

                iter.SetData(this, ilist);
                return iter;
            }

            SequentialEnumerableIterator seqIter = Interlocked.Exchange(ref _cachedSequentialEnumerableIterator, null)
                ?? new SequentialEnumerableIterator();

            seqIter.SetData(this, data);
            return seqIter;
        }
        //---------------------------------------------------------------------
#if NETSTANDARD2_1
        /// <inheritdoc />
        protected override DataPointIterator ProcessAsyncCore(IAsyncEnumerable<DataPoint> data)
        {
            AsyncEnumerableIterator iter = Interlocked.Exchange(ref _cachedAsyncEnumerableIterator, null)
                ?? new AsyncEnumerableIterator();

            iter.SetData(this, data);
            return iter;
        }
#endif
    }
}
