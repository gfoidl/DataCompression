// (c) gfoidl, all rights reserved

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace gfoidl.DataCompression.Tests.Compression.SwingingDoorCompressionTests
{
    public class IteratorCaching : Base
    {
        [Test]
        public void Data_given_as_IEnumerable___OK()
        {
            var sut      = new SwingingDoorCompression(1);
            var data     = RawDataForTrend();
            var expected = ExpectedForTrend().ToList();

            var actual0 = new List<DataPoint>();
            foreach (DataPoint dp in sut.Process(data))
                actual0.Add(dp);

            var actual1 = new List<DataPoint>();
            foreach (DataPoint dp in sut.Process(data))
                actual1.Add(dp);

            CollectionAssert.AreEqual(expected, actual0);
            CollectionAssert.AreEqual(expected, actual1);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Iterator_Caching_IEnumerable___same_Instance()
        {
            var sut   = new SwingingDoorCompression(1);
            var data  = RawDataForTrend();
            var dummy = new List<DataPoint>();

            DataPointIterator actual0 = sut.Process(data.Skip(1));
            foreach (DataPoint dp in actual0)
                dummy.Add(dp);

            DataPointIterator actual1 = sut.Process(data.Take(1));
            foreach (DataPoint dp in actual1)
                dummy.Add(dp);

            Assert.AreSame(actual0, actual1);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Tow_Iterators_IEnumerable___different_Instances()
        {
            var sut   = new SwingingDoorCompression(1);
            var data  = RawDataForTrend();
            var dummy = new List<DataPoint>();

            DataPointIterator actual0 = sut.Process(data.Skip(1));
            DataPointIterator actual1 = sut.Process(data.Take(1));

            foreach (DataPoint dp in actual0)
                dummy.Add(dp);

            foreach (DataPoint dp in actual1)
                dummy.Add(dp);

            Assert.AreNotSame(actual0, actual1);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Iterator_Caching_List___same_Instance()
        {
            var sut   = new SwingingDoorCompression(1);
            var data  = RawDataForTrend();
            var dummy = new List<DataPoint>();

            DataPointIterator actual0 = sut.Process(data.Skip(1).ToList());
            foreach (DataPoint dp in actual0)
                dummy.Add(dp);

            DataPointIterator actual1 = sut.Process(data.Take(1).ToList());
            foreach (DataPoint dp in actual1)
                dummy.Add(dp);

            Assert.AreSame(actual0, actual1);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Two_Iterators_Caching_List___different_Instances()
        {
            var sut   = new SwingingDoorCompression(1);
            var data  = RawDataForTrend();
            var dummy = new List<DataPoint>();

            DataPointIterator actual0 = sut.Process(data.Skip(1).ToList());
            DataPointIterator actual1 = sut.Process(data.Take(1).ToList());

            foreach (DataPoint dp in actual0)
                dummy.Add(dp);

            foreach (DataPoint dp in actual1)
                dummy.Add(dp);

            Assert.AreNotSame(actual0, actual1);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Iterator_Caching_IList___same_Instance()
        {
            var sut   = new SwingingDoorCompression(1);
            var data  = RawDataForTrend();
            var dummy = new List<DataPoint>();

            DataPointIterator actual0 = sut.Process(data.Skip(1).ToList().AsReadOnly());
            foreach (DataPoint dp in actual0)
                dummy.Add(dp);

            DataPointIterator actual1 = sut.Process(data.Take(1).ToList().AsReadOnly());
            foreach (DataPoint dp in actual1)
                dummy.Add(dp);

            Assert.AreSame(actual0, actual1);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Tow_Iterators_Caching_IList___different_Instances()
        {
            var sut   = new SwingingDoorCompression(1);
            var data  = RawDataForTrend();
            var dummy = new List<DataPoint>();

            DataPointIterator actual0 = sut.Process(data.Skip(1).ToList().AsReadOnly());
            DataPointIterator actual1 = sut.Process(data.Take(1).ToList().AsReadOnly());

            foreach (DataPoint dp in actual0)
                dummy.Add(dp);

            foreach (DataPoint dp in actual1)
                dummy.Add(dp);

            Assert.AreNotSame(actual0, actual1);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Iterator_Caching_Array___same_Instance()
        {
            var sut   = new SwingingDoorCompression(1);
            var data  = RawDataForTrend();
            var dummy = new List<DataPoint>();

            DataPointIterator actual0 = sut.Process(data.Skip(1).ToArray());
            foreach (DataPoint dp in actual0)
                dummy.Add(dp);

            DataPointIterator actual1 = sut.Process(data.Take(1).ToArray());
            foreach (DataPoint dp in actual1)
                dummy.Add(dp);

            Assert.AreSame(actual0, actual1);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Tow_Iterators_Caching_Array___different_Instances()
        {
            var sut   = new SwingingDoorCompression(1);
            var data  = RawDataForTrend();
            var dummy = new List<DataPoint>();

            DataPointIterator actual0 = sut.Process(data.Skip(1).ToArray());
            DataPointIterator actual1 = sut.Process(data.Take(1).ToArray());

            foreach (DataPoint dp in actual0)
                dummy.Add(dp);

            foreach (DataPoint dp in actual1)
                dummy.Add(dp);

            Assert.AreNotSame(actual0, actual1);
        }
    }
}
