// (c) gfoidl, all rights reserved

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace gfoidl.DataCompression.Tests.Compression.SwingingDoorCompressionTests
{
    public class MoveNext : Base
    {
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
        [Test]
        public void Empty_IEnumerable___empty_result()
        {
            var sut      = new SwingingDoorCompression(1);
            var data     = Empty();
            var iterator = sut.Process(data).GetEnumerator();

            Assert.IsFalse(iterator.MoveNext());
        }
        //---------------------------------------------------------------------
        [Test]
        public void Empty_IEnumerable_foreach___empty_result()
        {
            var sut  = new SwingingDoorCompression(1);
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
            var sut      = new SwingingDoorCompression(1);
            var data     = Array.Empty<DataPoint>();
            var iterator = sut.Process(data).GetEnumerator();

            Assert.IsFalse(iterator.MoveNext());
        }
        //---------------------------------------------------------------------
        [Test]
        public void Known_sequence___correct_result()
        {
            var sut      = new SwingingDoorCompression(1);
            var data     = KnownSequence();
            var iterator = sut.Process(data).GetEnumerator();
            var expected = KnownSequenceExpected().ToArray();

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
            var sut      = new SwingingDoorCompression(1);
            var data     = KnownSequence().ToArray();
            var iterator = sut.Process(data).GetEnumerator();
            var expected = KnownSequenceExpected().ToArray();

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
            var sut      = new SwingingDoorCompression(1);
            var data     = KnownSequence();
            var result   = sut.Process(data);
            var expected = KnownSequenceExpected().ToList();
            var actual   = new List<DataPoint>();

            foreach (DataPoint dp in result)
                actual.Add(dp);

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Known_sequence_as_array_foreach___correct_result()
        {
            var sut      = new SwingingDoorCompression(1);
            var data     = KnownSequence().ToArray();
            var result   = sut.Process(data);
            var expected = KnownSequenceExpected().ToArray();
            var actual   = new List<DataPoint>();

            foreach (DataPoint dp in result)
                actual.Add(dp);

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Known_sequence_ToArray___correct_result()
        {
            var sut      = new SwingingDoorCompression(1);
            var data     = KnownSequence();
            var result   = sut.Process(data);
            var expected = KnownSequenceExpected().ToArray();

            var actual = result.ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Known_sequence_with_constant_part_foreach____correct_result()
        {
            var sut      = new SwingingDoorCompression(1);
            var data     = KnownSequenceWithConstantPart();
            var result   = sut.Process(data);
            var expected = KnownSequenceWithConstantPartExpected().ToArray();
            var actual   = new List<DataPoint>();

            foreach (DataPoint dp in result)
                actual.Add(dp);

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Known_sequence_with_constant_part_ToArray____correct_result()
        {
            var sut      = new SwingingDoorCompression(1);
            var data     = KnownSequenceWithConstantPart();
            var result   = sut.Process(data);
            var expected = KnownSequenceWithConstantPartExpected().ToArray();

            var actual = result.ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void CompressionDeviation_0___input_echoed()
        {
            var sut      = new SwingingDoorCompression(0);
            var data     = KnownSequence();
            var result   = sut.Process(data);
            var expected = KnownSequence().ToArray();

            var actual = result.ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void CompressionDeviation_0_as_array___input_echoed()
        {
            var sut      = new SwingingDoorCompression(0);
            var data     = KnownSequence().ToArray();
            var result   = sut.Process(data);
            var expected = KnownSequence().ToArray();

            var actual = result.ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void CompressionDeviation_0_as_array_foreach___input_echoed()
        {
            var sut      = new SwingingDoorCompression(0);
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
