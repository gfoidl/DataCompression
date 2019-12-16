using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using gfoidl.DataCompression.Wrappers;
using NUnit.Framework;

namespace gfoidl.DataCompression.Tests.Compression.SwingingDoorCompressionTests
{
    public class ToArrayAsync : Base
    {
        [Test]
        public async Task Empty_IAsyncEnumerable___empty_result()
        {
            var sut  = new SwingingDoorCompression(1d);
            var data = EmptyAsync();

            var actual = sut.ProcessAsync(data);

            Assert.AreEqual(0, (await actual.ToListAsync()).Count);
        }
        //---------------------------------------------------------------------
        [Test]
        public async Task Data_given_as_IAsyncEnumerable___OK()
        {
            var sut      = new SwingingDoorCompression(1d);
            var data     = RawDataForTrendAsync();
            var expected = ExpectedForTrend().ToList();

            var actual = await sut.ProcessAsync(data).ToArrayAsync();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public async Task Data_IAsyncEnumerable_with_maxDeltaX___OK()
        {
            var sut      = new SwingingDoorCompression(1d, 6d);
            var data     = RawDataForMaxDeltaAsync();
            var expected = ExpectedForMaxDelta().ToList();

            var actual = await sut.ProcessAsync(data).ToArrayAsync();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public async Task IEnumerable_iterated_and_ToArray___OK()
        {
            var sut      = new SwingingDoorCompression(1d);
            var data     = RawDataForTrendAsync();
            var expected = ExpectedForTrend().ToList();

            DataPointIterator dataPointIterator = sut.ProcessAsync(data);
            DataPointIterator enumerator        = dataPointIterator.GetAsyncEnumerator();

            await enumerator.MoveNextAsync();
            await enumerator.MoveNextAsync();
            var actual                        = await dataPointIterator.ToArrayAsync();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public async Task Cancellation___OK()
        {
            var sut      = new SwingingDoorCompression(1d);
            var data     = RawDataForTrendAsync();
            var expected = ExpectedForTrend().Take(2).ToList();

            DataPointIterator dataPointIterator = sut.ProcessAsync(data);
            var cts                             = new CancellationTokenSource();
            DataPointIterator enumerator        = dataPointIterator.GetAsyncEnumerator(cts.Token);

            var actual = new List<DataPoint>();
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
