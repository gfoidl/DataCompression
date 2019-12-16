using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace gfoidl.DataCompression.Tests.ExtensionMethodsTests
{
    public class DeadBandCompression : Base
    {
        [Test]
        public void Data_is_null___throws_ArgumentNull()
        {
            IEnumerable<DataPoint> data = null;

            Assert.Throws<ArgumentNullException>(() => data.DeadBandCompression(0.1));
        }
        //---------------------------------------------------------------------
        [Test]
        public void DataPoints_given___OK()
        {
            IEnumerable<DataPoint> dataPoints = GetDataPoints();

            DataPointIterator actual = dataPoints.DeadBandCompression(0.1);

            Assert.IsNotNull(actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void DataPoints_given_with_TimeSpan___OK()
        {
            IEnumerable<DataPoint> dataPoints = GetDataPoints();

            DataPointIterator actual = dataPoints.DeadBandCompression(0.1, TimeSpan.FromSeconds(1));

            Assert.IsNotNull(actual);
        }
        //---------------------------------------------------------------------
#if NETCOREAPP
        [Test]
        public void DataPoints_given_async___OK()
        {
            IAsyncEnumerable<DataPoint> dataPoints = GetDataPointsAsync();

            DataPointAsyncIterator actual = dataPoints.DeadBandCompression(0.1);

            Assert.IsNotNull(actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void DataPoints_given_with_TimeSpan_async___OK()
        {
            IAsyncEnumerable<DataPoint> dataPoints = GetDataPointsAsync();

            DataPointAsyncIterator actual = dataPoints.DeadBandCompression(0.1, TimeSpan.FromSeconds(1));

            Assert.IsNotNull(actual);
        }
#endif
    }
}
