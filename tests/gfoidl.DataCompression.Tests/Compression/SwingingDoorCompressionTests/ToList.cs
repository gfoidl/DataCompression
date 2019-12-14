using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace gfoidl.DataCompression.Tests.Compression.SwingingDoorCompressionTests
{
    [TestFixture]
    public class ToList
    {
        private static readonly DataPointSerializer s_ser = new DataPointSerializer();
        //---------------------------------------------------------------------
        [Test]
        public void Data_given_as_IEnumerable___OK()
        {
            var sut      = new SwingingDoorCompression(1d);
            var data     = RawDataForTrend();
            var expected = ExpectedForTrend().ToList();

            var actual = sut.Process(data).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Data_given_as_List___OK()
        {
            var sut      = new SwingingDoorCompression(1d);
            var data     = RawDataForTrend().ToList();
            var expected = ExpectedForTrend().ToList();

            var actual = sut.Process(data).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Data_given_as_IList___OK()
        {
            var sut      = new SwingingDoorCompression(1d);
            var data     = RawDataForTrend().ToList().AsReadOnly();
            var expected = ExpectedForTrend().ToList();

            var actual = sut.Process(data).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Data_IEnumerable_with_maxDeltaX___OK()
        {
            var sut      = new SwingingDoorCompression(1d, 6d);
            var data     = RawDataForMaxDelta();
            var expected = ExpectedForMaxDelta().ToList();

            var actual = sut.Process(data).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Data_List_with_maxDeltaX___OK()
        {
            var sut      = new SwingingDoorCompression(1d, 6d);
            var data     = RawDataForMaxDelta().ToList();
            var expected = ExpectedForMaxDelta().ToList();

            var actual = sut.Process(data).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Data_IList_with_maxDeltaX___OK()
        {
            var sut      = new SwingingDoorCompression(1d, 6d);
            var data     = RawDataForMaxDelta().ToList().AsReadOnly();
            var expected = ExpectedForMaxDelta().ToList();

            var actual = sut.Process(data).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void One_DataPoint_IEnumerable___OK()
        {
            var sut      = new SwingingDoorCompression(1d);
            var data     = RawDataForMaxDelta().Take(1);
            var expected = RawDataForMaxDelta().Take(1).ToList();

            var actual = sut.Process(data).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void One_DataPoint_List___OK()
        {
            var sut      = new SwingingDoorCompression(1d);
            var data     = RawDataForMaxDelta().Take(1).ToList();
            var expected = RawDataForMaxDelta().Take(1).ToList();

            var actual = sut.Process(data).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void One_DataPoint_IList___OK()
        {
            var sut      = new SwingingDoorCompression(1d);
            var data     = RawDataForMaxDelta().Take(1).ToList().AsReadOnly();
            var expected = RawDataForMaxDelta().Take(1).ToList();

            var actual = sut.Process(data).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Two_DataPoint_IEnumerable___OK()
        {
            var sut      = new SwingingDoorCompression(1d);
            var data     = RawDataForMaxDelta().Take(2);
            var expected = RawDataForMaxDelta().Take(2).ToList();

            var actual = sut.Process(data).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Two_DataPoint_List___OK()
        {
            var sut      = new SwingingDoorCompression(1d);
            var data     = RawDataForMaxDelta().Take(2).ToList();
            var expected = RawDataForMaxDelta().Take(2).ToList();

            var actual = sut.Process(data).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Two_DataPoint_IList___OK()
        {
            var sut      = new SwingingDoorCompression(1d);
            var data     = RawDataForMaxDelta().Take(2).ToList().AsReadOnly();
            var expected = RawDataForMaxDelta().Take(2).ToList();

            var actual = sut.Process(data).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void MinDeltaX_IEnumerable___OK()
        {
            var sut      = new SwingingDoorCompression(1d, minDeltaX: 1d);
            var data     = RawMinDeltaX();
            var expected = ExpectedMinDeltaX().ToList();

            var actual = sut.Process(data).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void MinDeltaX_List___OK()
        {
            var sut      = new SwingingDoorCompression(1d, minDeltaX: 1d);
            var data     = RawMinDeltaX().ToList();
            var expected = ExpectedMinDeltaX().ToList();

            var actual = sut.Process(data).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void MinDeltaX_IList___OK()
        {
            var sut      = new SwingingDoorCompression(1d, minDeltaX: 1d);
            var data     = RawMinDeltaX().ToList().AsReadOnly();
            var expected = ExpectedMinDeltaX().ToList();

            var actual = sut.Process(data).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void IEnumerable_iterated_and_ToArray___OK()
        {
            var sut      = new SwingingDoorCompression(1d);
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
            var sut      = new SwingingDoorCompression(1d);
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
        private static IEnumerable<DataPoint> RawDataForTrend()     => s_ser.Read("../../../../../doc/data/swinging-door/trend_raw.csv");
        private static IEnumerable<DataPoint> ExpectedForTrend()    => s_ser.Read("../../../../../doc/data/swinging-door/trend_compressed.csv");
        private static IEnumerable<DataPoint> RawDataForMaxDelta()  => s_ser.Read("../../../../../doc/data/swinging-door/maxDelta_raw.csv");
        private static IEnumerable<DataPoint> ExpectedForMaxDelta() => s_ser.Read("../../../../../doc/data/swinging-door/maxDelta_compressed.csv");
        //---------------------------------------------------------------------
        private static IEnumerable<DataPoint> RawMinDeltaX()
        {
            yield return (0d, 2d);
            yield return (1d, 2d);
            yield return (2d, 2d);
            yield return (3d, 2d);
            yield return (4d, 2d);
            yield return (5d, 10d);
            yield return (6d, 10d);
            yield return (7d, 10d);
            yield return (8d, 10d);
            yield return (9d, 10d);
        }
        //---------------------------------------------------------------------
        private static IEnumerable<DataPoint> ExpectedMinDeltaX()
        {
            yield return (0d, 2d);
            yield return (4d, 2d);
            yield return (6d, 10d);
            yield return (9d, 10d);
        }
    }
}
