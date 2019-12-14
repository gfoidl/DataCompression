using System;
using System.Collections.Generic;
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
    }
}
