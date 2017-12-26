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
        public SwingingDoorCompression(in double compressionDeviation, in double? maxDeltaX = null, in double? minDeltaX = null)
        {
            this.CompressionDeviation = compressionDeviation;
            this.MaxDeltaX            = maxDeltaX;
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
        public SwingingDoorCompression(in double compressionDeviation, in TimeSpan maxTime, in TimeSpan? minTime)
            : this(compressionDeviation, maxTime.Ticks, minTime?.Ticks)
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

            DataPoint lastArchived = dataEnumerator.Current;
            yield return lastArchived;

            DataPoint snapShot                       = lastArchived;
            DataPoint incoming                       = snapShot;
            (double SlopeMax, double SlopeMin) slope = default;
            this.OpenNewDoor(ref lastArchived, ref snapShot, lastArchived, ref slope);

            while (dataEnumerator.MoveNext())
            {
                incoming                = dataEnumerator.Current;
                var (archive, maxDelta) = this.IsPointToArchive(lastArchived, slope, incoming);

                if (!archive)
                {
                    this.CloseTheDoor(lastArchived, ref snapShot, incoming, ref slope);
                    continue;
                }

                if (!maxDelta)
                    yield return snapShot;

                if (this.MinDeltaX.HasValue)
                    while (dataEnumerator.MoveNext())
                    {
                        incoming = dataEnumerator.Current;
                        if ((incoming.X - snapShot.X) > this.MinDeltaX.Value) break;
                    }

                yield return incoming;

                this.OpenNewDoor(ref lastArchived, ref snapShot, incoming, ref slope);
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

            DataPoint lastArchived = data[0];
            yield return lastArchived;

            DataPoint snapShot                       = default;
            DataPoint incoming                       = default;
            (double SlopeMax, double SlopeMin) slope = default;
            this.OpenNewDoor(ref lastArchived, ref snapShot, lastArchived, ref slope);

            int n = data.Count;
            for (int i = 1; i < n; ++i)
            {
                incoming                = data[i];
                var (archive, maxDelta) = this.IsPointToArchive(lastArchived, slope, incoming);

                if (!archive)
                {
                    this.CloseTheDoor(lastArchived, ref snapShot, incoming, ref slope);
                    continue;
                }

                if (!maxDelta)
                    yield return snapShot;

                if (this.MinDeltaX.HasValue)
                    for (; i < data.Count; ++i)
                    {
                        incoming = data[i];
                        if ((incoming.X - snapShot.X) > this.MinDeltaX.Value) break;
                    }

                yield return incoming;

                this.OpenNewDoor(ref lastArchived, ref snapShot, incoming, ref slope);
            }

            if (incoming != lastArchived)
                yield return incoming;
        }
        //---------------------------------------------------------------------
        private (bool archive, bool maxDelta) IsPointToArchive(in DataPoint lastArchived, in (double slopeMax, double slopeMin) slope, in DataPoint incoming)
        {
            if ((incoming.X - lastArchived.X) >= (this.MaxDeltaX ?? double.MaxValue)) return (true, true);

            // Better to compare via gradient (1 calculation) than comparing to allowed y-values (2 calcuations)
            // Obviously, the result should be the same ;-)
            double slopeToIncoming = lastArchived.Gradient(incoming);

            return (slopeToIncoming < slope.slopeMin || slope.slopeMax < slopeToIncoming, false);
        }
        //---------------------------------------------------------------------
        private void CloseTheDoor(
            in DataPoint lastArchived,
            ref DataPoint snapShot,
            in DataPoint incoming,
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
            in DataPoint incoming,
            ref (double SlopeMax, double SlopeMin) slope)
        {
            lastArchived = incoming;
            snapShot     = lastArchived;
            slope        = (double.PositiveInfinity, double.NegativeInfinity);
        }
    }
}