using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace gfoidl.DataCompression.Tests.Compression.SwingingDoorCompressionTests
{
    [TestFixture]
    public class MoveNext
    {
        private static readonly DataPointSerializer _ser = new DataPointSerializer();
        //---------------------------------------------------------------------
        [Test]
        public void Enumerable_MoveNext_without_GetEnumerator___throws_InvalidOperation()
        {
            var sut  = new SwingingDoorCompression(1);
            var data = RawDataForTrend();

            var iterator = sut.Process(data);

            Assert.Throws<InvalidOperationException>(() => iterator.MoveNext());
        }
        //---------------------------------------------------------------------
        [Test]
        public void Indexed_MoveNext_without_GetEnumerator___throws_InvalidOperation()
        {
            var sut  = new SwingingDoorCompression(1);
            var data = RawDataForTrend().ToArray();

            var iterator = sut.Process(data);

            Assert.Throws<InvalidOperationException>(() => iterator.MoveNext());
        }
        //---------------------------------------------------------------------
        private static IEnumerable<DataPoint> RawDataForTrend() => _ser.Read("../../../../../doc/data/dead-band/trend_raw.csv");
    }
}
