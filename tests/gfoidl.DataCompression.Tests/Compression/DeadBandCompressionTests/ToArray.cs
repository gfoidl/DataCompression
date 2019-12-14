using System;
using System.Collections.Generic;
using System.Linq;
using gfoidl.DataCompression.Wrappers;
using NUnit.Framework;

namespace gfoidl.DataCompression.Tests.Compression.DeadBandCompressionTests
{
    [TestFixture]
    public class ToArray
    {
        private static readonly DataPointSerializer s_ser = new DataPointSerializer();
        //---------------------------------------------------------------------
        [Test]
        public void Empty_IEnumerable___empty_result()
        {
            var sut  = new DeadBandCompression(0.1);
            var data = Empty();

            var actual = sut.Process(data);

            Assert.AreSame(Array.Empty<DataPoint>(), actual.ToArray());
            Assert.AreEqual(0, actual.ToList().Count);
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

            var actual = sut.Process(data);

            Assert.AreSame(Array.Empty<DataPoint>(), actual.ToArray());
            Assert.AreEqual(0, actual.ToList().Count);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Data_given_as_IEnumerable___OK()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = RawDataForTrend();
            var expected = ExpectedForTrend().ToList();

            var actual = sut.Process(data).ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Data_given_as_IList___OK()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = RawDataForTrend() .ToList().AsReadOnly();
            var expected = ExpectedForTrend().ToList();

            var actual = sut.Process(data).ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Data_given_as_List___OK()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = RawDataForTrend() .ToList();
            var expected = ExpectedForTrend().ToList();

            var actual = sut.Process(data).ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Data_given_as_Array___OK()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = RawDataForTrend() .ToArray();
            var expected = ExpectedForTrend().ToArray();

            var actual = sut.Process(data).ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Data_given_as_ListWrapper___OK()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = new ListWrapper<DataPoint>(RawDataForTrend().ToList());
            var expected = ExpectedForTrend().ToList();

            var actual = sut.Process(data).ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Data_given_as_ArrayWrapper___OK()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = new ArrayWrapper<DataPoint>(RawDataForTrend().ToArray());
            var expected = ExpectedForTrend().ToArray();

            var actual = sut.Process(data).ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Data_IEnumerable_with_maxDeltaX___OK()
        {
            var sut      = new DeadBandCompression(0.1, 4d);
            var data     = RawDataForMaxDelta();
            var expected = ExpectedForMaxDelta().ToList();

            var actual = sut.Process(data).ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Data_List_with_maxDeltaX___OK()
        {
            var sut      = new DeadBandCompression(0.1, 4d);
            var data     = RawDataForMaxDelta().ToList();
            var expected = ExpectedForMaxDelta().ToList();

            var actual = sut.Process(data).ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Data_IList_with_maxDeltaX___OK()
        {
            var sut      = new DeadBandCompression(0.1, 4d);
            var data     = RawDataForMaxDelta().ToList().AsReadOnly();
            var expected = ExpectedForMaxDelta().ToList();

            var actual = sut.Process(data).ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void One_DataPoint_IEnumerable___OK()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = RawDataForMaxDelta().Take(1);
            var expected = RawDataForMaxDelta().Take(1).ToList();

            var actual = sut.Process(data).ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void One_DataPoint_List___OK()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = RawDataForMaxDelta().Take(1).ToList();
            var expected = RawDataForMaxDelta().Take(1).ToList();

            var actual = sut.Process(data).ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void One_DataPoint_IList___OK()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = RawDataForMaxDelta().Take(1).ToList().AsReadOnly();
            var expected = RawDataForMaxDelta().Take(1).ToList();

            var actual = sut.Process(data).ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Two_DataPoint_IEnumerable___OK()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = RawDataForMaxDelta().Take(2);
            var expected = RawDataForMaxDelta().Take(2).ToList();

            var actual = sut.Process(data).ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Two_DataPoint_List___OK()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = RawDataForMaxDelta().Take(2).ToList();
            var expected = RawDataForMaxDelta().Take(2).ToList();

            var actual = sut.Process(data).ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Two_DataPoint_IList___OK()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = RawDataForMaxDelta().Take(2).ToList().AsReadOnly();
            var expected = RawDataForMaxDelta().Take(2).ToList();

            var actual = sut.Process(data).ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void IEnumerable_iterated_and_ToArray___OK()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = RawDataForTrend();
            var expected = ExpectedForTrend().ToList();

            DataPointIterator dataPointIterator = sut.Process(data);

            DataPointIterator enumerator = dataPointIterator.GetEnumerator();
            enumerator.MoveNext();
            enumerator.MoveNext();
            var actual = dataPointIterator.ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void List_iterated_and_ToArray___OK()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = RawDataForTrend().ToList();
            var expected = ExpectedForTrend().ToList();

            DataPointIterator dataPointIterator = sut.Process(data);

            DataPointIterator enumerator = dataPointIterator.GetEnumerator();
            enumerator.MoveNext();
            enumerator.MoveNext();
            var actual = dataPointIterator.ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        private static IEnumerable<DataPoint> RawDataForTrend()     => s_ser.Read("../../../../../doc/data/dead-band/trend_raw.csv");
        private static IEnumerable<DataPoint> ExpectedForTrend()    => s_ser.Read("../../../../../doc/data/dead-band/trend_compressed.csv");
        private static IEnumerable<DataPoint> RawDataForMaxDelta()  => s_ser.Read("../../../../../doc/data/dead-band/maxDelta_raw.csv");
        private static IEnumerable<DataPoint> ExpectedForMaxDelta() => s_ser.Read("../../../../../doc/data/dead-band/maxDelta_compressed.csv");
    }
}
