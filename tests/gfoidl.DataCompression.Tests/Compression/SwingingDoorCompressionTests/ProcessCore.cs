// (c) gfoidl, all rights reserved

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace gfoidl.DataCompression.Tests.Compression.SwingingDoorCompressionTests
{
    public class ProcessCore : Base
    {
        [Test, TestCaseSource(typeof(Base), nameof(Base.IEnumerableTestCases))]
        public void Data_given_as_IEnumerable___OK(double compressionDeviation, IEnumerable<DataPoint> rawData, IEnumerable<DataPoint> expectedData)
        {
            var sut      = new SwingingDoorCompression(compressionDeviation);
            var data     = rawData;
            var expected = expectedData.ToList();

            var actual = new List<DataPoint>();
            foreach (DataPoint dp in sut.Process(data))
                actual.Add(dp);

            Print(expected, "expected");
            Print(actual  , "actual");
            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test, TestCaseSource(typeof(Base), nameof(Base.IEnumerableTestCases))]
        public void Data_given_as_List___OK(double compressionDeviation, IEnumerable<DataPoint> rawData, IEnumerable<DataPoint> expectedData)
        {
            var sut      = new SwingingDoorCompression(compressionDeviation);
            var data     = rawData     .ToList();
            var expected = expectedData.ToList();

            var actual = new List<DataPoint>();
            foreach (DataPoint dp in sut.Process(data))
                actual.Add(dp);

            Print(expected, "expected");
            Print(actual  , "actual");
            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test, TestCaseSource(typeof(Base), nameof(Base.IEnumerableTestCases))]
        public void Data_given_as_IList___OK(double compressionDeviation, IEnumerable<DataPoint> rawData, IEnumerable<DataPoint> expectedData)
        {
            var sut      = new SwingingDoorCompression(compressionDeviation);
            var data     = rawData     .ToList().AsReadOnly();
            var expected = expectedData.ToList();

            var actual = new List<DataPoint>();
            foreach (DataPoint dp in sut.Process(data))
                actual.Add(dp);

            Print(expected, "expected");
            Print(actual  , "actual");
            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Data_IEnumerable_with_maxDeltaX___OK()
        {
            var sut      = new SwingingDoorCompression(1d, maxDeltaX: 6d);
            var data     = RawDataForMaxDelta();
            var expected = ExpectedForMaxDelta().ToList();

            var actual = new List<DataPoint>();
            foreach (DataPoint dp in sut.Process(data))
                actual.Add(dp);

            Print(expected, "expected");
            Print(actual  , "actual");
            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Data_List_with_maxDeltaX___OK()
        {
            var sut      = new SwingingDoorCompression(1d, maxDeltaX: 6d);
            var data     = RawDataForMaxDelta() .ToList();
            var expected = ExpectedForMaxDelta().ToList();

            var actual = new List<DataPoint>();
            foreach (DataPoint dp in sut.Process(data))
                actual.Add(dp);

            Print(expected, "expected");
            Print(actual  , "actual");
            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Data_IList_with_maxDeltaX___OK()
        {
            var sut      = new SwingingDoorCompression(1d, maxDeltaX: 6d);
            var data     = RawDataForMaxDelta() .ToList().AsReadOnly();
            var expected = ExpectedForMaxDelta().ToList();

            var actual = new List<DataPoint>();
            foreach (DataPoint dp in sut.Process(data))
                actual.Add(dp);

            Print(expected, "expected");
            Print(actual  , "actual");
            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Empty_IEnumerable___OK()
        {
            var sut  = new SwingingDoorCompression(1d);
            var data = KnownSequence().Take(0);

            var actual = new List<DataPoint>();
            foreach (DataPoint dp in sut.Process(data))
                actual.Add(dp);

            Assert.AreEqual(0, actual.Count);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Empty_List___OK()
        {
            var sut  = new SwingingDoorCompression(1d);
            var data = KnownSequence().Take(0).ToList();

            var actual = new List<DataPoint>();
            foreach (DataPoint dp in sut.Process(data))
                actual.Add(dp);

            Assert.AreEqual(0, actual.Count);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Empty_IList___OK()
        {
            var sut  = new SwingingDoorCompression(1d);
            var data = KnownSequence().Take(0).ToList().AsReadOnly();

            var actual = new List<DataPoint>();
            foreach (DataPoint dp in sut.Process(data))
                actual.Add(dp);

            Assert.AreEqual(0, actual.Count);
        }
        //---------------------------------------------------------------------
        [Test]
        public void One_DataPoint_IEnumerable___OK()
        {
            var sut      = new SwingingDoorCompression(1d);
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
            var sut      = new SwingingDoorCompression(1d);
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
            var sut      = new SwingingDoorCompression(1d);
            var data     = KnownSequence().Take(1).ToList().AsReadOnly();
            var expected = KnownSequence().Take(1).ToList();

            var actual = new List<DataPoint>();
            foreach (DataPoint dp in sut.Process(data))
                actual.Add(dp);

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test, TestCaseSource(typeof(Base), nameof(Base.TwoDataPointsTestCases))]
        public void Two_DataPoint_IEnumerable___OK(IEnumerable<DataPoint> rawData, List<DataPoint> expectedData)
        {
            var sut      = new SwingingDoorCompression(1d);
            var data     = rawData;
            var expected = expectedData;

            var actual = new List<DataPoint>();
            foreach (DataPoint dp in sut.Process(data))
                actual.Add(dp);

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test, TestCaseSource(typeof(Base), nameof(Base.TwoDataPointsTestCases))]
        public void Two_DataPoint_List___OK(IEnumerable<DataPoint> rawData, List<DataPoint> expectedData)
        {
            var sut      = new SwingingDoorCompression(1d);
            var data     = rawData.ToList();
            var expected = expectedData;

            var actual = new List<DataPoint>();
            foreach (DataPoint dp in sut.Process(data))
                actual.Add(dp);

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test, TestCaseSource(typeof(Base), nameof(Base.TwoDataPointsTestCases))]
        public void Two_DataPoint_IList___OK(IEnumerable<DataPoint> rawData, List<DataPoint> expectedData)
        {
            var sut      = new SwingingDoorCompression(1d);
            var data     = rawData.ToList().AsReadOnly();
            var expected = expectedData;

            var actual = new List<DataPoint>();
            foreach (DataPoint dp in sut.Process(data))
                actual.Add(dp);

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void MinDeltaX_IEnumerable___OK()
        {
            var sut      = new SwingingDoorCompression(1d, minDeltaX: 1d);
            var data     = RawMinDeltaX();
            var expected = ExpectedMinDeltaX().ToList();

            var actual = new List<DataPoint>();
            foreach (DataPoint dp in sut.Process(data))
                actual.Add(dp);

            Print(expected, "expected");
            Print(actual  , "actual");
            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void MinDeltaX_List___OK()
        {
            var sut      = new SwingingDoorCompression(1d, minDeltaX: 1d);
            var data     = RawMinDeltaX()     .ToList();
            var expected = ExpectedMinDeltaX().ToList();

            var actual = new List<DataPoint>();
            foreach (DataPoint dp in sut.Process(data))
                actual.Add(dp);

            Print(expected, "expected");
            Print(actual  , "actual");
            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void MinDeltaX_IList___OK()
        {
            var sut      = new SwingingDoorCompression(1d, minDeltaX: 1d);
            var data     = RawMinDeltaX()     .ToList().AsReadOnly();
            var expected = ExpectedMinDeltaX().ToList();

            var actual = new List<DataPoint>();
            foreach (DataPoint dp in sut.Process(data))
                actual.Add(dp);

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
