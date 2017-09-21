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
            this.MaxDeltaX = maxDeltaX;
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
            throw new NotImplementedException();
        }
        //---------------------------------------------------------------------
        private IEnumerable<DataPoint> ProcessCore(IList<DataPoint> data)
        {
            throw new NotImplementedException();
        }
    }
}