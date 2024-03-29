// (c) gfoidl, all rights reserved

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace gfoidl.DataCompression.Tests.Compression.SwingingDoorCompressionTests
{
    public class ToListAsync : Base
    {
        [Test, TestCaseSource(typeof(Base), nameof(Base.IAsyncEnumerableTestCases))]
        public async Task Data_given_as_IAsyncEnumerable___OK(double compressionDeviation, IAsyncEnumerable<DataPoint> rawData, IEnumerable<DataPoint> expectedData)
        {
            var sut      = new SwingingDoorCompression(compressionDeviation);
            var data     = rawData;
            var expected = expectedData.ToList();

            var actual = await sut.ProcessAsync(data).ToListAsync();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public async Task Data_IAsyncEnumerable_with_maxDeltaX___OK()
        {
            var sut      = new SwingingDoorCompression(1d, 6d);
            var data     = RawDataAsync(RawDataForMaxDelta());
            var expected = ExpectedForMaxDelta().ToList();

            var actual = await sut.ProcessAsync(data).ToListAsync();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test, TestCaseSource(typeof(Base), nameof(Base.IAsyncEnumerableTestCases))]
        public async Task IEnumerable_iterated_and_ToList___OK(double compressionDeviation, IAsyncEnumerable<DataPoint> rawData, IEnumerable<DataPoint> expectedData)
        {
            var sut      = new SwingingDoorCompression(compressionDeviation);
            var data     = rawData;
            var expected = expectedData.ToList();

            DataPointIterator dataPointIterator = sut.ProcessAsync(data);
            var enumerator                      = dataPointIterator.GetAsyncEnumerator();

            await enumerator.MoveNextAsync();
            await enumerator.MoveNextAsync();
            var actual = await dataPointIterator.ToListAsync();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public async Task Cancellation___OK()
        {
            var sut      = new SwingingDoorCompression(1d);
            var data     = RawDataAsync(RawDataForTrend());
            var expected = ExpectedForTrend().Take(2).ToList();

            DataPointIterator dataPointIterator = sut.ProcessAsync(data);
            var cts                             = new CancellationTokenSource();
            var enumerator                      = dataPointIterator.GetAsyncEnumerator(cts.Token);

            var actual = new List<DataPoint>();
            await enumerator.MoveNextAsync();
            actual.Add(enumerator.Current);
            await enumerator.MoveNextAsync();
            actual.Add(enumerator.Current);
            cts.Cancel();

            List<DataPoint> res = null;
            Assert.ThrowsAsync<OperationCanceledException>(async () => res = await dataPointIterator.ToListAsync(cts.Token));

            CollectionAssert.AreEqual(expected, actual);
            Assert.IsNull(res);
        }
    }
}
