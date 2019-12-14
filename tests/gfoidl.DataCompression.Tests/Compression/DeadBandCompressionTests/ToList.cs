using System.Linq;
using NUnit.Framework;

namespace gfoidl.DataCompression.Tests.Compression.DeadBandCompressionTests
{
    public class ToList : Base
    {
        [Test]
        public void Data_given_as_IEnumerable___OK()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = RawDataForTrend();
            var expected = ExpectedForTrend().ToList();

            var actual = sut.Process(data).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Data_given_as_List___OK()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = RawDataForTrend().ToList();
            var expected = ExpectedForTrend().ToList();

            var actual = sut.Process(data).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Data_given_as_IList___OK()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = RawDataForTrend().ToList().AsReadOnly();
            var expected = ExpectedForTrend().ToList();

            var actual = sut.Process(data).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Data_IEnumerable_with_maxDeltaX___OK()
        {
            var sut      = new DeadBandCompression(0.1, 4d);
            var data     = RawDataForMaxDelta();
            var expected = ExpectedForMaxDelta().ToList();

            var actual = sut.Process(data).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Data_List_with_maxDeltaX___OK()
        {
            var sut      = new DeadBandCompression(0.1, 4d);
            var data     = RawDataForMaxDelta().ToList();
            var expected = ExpectedForMaxDelta().ToList();

            var actual = sut.Process(data).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Data_IList_with_maxDeltaX___OK()
        {
            var sut      = new DeadBandCompression(0.1, 4d);
            var data     = RawDataForMaxDelta().ToList().AsReadOnly();
            var expected = ExpectedForMaxDelta().ToList();

            var actual = sut.Process(data).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void One_DataPoint_IEnumerable___OK()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = RawDataForMaxDelta().Take(1);
            var expected = RawDataForMaxDelta().Take(1).ToList();

            var actual = sut.Process(data).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void One_DataPoint_List___OK()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = RawDataForMaxDelta().Take(1).ToList();
            var expected = RawDataForMaxDelta().Take(1).ToList();

            var actual = sut.Process(data).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void One_DataPoint_IList___OK()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = RawDataForMaxDelta().Take(1).ToList().AsReadOnly();
            var expected = RawDataForMaxDelta().Take(1).ToList();

            var actual = sut.Process(data).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Two_DataPoint_IEnumerable___OK()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = RawDataForMaxDelta().Take(2);
            var expected = RawDataForMaxDelta().Take(2).ToList();

            var actual = sut.Process(data).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Two_DataPoint_List___OK()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = RawDataForMaxDelta().Take(2).ToList();
            var expected = RawDataForMaxDelta().Take(2).ToList();

            var actual = sut.Process(data).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Two_DataPoint_IList___OK()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = RawDataForMaxDelta().Take(2).ToList().AsReadOnly();
            var expected = RawDataForMaxDelta().Take(2).ToList();

            var actual = sut.Process(data).ToList();

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
    }
}
