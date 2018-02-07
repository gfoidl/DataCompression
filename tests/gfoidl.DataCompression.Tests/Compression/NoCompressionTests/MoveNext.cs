using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace gfoidl.DataCompression.Tests.Compression.NoCompressionTests
{
    [TestFixture]
    public class MoveNext
    {
        private static readonly DataPointSerializer _ser = new DataPointSerializer();
        //---------------------------------------------------------------------
        [Test]
        public void MoveNext_without_GetEnumerator___throws_InvalidOperation()
        {
            var sut  = new NoCompression();
            var data = RawDataForTrend();

            var iterator = sut.Process(data);

            Assert.Throws<InvalidOperationException>(() => iterator.MoveNext());
        }
        //---------------------------------------------------------------------
        private static IEnumerable<DataPoint> RawDataForTrend() => _ser.Read("../../../../../doc/data/dead-band/trend_raw.csv");
    }
}
