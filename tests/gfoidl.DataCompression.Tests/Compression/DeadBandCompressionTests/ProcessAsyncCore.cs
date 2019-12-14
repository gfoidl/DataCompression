using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace gfoidl.DataCompression.Tests.Compression.DeadBandCompressionTests
{
    [TestFixture]
    public class ProcessAsyncCore
    {
        private static readonly DataPointSerializer s_ser = new DataPointSerializer();
        //---------------------------------------------------------------------
        [Test]
        public async Task Data_given_as_IAsyncEnumerable___OK()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = RawDataForTrendAsync();
            var expected = ExpectedForTrend().ToList();

            var actual = new List<DataPoint>();
            await foreach (DataPoint dp in sut.ProcessAsync(data))
                actual.Add(dp);

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public async Task Cancellation_after_two_items___OK()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = RawDataForTrendAsync();
            var expected = ExpectedForTrend().Take(2).ToList();
            var cts      = new CancellationTokenSource();

            var actual = new List<DataPoint>();
            int idx    = 0;
            try
            {
                await foreach (DataPoint dp in sut.ProcessAsync(data, cts.Token))
                {
                    actual.Add(dp);
                    idx++;

                    if (idx == 2) cts.Cancel();
                }
            }
            catch (OperationCanceledException) { }

            CollectionAssert.AreEqual(actual, expected);
        }
        //---------------------------------------------------------------------
        [Test]
        public async Task Data_IAsyncEnumerable_with_maxDeltaX___OK()
        {
            var sut      = new DeadBandCompression(0.1, 4d);
            var data     = RawDataForMaxDeltaAsync();
            var expected = ExpectedForMaxDelta().ToList();

            var actual = new List<DataPoint>();
            await foreach (DataPoint dp in sut.ProcessAsync(data))
                actual.Add(dp);

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        private static IEnumerable<DataPoint> RawDataForTrend()     => s_ser.Read("../../../../../doc/data/dead-band/trend_raw.csv");
        private static IEnumerable<DataPoint> ExpectedForTrend()    => s_ser.Read("../../../../../doc/data/dead-band/trend_compressed.csv");
        private static IEnumerable<DataPoint> RawDataForMaxDelta()  => s_ser.Read("../../../../../doc/data/dead-band/maxDelta_raw.csv");
        private static IEnumerable<DataPoint> ExpectedForMaxDelta() => s_ser.Read("../../../../../doc/data/dead-band/maxDelta_compressed.csv");
        //---------------------------------------------------------------------
        private static async IAsyncEnumerable<DataPoint> RawDataForTrendAsync([EnumeratorCancellation] CancellationToken ct = default)
        {
            foreach (DataPoint dp in RawDataForTrend())
            {
                ct.ThrowIfCancellationRequested();
                await Task.Yield();
                yield return dp;
            }
        }
        //---------------------------------------------------------------------
        private static async IAsyncEnumerable<DataPoint> RawDataForMaxDeltaAsync([EnumeratorCancellation] CancellationToken ct = default)
        {
            foreach (DataPoint dp in RawDataForMaxDelta())
            {
                ct.ThrowIfCancellationRequested();
                await Task.Yield();
                yield return dp;
            }
        }
    }
}
