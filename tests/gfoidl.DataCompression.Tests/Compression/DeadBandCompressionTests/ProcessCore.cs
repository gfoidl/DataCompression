// (c) gfoidl, all rights reserved

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace gfoidl.DataCompression.Tests.Compression.DeadBandCompressionTests
{
    public class ProcessCore : Base
    {
        [Test]
        public void Data_given_as_IEnumerable___OK()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = RawDataForTrend();
            var expected = ExpectedForTrend().ToList();

            var actual = new List<DataPoint>();
            foreach (DataPoint dp in sut.Process(data))
                actual.Add(dp);

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Data_given_as_List___OK()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = RawDataForTrend().ToList();
            var expected = ExpectedForTrend().ToList();

            var actual = new List<DataPoint>();
            foreach (DataPoint dp in sut.Process(data))
                actual.Add(dp);

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Data_given_as_IList___OK()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = RawDataForTrend().ToList().AsReadOnly();
            var expected = ExpectedForTrend().ToList();

            var actual = new List<DataPoint>();
            foreach (DataPoint dp in sut.Process(data))
                actual.Add(dp);

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Data_IEnumerable_with_maxDeltaX___OK()
        {
            var sut      = new DeadBandCompression(0.1, 4d);
            var data     = RawDataForMaxDelta();
            var expected = ExpectedForMaxDelta().ToList();

            var actual = new List<DataPoint>();
            foreach (DataPoint dp in sut.Process(data))
                actual.Add(dp);

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Data_List_with_maxDeltaX___OK()
        {
            var sut      = new DeadBandCompression(0.1, 4d);
            var data     = RawDataForMaxDelta().ToList();
            var expected = ExpectedForMaxDelta().ToList();

            var actual = new List<DataPoint>();
            foreach (DataPoint dp in sut.Process(data))
                actual.Add(dp);

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Data_IList_with_maxDeltaX___OK()
        {
            var sut      = new DeadBandCompression(0.1, 4d);
            var data     = RawDataForMaxDelta().ToList().AsReadOnly();
            var expected = ExpectedForMaxDelta().ToList();

            var actual = new List<DataPoint>();
            foreach (DataPoint dp in sut.Process(data))
                actual.Add(dp);

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void One_DataPoint_IEnumerable___OK()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = KnownSequence().Take(1);
            var expected = KnownSequence().Take(1).ToList();

            var actual = new List<DataPoint>();
            foreach (DataPoint dp in sut.Process(data))
                actual.Add(dp);

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void One_DataPoint_List___OK()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = KnownSequence().Take(1).ToList();
            var expected = KnownSequence().Take(1).ToList();

            var actual = new List<DataPoint>();
            foreach (DataPoint dp in sut.Process(data))
                actual.Add(dp);

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void One_DataPoint_IList___OK()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = KnownSequence().Take(1).ToList().AsReadOnly();
            var expected = KnownSequence().Take(1).ToList();

            var actual = new List<DataPoint>();
            foreach (DataPoint dp in sut.Process(data))
                actual.Add(dp);

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Two_DataPoint_IEnumerable___OK()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = KnownSequence().Take(2);
            var expected = KnownSequence().Take(2).ToList();

            var actual = new List<DataPoint>();
            foreach (DataPoint dp in sut.Process(data))
                actual.Add(dp);

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Two_DataPoint_List___OK()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = KnownSequence().Take(2).ToList();
            var expected = KnownSequence().Take(2).ToList();

            var actual = new List<DataPoint>();
            foreach (DataPoint dp in sut.Process(data))
                actual.Add(dp);

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Two_DataPoint_IList___OK()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = KnownSequence().Take(2).ToList().AsReadOnly();
            var expected = KnownSequence().Take(2).ToList();

            var actual = new List<DataPoint>();
            foreach (DataPoint dp in sut.Process(data))
                actual.Add(dp);

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
