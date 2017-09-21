using System;
using System.Collections.Generic;

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
        /// Creates a new instance of swinging door compression.
        /// </summary>
        /// <param name="compressionDeviation">
        /// (Absolut) Compression deviation applied to the y values to calculate the
        /// min and max slopes. Cf. CompDev in documentation.
        /// </param>
        /// <param name="maxDeltaX">
        /// Length of x before for sure a value gets recoreded. See <see cref="MaxDeltaX"/>.
        /// </param>
        public SwingingDoorCompression(double compressionDeviation, double? maxDeltaX = null)
        {
            this.CompressionDeviation = compressionDeviation;
            this.MaxDeltaX            = maxDeltaX;
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
        public SwingingDoorCompression(double compressionDeviation, TimeSpan maxTime)
            : this(compressionDeviation, maxTime.Ticks)
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

            DataPoint lastArchived = dataEnumerator.Current;
            yield return lastArchived;

            DataPoint snapShot                       = lastArchived;
            DataPoint incoming                       = snapShot;
            (double SlopeMax, double SlopeMin) slope = default;
            this.OpenNewDoor(ref lastArchived, ref snapShot, lastArchived, ref slope);

            while (dataEnumerator.MoveNext())
            {
                incoming    = dataEnumerator.Current;
                var archive = this.IsPointToArchive(lastArchived, slope, incoming);

                if (!archive.Archive)
                {
                    this.CloseTheDoor(lastArchived, ref snapShot, incoming, ref slope);
                    continue;
                }

                if (!archive.MaxDelta)
                    yield return snapShot;

                yield return incoming;

                this.OpenNewDoor(ref lastArchived, ref snapShot, incoming, ref slope);
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

            DataPoint lastArchived = data[0];
            yield return lastArchived;

            DataPoint snapShot                       = default;
            DataPoint incoming                       = default;
            (double SlopeMax, double SlopeMin) slope = default;
            this.OpenNewDoor(ref lastArchived, ref snapShot, lastArchived, ref slope);

            for (int i = 1; i < data.Count; ++i)
            {
                incoming    = data[i];
                var archive = this.IsPointToArchive(lastArchived, slope, incoming);

                if (!archive.Archive)
                {
                    this.CloseTheDoor(lastArchived, ref snapShot, incoming, ref slope);
                    continue;
                }

                if (!archive.MaxDelta)
                    yield return snapShot;

                yield return incoming;

                this.OpenNewDoor(ref lastArchived, ref snapShot, incoming, ref slope);
            }

            if (incoming != lastArchived)
                yield return incoming;
        }
        //---------------------------------------------------------------------
        private (bool Archive, bool MaxDelta) IsPointToArchive(DataPoint lastArchived, (double slopeMax, double slopeMin) slope, DataPoint incoming)
        {
            if ((incoming.X - lastArchived.X) >= (this.MaxDeltaX ?? double.MaxValue)) return (true, true);

            // Better to compare via gradient (1 calculation) than comparing to allowed y-values (2 calcuations)
            // Obviously, the result should be the same ;-)
            double slopeToIncoming = lastArchived.Gradient(incoming);

            return (slopeToIncoming < slope.slopeMin || slope.slopeMax < slopeToIncoming, false);
        }
        //---------------------------------------------------------------------
        private void CloseTheDoor(
            DataPoint     lastArchived,
            ref DataPoint snapShot,
            DataPoint     incoming,
            ref (double SlopeMax, double SlopeMin) slope)
        {
            double upperSlope = lastArchived.Gradient((incoming.X, incoming.Y + this.CompressionDeviation));
            double lowerSlope = lastArchived.Gradient((incoming.X, incoming.Y - this.CompressionDeviation));

            slope.SlopeMax = Math.Min(slope.SlopeMax, upperSlope);
            slope.SlopeMin = Math.Max(slope.SlopeMin, lowerSlope);
            snapShot       = incoming;
        }
        //---------------------------------------------------------------------
        private void OpenNewDoor(
            ref DataPoint lastArchived,
            ref DataPoint snapShot,
            DataPoint     incoming,
            ref (double SlopeMax, double SlopeMin) slope)
        {
            lastArchived = incoming;
            snapShot     = lastArchived;
            slope        = (double.PositiveInfinity, double.NegativeInfinity);
        }
    }
}