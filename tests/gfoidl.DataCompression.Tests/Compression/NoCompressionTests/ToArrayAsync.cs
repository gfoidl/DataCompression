using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace gfoidl.DataCompression.Tests.Compression.NoCompressionTests
{
    public class ToArrayAsync : Base
    {
        [Test]
        public async Task Data_given_as_IAsyncEnumerable___OK()
        {
            var sut      = new NoCompression();
            var data     = RawDataForTrendAsync();
            var expected = RawDataForTrend().ToList();

            var actual = await sut.ProcessAsync(data).ToArrayAsync();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public async Task Data_IAsyncEnumerable_with_maxDeltaX___OK()
        {
            var sut      = new NoCompression();
            var data     = RawDataForMaxDeltaAsync();
            var expected = RawDataForMaxDelta().ToList();

            var actual = await sut.ProcessAsync(data).ToArrayAsync();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public async Task IAsyncEnumerable_iterated_and_ToArray___OK()
        {
            var sut      = new NoCompression();
            var data     = RawDataForTrendAsync();
            var expected = RawDataForTrend().ToList();

            DataPointIterator dataPointIterator = sut.ProcessAsync(data);
            var enumerator                      = dataPointIterator.GetAsyncEnumerator();

            await enumerator.MoveNextAsync();
            await enumerator.MoveNextAsync();
            var actual = await dataPointIterator.ToArrayAsync();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public async Task Cancellation___OK()
        {
            var sut      = new NoCompression();
            var data     = RawDataForTrendAsync();
            var expected = RawDataForTrend().Take(2).ToList();

            DataPointIterator dataPointIterator = sut.ProcessAsync(data);
            var cts                             = new CancellationTokenSource();
            var enumerator                      = dataPointIterator.GetAsyncEnumerator(cts.Token);

            var actual = new List<DataPoint>();
            await enumerator.MoveNextAsync();
            actual.Add(enumerator.Current);
            await enumerator.MoveNextAsync();
            actual.Add(enumerator.Current);
            cts.Cancel();

            DataPoint[] res = null;
            Assert.ThrowsAsync<OperationCanceledException>(async () => res = await dataPointIterator.ToArrayAsync(cts.Token));

            CollectionAssert.AreEqual(expected, actual);
            Assert.IsNull(res);
        }
    }
}
