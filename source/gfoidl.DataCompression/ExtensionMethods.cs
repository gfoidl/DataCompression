using System;
using System.Collections.Generic;

namespace gfoidl.DataCompression
{
    /// <summary>
    /// Extension methods for easy access and fluent interface to compression methods.
    /// </summary>
    public static class ExtensionMethods
    {
        //---------------------------------------------------------------------
        /// <summary>
        /// A filter that performs dead band compression.
        /// </summary>
        /// <param name="data">Input data</param>
        /// <param name="instrumentPrecision">(Absolut) precision of the instrument</param>
        /// <param name="maxDeltaX">
        /// Length of x before for sure a value gets recoreded. See <see cref="DeadBandCompression.MaxDeltaX" />.
        /// </param>
        /// <returns>Dead band compressed / filtered data.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="data" /> is <c>null</c>
        /// </exception>
        public static IEnumerable<DataPoint> DeadBandCompression(this IEnumerable<DataPoint> data, double instrumentPrecision, double? maxDeltaX = null)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            var compression = new DeadBandCompression(instrumentPrecision, maxDeltaX);
            return compression.Process(data);
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  A filter that performs dead band compression.
        /// </summary>
        /// <param name="data">Input data</param>
        /// <param name="instrumentPrecision">(Absolut) precision of the instrument</param>
        /// <param name="maxTime">Length of time before for sure a value gets recoreded</param>
        /// <returns>Dead band compressed / filtered data.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="data" /> is <c>null</c>
        /// </exception>
        public static IEnumerable<DataPoint> DeadBandCompression(this IEnumerable<DataPoint> data, double instrumentPrecision, TimeSpan maxTime)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            var compression = new DeadBandCompression(instrumentPrecision, maxTime);
            return compression.Process(data);
        }
    }
}