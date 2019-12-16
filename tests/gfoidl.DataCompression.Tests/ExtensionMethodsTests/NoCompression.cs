using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace gfoidl.DataCompression.Tests.ExtensionMethodsTests
{
    public class NoCompression : Base
    {
        [Test]
        public void Data_is_null___throws_ArgumentNull()
        {
            IEnumerable<DataPoint> data = null;

            Assert.Throws<ArgumentNullException>(() => data.NoCompression());
        }
        //---------------------------------------------------------------------
        [Test]
        public void DataPoints_given___OK()
        {
            IEnumerable<DataPoint> dataPoints = GetDataPoints();

            DataPointIterator actual = dataPoints.NoCompression();

            Assert.IsNotNull(actual);
        }
        //---------------------------------------------------------------------
#if NETCOREAPP
        [Test]
        public void DataPoints_given_async___OK()
        {
            IAsyncEnumerable<DataPoint> dataPoints = GetDataPointsAsync();

            DataPointIterator actual = dataPoints.NoCompressionAsync();

            Assert.IsNotNull(actual);
        }
#endif
    }
}
