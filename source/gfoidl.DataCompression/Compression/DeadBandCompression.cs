using System;
using System.Collections.Generic;

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
        public double? MaxDeltaX { get; }
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
        public DeadBandCompression(in double instrumentPrecision, in double? maxDeltaX = null)
        {
            this.InstrumentPrecision = instrumentPrecision;
            this.MaxDeltaX           = maxDeltaX;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Creates a new instance of dead band compression.
        /// </summary>
        /// <param name="instrumentPrecision">
        /// (Absolut) precision of the instrument. Cf. ExDev in documentation.
        /// </param>
        /// <param name="maxTime">Length of time before for sure a value gets recoreded</param>
        public DeadBandCompression(in double instrumentPrecision, in TimeSpan maxTime)
        : this(instrumentPrecision, maxTime.Ticks)
        { }
        //---------------------------------------------------------------------
        /// <summary>
        /// Implementation of the compression / filtering.
        /// </summary>
        /// <typeparam name="TList">The type of the enumeration / list.</typeparam>
        /// <param name="data">Input data</param>
        /// <returns>The compressed / filtered data.</returns>
        protected override IEnumerable<DataPoint> ProcessCore<TList>(in TList data)
        {
            if (data is List<DataPoint> list)
            {
                var wrapper = new ListWrapper(list);
                return this.ProcessCoreImpl(wrapper);
            }
            else if (data is DataPoint[] array)
            {
                var wrapper = new ArrayWrapper(array);
                return this.ProcessCoreImpl(wrapper);
            }
            else
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
                incoming                = dataEnumerator.Current;
                var (archive, maxDelta) = this.IsPointToArchive(lastArchived, bounding, incoming);

                if (!archive)
                {
                    snapShot = incoming;
                    continue;
                }

                if (!maxDelta)
                    yield return snapShot;

                yield return incoming;

                this.UpdatePoints(ref snapShot, ref lastArchived, incoming, maxDelta, ref bounding);
            }

            if (incoming != lastArchived)
                yield return incoming;
        }
        //---------------------------------------------------------------------
        private IEnumerable<DataPoint> ProcessCoreImpl<TList>(TList data) where TList : IList<DataPoint>
        {
            if (data.Count < 2)
            {
                foreach (var dp in data)
                    yield return dp;

                yield break;
            }

            DataPoint snapShot     = data[0];
            DataPoint lastArchived = snapShot;
            DataPoint incoming     = default;
            yield return snapShot;

            (double Min, double Max) bounding = this.GetBounding(snapShot);

            int n = data.Count;
            for (int i = 1; i < n; ++i)
            {
                incoming                = data[i];
                var (archive, maxDelta) = this.IsPointToArchive(lastArchived, bounding, incoming);

                if (!archive)
                {
                    snapShot = incoming;
                    continue;
                }

                if (!maxDelta)
                    yield return snapShot;

                yield return incoming;

                this.UpdatePoints(ref snapShot, ref lastArchived, incoming, maxDelta, ref bounding);
            }

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
        private (bool archive, bool maxDelta) IsPointToArchive(in DataPoint lastArchived, in (double Min, double Max) bounding, in DataPoint incoming)
        {
            if ((incoming.X - lastArchived.X) >= (this.MaxDeltaX ?? double.MaxValue)) return (true, true);

            return (incoming.Y < bounding.Min || bounding.Max < incoming.Y, false);
        }
        //---------------------------------------------------------------------
        private void UpdatePoints(
            ref DataPoint snapShot,
            ref DataPoint lastArchived,
            in DataPoint incoming,
            bool          maxDelta,
            ref (double, double) bounding)
        {
            snapShot     = incoming;
            lastArchived = incoming;

            if (!maxDelta) bounding = this.GetBounding(snapShot);
        }
    }
}