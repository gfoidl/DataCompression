// (c) gfoidl, all rights reserved

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace gfoidl.DataCompression.Tests.Compression.SwingingDoorCompressionTests
{
    public class ProcessAsyncCore : Base
    {
        [Test, TestCaseSource(typeof(Base), nameof(Base.IAsyncEnumerableTestCases))]
        public async Task Data_given_as_IAsyncEnumerable___OK(double compressionDeviation, IAsyncEnumerable<DataPoint> rawData, IEnumerable<DataPoint> expectedData)
        {
            var sut      = new SwingingDoorCompression(compressionDeviation);
            var data     = rawData;
            var expected = expectedData.ToList();

            var actual = new List<DataPoint>();
            await foreach (DataPoint dp in sut.ProcessAsync(data))
                actual.Add(dp);

            Print(expected, "expected");
            Print(actual  , "actual");
            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Cancellation_after_two_items___OK()
        {
            var sut      = new SwingingDoorCompression(1d);
            var data     = RawDataAsync(RawDataForTrend());
            var expected = ExpectedForTrend().Take(2).ToList();
            var cts      = new CancellationTokenSource();

            var actual = new List<DataPoint>();
            int idx    = 0;

            Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                await foreach (DataPoint dp in sut.ProcessAsync(data).WithCancellation(cts.Token))
                {
                    actual.Add(dp);
                    idx++;

                    if (idx == 2) cts.Cancel();
                }
            });

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public async Task Data_IAsyncEnumerable_with_maxDeltaX___OK()
        {
            var sut      = new SwingingDoorCompression(1d, 6d);
            var data     = RawDataAsync(RawDataForMaxDelta());
            var expected = ExpectedForMaxDelta().ToList();

            var actual = new List<DataPoint>();
            await foreach (DataPoint dp in sut.ProcessAsync(data))
                actual.Add(dp);

            Print(expected, "expected");
            Print(actual  , "actual");
            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public async Task Data_IAsyncEnumerable_with_minDeltaX___OK()
        {
            var sut      = new SwingingDoorCompression(1d, minDeltaX: 1d);
            var data     = RawDataAsync(RawMinDeltaX());
            var expected = ExpectedMinDeltaX().ToList();

            var actual = new List<DataPoint>();
            await foreach (DataPoint dp in sut.ProcessAsync(data))
                actual.Add(dp);

            Print(expected, "expected");
            Print(actual  , "actual");
            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public async Task Data_IAsyncEnumerable_with_minDeltaX_ToArray___OK()
        {
            int currentRawMinDeltaXCount = RawMinDeltaXCounter;

            var sut      = new SwingingDoorCompression(1d, minDeltaX: 1d);
            var data     = RawDataAsync(RawMinDeltaX());
            var expected = ExpectedMinDeltaX().ToList();

            var actual = await sut.ProcessAsync(data).ToArrayAsync();

            CollectionAssert.AreEqual(expected, actual);
            Assert.AreEqual(currentRawMinDeltaXCount + 1, RawMinDeltaXCounter);
        }
    }
}
