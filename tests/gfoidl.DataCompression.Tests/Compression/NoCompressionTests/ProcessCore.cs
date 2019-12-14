using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace gfoidl.DataCompression.Tests.Compression.NoCompressionTests
{
    public class ProcessCore : Base
    {
        [Test]
        public void Data_given_as_IEnumerable___OK()
        {
            var sut      = new NoCompression();
            var data     = RawDataForTrend();
            var expected = RawDataForTrend().ToList();

            var actual = new List<DataPoint>();
            foreach (DataPoint dp in sut.Process(data))
                actual.Add(dp);

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Data_given_as_List___OK()
        {
            var sut      = new NoCompression();
            var data     = RawDataForTrend().ToList();
            var expected = RawDataForTrend().ToList();

            var actual = new List<DataPoint>();
            foreach (DataPoint dp in sut.Process(data))
                actual.Add(dp);

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Data_given_as_IList___OK()
        {
            var sut      = new NoCompression();
            var data     = RawDataForTrend().ToList().AsReadOnly();
            var expected = RawDataForTrend().ToList();

            var actual = new List<DataPoint>();
            foreach (DataPoint dp in sut.Process(data))
                actual.Add(dp);

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Data_IEnumerable_with_maxDeltaX___OK()
        {
            var sut      = new NoCompression();
            var data     = RawDataForMaxDelta();
            var expected = RawDataForMaxDelta().ToList();

            var actual = new List<DataPoint>();
            foreach (DataPoint dp in sut.Process(data))
                actual.Add(dp);

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Data_List_with_maxDeltaX___OK()
        {
            var sut      = new NoCompression();
            var data     = RawDataForMaxDelta().ToList();
            var expected = RawDataForMaxDelta().ToList();

            var actual = new List<DataPoint>();
            foreach (DataPoint dp in sut.Process(data))
                actual.Add(dp);

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Data_IList_with_maxDeltaX___OK()
        {
            var sut      = new NoCompression();
            var data     = RawDataForMaxDelta().ToList().AsReadOnly();
            var expected = RawDataForMaxDelta().ToList();

            var actual = new List<DataPoint>();
            foreach (DataPoint dp in sut.Process(data))
                actual.Add(dp);

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void One_DataPoint_IEnumerable___OK()
        {
            var sut      = new NoCompression();
            var data     = RawDataForMaxDelta().Take(1);
            var expected = RawDataForMaxDelta().Take(1).ToList();

            var actual = new List<DataPoint>();
            foreach (DataPoint dp in sut.Process(data))
                actual.Add(dp);

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void One_DataPoint_List___OK()
        {
            var sut      = new NoCompression();
            var data     = RawDataForMaxDelta().Take(1).ToList();
            var expected = RawDataForMaxDelta().Take(1).ToList();

            var actual = new List<DataPoint>();
            foreach (DataPoint dp in sut.Process(data))
                actual.Add(dp);

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void One_DataPoint_IList___OK()
        {
            var sut      = new NoCompression();
            var data     = RawDataForMaxDelta().Take(1).ToList().AsReadOnly();
            var expected = RawDataForMaxDelta().Take(1).ToList();

            var actual = new List<DataPoint>();
            foreach (DataPoint dp in sut.Process(data))
                actual.Add(dp);

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Two_DataPoint_IEnumerable___OK()
        {
            var sut      = new NoCompression();
            var data     = RawDataForMaxDelta().Take(2);
            var expected = RawDataForMaxDelta().Take(2).ToList();

            var actual = new List<DataPoint>();
            foreach (DataPoint dp in sut.Process(data))
                actual.Add(dp);

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Two_DataPoint_List___OK()
        {
            var sut      = new NoCompression();
            var data     = RawDataForMaxDelta().Take(2).ToList();
            var expected = RawDataForMaxDelta().Take(2).ToList();

            var actual = new List<DataPoint>();
            foreach (DataPoint dp in sut.Process(data))
                actual.Add(dp);

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Two_DataPoint_IList___OK()
        {
            var sut      = new NoCompression();
            var data     = RawDataForMaxDelta().Take(2).ToList().AsReadOnly();
            var expected = RawDataForMaxDelta().Take(2).ToList();

            var actual = new List<DataPoint>();
            foreach (DataPoint dp in sut.Process(data))
                actual.Add(dp);

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
