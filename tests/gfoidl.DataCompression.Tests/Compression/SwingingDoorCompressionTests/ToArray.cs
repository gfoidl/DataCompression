// (c) gfoidl, all rights reserved

using System;
using System.Collections.Generic;
using System.Linq;
using gfoidl.DataCompression.Wrappers;
using NUnit.Framework;

namespace gfoidl.DataCompression.Tests.Compression.SwingingDoorCompressionTests
{
    public class ToArray : Base
    {
        [Test]
        public void Empty_IEnumerable___empty_result()
        {
            var sut  = new SwingingDoorCompression(1d);
            var data = Empty();

            var actual = sut.Process(data);

            Assert.AreSame(Array.Empty<DataPoint>(), actual.ToArray());
            Assert.AreEqual(0, actual.ToList().Count);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Empty_Array___empty_result()
        {
            var sut  = new SwingingDoorCompression(1d);
            var data = new DataPoint[0];

            var actual = sut.Process(data);

            Assert.AreSame(Array.Empty<DataPoint>(), actual.ToArray());
            Assert.AreEqual(0, actual.ToList().Count);
        }
        //---------------------------------------------------------------------
        [Test, TestCaseSource(typeof(Base), nameof(Base.IEnumerableTestCases))]
        public void Data_given_as_IEnumerable___OK(double compressionDeviation, IEnumerable<DataPoint> rawData, IEnumerable<DataPoint> expectedData)
        {
            var sut      = new SwingingDoorCompression(compressionDeviation);
            var data     = rawData;
            var expected = expectedData.ToList();

            var actual = sut.Process(data).ToArray();

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

            var actual = sut.Process(data).ToArray();

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

            var actual = sut.Process(data).ToArray();

            Print(expected, "expected");
            Print(actual  , "actual");
            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test, TestCaseSource(typeof(Base), nameof(Base.IEnumerableTestCases))]
        public void Data_given_as_Array___OK(double compressionDeviation, IEnumerable<DataPoint> rawData, IEnumerable<DataPoint> expectedData)
        {
            var sut      = new SwingingDoorCompression(compressionDeviation);
            var data     = rawData     .ToArray();
            var expected = expectedData.ToList();

            var actual = sut.Process(data).ToArray();

            Print(expected, "expected");
            Print(actual  , "actual");
            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test, TestCaseSource(typeof(Base), nameof(Base.IEnumerableTestCases))]
        public void Data_given_as_ListWrapper___OK(double compressionDeviation, IEnumerable<DataPoint> rawData, IEnumerable<DataPoint> expectedData)
        {
            var sut      = new SwingingDoorCompression(compressionDeviation);
            var data     = new ListWrapper<DataPoint>(rawData.ToList());
            var expected = expectedData.ToList();

            var actual = sut.Process(data).ToArray();

            Print(expected, "expected");
            Print(actual  , "actual");
            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test, TestCaseSource(typeof(Base), nameof(Base.IEnumerableTestCases))]
        public void Data_given_as_ArrayWrapper___OK(double compressionDeviation, IEnumerable<DataPoint> rawData, IEnumerable<DataPoint> expectedData)
        {
            var sut      = new SwingingDoorCompression(compressionDeviation);
            var data     = new ArrayWrapper<DataPoint>(rawData.ToArray());
            var expected = expectedData.ToList();

            var actual = sut.Process(data).ToArray();

            Print(expected, "expected");
            Print(actual  , "actual");
            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Data_IEnumerable_with_maxDeltaX___OK()
        {
            var sut      = new SwingingDoorCompression(1d, 6d);
            var data     = RawDataForMaxDelta();
            var expected = ExpectedForMaxDelta().ToList();

            var actual = sut.Process(data).ToArray();

            Print(expected, "expected");
            Print(actual  , "actual");
            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Data_List_with_maxDeltaX___OK()
        {
            var sut      = new SwingingDoorCompression(1d, 6d);
            var data     = RawDataForMaxDelta() .ToList();
            var expected = ExpectedForMaxDelta().ToList();

            var actual = sut.Process(data).ToArray();

            Print(expected, "expected");
            Print(actual  , "actual");
            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Data_IList_with_maxDeltaX___OK()
        {
            var sut      = new SwingingDoorCompression(1d, 6d);
            var data     = RawDataForMaxDelta() .ToList().AsReadOnly();
            var expected = ExpectedForMaxDelta().ToList();

            var actual = sut.Process(data).ToArray();

            Print(expected, "expected");
            Print(actual  , "actual");
            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void One_DataPoint_IEnumerable___OK()
        {
            var sut      = new SwingingDoorCompression(1d);
            var data     = RawDataForMaxDelta().Take(1);
            var expected = RawDataForMaxDelta().Take(1).ToList();

            var actual = sut.Process(data).ToArray();

            Print(expected, "expected");
            Print(actual  , "actual");
            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void One_DataPoint_List___OK()
        {
            var sut      = new SwingingDoorCompression(1d);
            var data     = RawDataForMaxDelta().Take(1).ToList();
            var expected = RawDataForMaxDelta().Take(1).ToList();

            var actual = sut.Process(data).ToArray();

            Print(expected, "expected");
            Print(actual  , "actual");
            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void One_DataPoint_IList___OK()
        {
            var sut      = new SwingingDoorCompression(1d);
            var data     = RawDataForMaxDelta().Take(1).ToList().AsReadOnly();
            var expected = RawDataForMaxDelta().Take(1).ToList();

            var actual = sut.Process(data).ToArray();

            Print(expected, "expected");
            Print(actual  , "actual");
            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Two_DataPoints_IEnumerable___OK()
        {
            var sut      = new SwingingDoorCompression(1d);
            var data     = RawDataForMaxDelta().Take(2);
            var expected = RawDataForMaxDelta().Take(2).ToList();

            var actual = sut.Process(data).ToArray();

            Print(expected, "expected");
            Print(actual  , "actual");
            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Two_DataPoints_List___OK()
        {
            var sut      = new SwingingDoorCompression(1d);
            var data     = RawDataForMaxDelta().Take(2).ToList();
            var expected = RawDataForMaxDelta().Take(2).ToList();

            var actual = sut.Process(data).ToArray();

            Print(expected, "expected");
            Print(actual  , "actual");
            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Two_DataPoints_IList___OK()
        {
            var sut      = new SwingingDoorCompression(1d);
            var data     = RawDataForMaxDelta().Take(2).ToList().AsReadOnly();
            var expected = RawDataForMaxDelta().Take(2).ToList();

            var actual = sut.Process(data).ToArray();

            Print(expected, "expected");
            Print(actual  , "actual");
            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void MinDeltaX_IEnumerable___OK()
        {
            var sut      = new SwingingDoorCompression(1d, minDeltaX: 1d);
            var data     = RawMinDeltaX();
            var expected = ExpectedMinDeltaX().ToList();

            var actual = sut.Process(data).ToArray();

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

            var actual = sut.Process(data).ToArray();

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

            var actual = sut.Process(data).ToArray();

            Print(expected, "expected");
            Print(actual  , "actual");
            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test, TestCaseSource(typeof(Base), nameof(Base.IEnumerableTestCases))]
        public void IEnumerable_iterated_and_ToArray___OK(double compressionDeviation, IEnumerable<DataPoint> rawData, IEnumerable<DataPoint> expectedData)
        {
            var sut      = new SwingingDoorCompression(compressionDeviation);
            var data     = rawData;
            var expected = expectedData.ToList();

            DataPointIterator dataPointIterator = sut.Process(data);

            DataPointIterator enumerator = dataPointIterator.GetEnumerator();
            enumerator.MoveNext();
            enumerator.MoveNext();
            var actual = dataPointIterator.ToArray();

            Print(expected, "expected");
            Print(actual  , "actual");
            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test, TestCaseSource(typeof(Base), nameof(Base.IEnumerableTestCases))]
        public void List_iterated_and_ToArray___OK(double compressionDeviation, IEnumerable<DataPoint> rawData, IEnumerable<DataPoint> expectedData)
        {
            var sut      = new SwingingDoorCompression(compressionDeviation);
            var data     = rawData     .ToList();
            var expected = expectedData.ToList();

            DataPointIterator dataPointIterator = sut.Process(data);

            DataPointIterator enumerator = dataPointIterator.GetEnumerator();
            enumerator.MoveNext();
            enumerator.MoveNext();
            var actual = dataPointIterator.ToArray();

            Print(expected, "expected");
            Print(actual  , "actual");
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
