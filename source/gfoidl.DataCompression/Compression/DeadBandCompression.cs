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
        public DeadBandCompression(double instrumentPrecision, double? maxDeltaX = null)
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
        public DeadBandCompression(double instrumentPrecision, TimeSpan maxTime)
        : this(instrumentPrecision, maxTime.Ticks)
        { }
        //---------------------------------------------------------------------
        /// <summary>
        /// Implementation of the compression / filtering.
        /// </summary>
        /// <param name="data">Input data</param>
        /// <returns>The compressed / filtered data.</returns>
        protected override IEnumerable<DataPoint> ProcessCore(IEnumerable<DataPoint> data)
        {
            if (data is IList<DataPoint> list) return this.ProcessCore(list);

            return this.ProcessCore(data.GetEnumerator());
        }
        //---------------------------------------------------------------------
        private IEnumerable<DataPoint> ProcessCore(IEnumerator<DataPoint> dataEnumerator)
        {
            if (!dataEnumerator.MoveNext()) yield break;

            DataPoint snapShot     = dataEnumerator.Current;
            DataPoint lastArchived = snapShot;
            DataPoint incoming     = snapShot;      // sentinel, null would be possible but to much work around
            yield return snapShot;

            (double Min, double Max) bounding = this.GetBounding(snapShot);

            while (dataEnumerator.MoveNext())
            {
                incoming    = dataEnumerator.Current;
                var archive = this.IsPointToArchive(incoming, bounding, lastArchived);

                if (!archive.Archive)
                {
                    snapShot = incoming;
                    continue;
                }

                if (!archive.MaxDelta)
                    yield return snapShot;

                yield return incoming;

                this.UpdatePoints(ref snapShot, ref lastArchived, ref bounding, incoming, archive.MaxDelta);
            }

            if (incoming != lastArchived)
                yield return incoming;
        }
        //---------------------------------------------------------------------
        private IEnumerable<DataPoint> ProcessCore(IList<DataPoint> data)
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

            for (int i = 1; i < data.Count; ++i)
            {
                incoming    = data[i];
                var archive = this.IsPointToArchive(incoming, bounding, lastArchived);

                if (!archive.Archive)
                {
                    snapShot = incoming;
                    continue;
                }

                if (!archive.MaxDelta)
                    yield return snapShot;

                yield return incoming;

                this.UpdatePoints(ref snapShot, ref lastArchived, ref bounding, incoming, archive.MaxDelta);
            }

            yield return incoming;
        }
        //---------------------------------------------------------------------
        private (double Min, double Max) GetBounding(DataPoint snapShot)
        {
            double min = snapShot.Y - this.InstrumentPrecision;
            double max = snapShot.Y + this.InstrumentPrecision;

            return (min, max);
        }
        //---------------------------------------------------------------------
        private (bool Archive, bool MaxDelta) IsPointToArchive(DataPoint incoming, (double Min, double Max) bounding, DataPoint lastArchived)
        {
            if ((incoming.X - lastArchived.X) >= (this.MaxDeltaX ?? double.MaxValue)) return (true, true);

            return (incoming.Y < bounding.Min || bounding.Max < incoming.Y, false);
        }
        //---------------------------------------------------------------------
        private void UpdatePoints(
            ref DataPoint snapShot,
            ref DataPoint lastArchived,
            ref (double,  double) bounding,
            DataPoint     incoming,
            bool          maxDelta)
        {
            snapShot     = incoming;
            lastArchived = incoming;

            if (!maxDelta) bounding = this.GetBounding(snapShot);
        }
    }
}