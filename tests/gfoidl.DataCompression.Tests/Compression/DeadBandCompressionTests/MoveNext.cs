using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace gfoidl.DataCompression.Tests.Compression.DeadBandCompressionTests
{
    public class MoveNext : Base
    {
        [Test]
        public void MoveNext_without_GetEnumerator___throws_InvalidOperation()
        {
            var sut  = new DeadBandCompression(1);
            var data = RawDataForTrend();

            var iterator = sut.Process(data);

            Assert.Throws<InvalidOperationException>(() => iterator.MoveNext());
        }
        //---------------------------------------------------------------------
        [Test]
        public void Indexed_MoveNext_without_GetEnumerator___throws_InvalidOperation()
        {
            var sut  = new DeadBandCompression(1);
            var data = RawDataForTrend().ToArray();

            var iterator = sut.Process(data);

            Assert.Throws<InvalidOperationException>(() => iterator.MoveNext());
        }
        //---------------------------------------------------------------------
        [Test]
        public void Empty_IEnumerable___empty_result()
        {
            var sut  = new DeadBandCompression(0.1);
            var data = Empty();

            var iterator = sut.Process(data);

            Assert.IsFalse(iterator.MoveNext());
            //-----------------------------------------------------------------
            static IEnumerable<DataPoint> Empty()
            {
                yield break;
            }
        }
        //---------------------------------------------------------------------
        [Test]
        public void Empty_Array___empty_result()
        {
            var sut  = new DeadBandCompression(0.1);
            var data = new DataPoint[0];

            var iterator = sut.Process(data);

            Assert.IsFalse(iterator.MoveNext());
        }
    }
}
