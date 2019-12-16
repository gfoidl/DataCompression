using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace gfoidl.DataCompression.Tests.Compression.DeadBandCompressionTests
{
    public class ToArrayAsync : Base
    {
        [Test]
        public async Task Empty_IAsyncEnumerable___empty_result()
        {
            var sut  = new DeadBandCompression(0.1);
            var data = EmptyAsync();

            var actual = sut.ProcessAsync(data);

            Assert.AreEqual(0, (await actual.ToListAsync()).Count);
        }
        //---------------------------------------------------------------------
        [Test]
        public async Task Data_given_as_IAsyncEnumerable___OK()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = RawDataForTrendAsync();
            var expected = ExpectedForTrend().ToList();

            var actual = await sut.ProcessAsync(data).ToArrayAsync();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public async Task Data_IAsyncEnumerable_with_maxDeltaX___OK()
        {
            var sut      = new DeadBandCompression(0.1, 4d);
            var data     = RawDataForMaxDeltaAsync();
            var expected = ExpectedForMaxDelta().ToList();

            var actual = await sut.ProcessAsync(data).ToArrayAsync();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public async Task IEnumerable_iterated_and_ToArray___OK()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = RawDataForTrendAsync();
            var expected = ExpectedForTrend().ToList();

            DataPointAsyncIterator dataPointIterator = sut.ProcessAsync(data);

            DataPointAsyncIterator enumerator = dataPointIterator.GetAsyncEnumerator();
            await enumerator.MoveNextAsync();
            await enumerator.MoveNextAsync();
            var actual                        = await dataPointIterator.ToArrayAsync();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public async Task Cancellation___OK()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = RawDataForTrendAsync();
            var expected = ExpectedForTrend().Take(2).ToList();

            DataPointAsyncIterator dataPointIterator = sut.ProcessAsync(data);
            var cts                                  = new CancellationTokenSource();

            DataPointAsyncIterator enumerator = dataPointIterator.GetAsyncEnumerator(cts.Token);
            var actual                        = new List<DataPoint>();
            await enumerator.MoveNextAsync();
            actual.Add(enumerator.Current);
            await enumerator.MoveNextAsync();
            actual.Add(enumerator.Current);
            cts.Cancel();

            DataPoint[] res = null;
            Assert.ThrowsAsync<OperationCanceledException>(async () => res = await dataPointIterator.ToArrayAsync());

            CollectionAssert.AreEqual(expected, actual);
            Assert.IsNull(res);
        }
    }
}
