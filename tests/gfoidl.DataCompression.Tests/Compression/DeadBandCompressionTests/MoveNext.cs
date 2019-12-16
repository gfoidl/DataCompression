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
            var sut      = new DeadBandCompression(0.1);
            var data     = Empty();
            var iterator = sut.Process(data).GetEnumerator();

            Assert.IsFalse(iterator.MoveNext());
        }
        //---------------------------------------------------------------------
        [Test]
        public void Empty_IEnumerable_foreach___empty_result()
        {
            var sut  = new DeadBandCompression(0.1);
            var data = Empty();

            int count = 0;
            foreach (DataPoint dp in sut.Process(data))
            {
                count++;
            }

            Assert.AreEqual(0, count);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Empty_Array___empty_result()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = new DataPoint[0];
            var iterator = sut.Process(data).GetEnumerator();

            Assert.IsFalse(iterator.MoveNext());
        }
        //---------------------------------------------------------------------
        [Test]
        public void Known_sequence___correct_result()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = KnownSequence();
            var iterator = sut.Process(data).GetEnumerator();
            var expected = KnownSequence().ToArray();

            Assert.Multiple(() =>
            {
                int step = 0;
                Assert.IsTrue(iterator.MoveNext(), $"MoveNext step: {step}");
                Assert.AreEqual(expected[step], iterator.Current, $"Equal step: {step}");
                step++;
                Assert.IsTrue(iterator.MoveNext(), $"MoveNext step: {step}");
                Assert.AreEqual(expected[step], iterator.Current, $"Equal step: {step}");
                step++;
                Assert.IsTrue(iterator.MoveNext(), $"MoveNext step: {step}");
                Assert.AreEqual(expected[step], iterator.Current, $"Equal step: {step}");
                step++;
                Assert.IsFalse(iterator.MoveNext(), $"MoveNext step: {step++}");
            });
        }
        //---------------------------------------------------------------------
        [Test]
        public void Known_sequence_as_array___correct_result()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = KnownSequence().ToArray();
            var iterator = sut.Process(data).GetEnumerator();
            var expected = KnownSequence().ToArray();

            Assert.Multiple(() =>
            {
                int step = 0;
                Assert.IsTrue(iterator.MoveNext(), $"MoveNext step: {step}");
                Assert.AreEqual(expected[step], iterator.Current, $"Equal step: {step}");
                step++;
                Assert.IsTrue(iterator.MoveNext(), $"MoveNext step: {step}");
                Assert.AreEqual(expected[step], iterator.Current, $"Equal step: {step}");
                step++;
                Assert.IsTrue(iterator.MoveNext(), $"MoveNext step: {step}");
                Assert.AreEqual(expected[step], iterator.Current, $"Equal step: {step}");
                step++;
                Assert.IsFalse(iterator.MoveNext(), $"MoveNext step: {step++}");
            });
        }
        //---------------------------------------------------------------------
        [Test]
        public void Known_sequence_foreach___correct_result()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = KnownSequence();
            var result   = sut.Process(data);
            var expected = KnownSequence().ToList();
            var actual   = new List<DataPoint>();

            foreach (DataPoint dp in result)
                actual.Add(dp);

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Known_sequence_as_array_foreach___correct_result()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = KnownSequence().ToArray();
            var result   = sut.Process(data);
            var expected = KnownSequence().ToArray();
            var actual   = new List<DataPoint>();

            foreach (DataPoint dp in result)
                actual.Add(dp);

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
