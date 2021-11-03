// (c) gfoidl, all rights reserved

using System.Collections.Generic;
using NUnit.Framework;

#if NETCOREAPP
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
#endif

namespace gfoidl.DataCompression.Tests.Compression.SwingingDoorCompressionTests
{
    public abstract class Base : Compression.Base
    {
        private static readonly DataPointSerializer s_ser = new DataPointSerializer();
        //---------------------------------------------------------------------
        protected static int RawMinDeltaXCounter { get; private set; }
        //---------------------------------------------------------------------
        protected static IEnumerable<TestCaseData> IEnumerableTestCases()
        {
            // https://docs.nunit.org/articles/nunit/running-tests/Template-Based-Test-Naming.html
            yield return new TestCaseData(1.0, RawDataForTrend() , ExpectedForTrend()) .SetName("{m} trend");
            yield return new TestCaseData(0.1, RawDataForTrend1(), ExpectedForTrend1()).SetName("{m} trend1");
        }
        //---------------------------------------------------------------------
        protected static IEnumerable<DataPoint> RawDataForTrend()     => s_ser.Read("../../../../../data/swinging-door/trend_raw.csv");
        protected static IEnumerable<DataPoint> ExpectedForTrend()    => s_ser.Read("../../../../../data/swinging-door/trend_compressed.csv");
        protected static IEnumerable<DataPoint> RawDataForTrend1()    => s_ser.Read("../../../../../data/swinging-door/trend1_raw.csv");
        protected static IEnumerable<DataPoint> ExpectedForTrend1()   => s_ser.Read("../../../../../data/swinging-door/trend1_compressed.csv");
        protected static IEnumerable<DataPoint> RawDataForMaxDelta()  => s_ser.Read("../../../../../data/swinging-door/maxDelta_raw.csv");
        protected static IEnumerable<DataPoint> ExpectedForMaxDelta() => s_ser.Read("../../../../../data/swinging-door/maxDelta_compressed.csv");
        //---------------------------------------------------------------------
        protected static IEnumerable<DataPoint> RawMinDeltaX()
        {
            RawMinDeltaXCounter++;

            yield return (0d, 2d);
            yield return (1d, 2d);
            yield return (2d, 2d);
            yield return (3d, 2d);
            yield return (4d, 2d);
            yield return (5d, 10d);
            yield return (6d, 10d);
            yield return (7d, 10d);
            yield return (8d, 10d);
            yield return (9d, 10d);
        }
        //---------------------------------------------------------------------
        protected static IEnumerable<DataPoint> ExpectedMinDeltaX()
        {
            yield return (0d, 2d);
            yield return (4d, 2d);
            yield return (6d, 10d);
            yield return (9d, 10d);
        }
        //---------------------------------------------------------------------
#if NETCOREAPP
        protected static IEnumerable<TestCaseData> IAsyncEnumerableTestCases()
        {
            // https://docs.nunit.org/articles/nunit/running-tests/Template-Based-Test-Naming.html
            yield return new TestCaseData(1.0, RawDataForTrendAsync() , ExpectedForTrend()) .SetName("{m} trend");
            yield return new TestCaseData(0.1, RawDataForTrend1Async(), ExpectedForTrend1()).SetName("{m} trend1");
        }
        //---------------------------------------------------------------------
        protected static async IAsyncEnumerable<DataPoint> RawDataForTrendAsync([EnumeratorCancellation] CancellationToken ct = default)
        {
            foreach (DataPoint dp in RawDataForTrend())
            {
                ct.ThrowIfCancellationRequested();
                await Task.Yield();
                yield return dp;
            }
        }
        //---------------------------------------------------------------------
        protected static async IAsyncEnumerable<DataPoint> RawDataForTrend1Async([EnumeratorCancellation] CancellationToken ct = default)
        {
            foreach (DataPoint dp in RawDataForTrend1())
            {
                ct.ThrowIfCancellationRequested();
                await Task.Yield();
                yield return dp;
            }
        }
        //---------------------------------------------------------------------
        protected static async IAsyncEnumerable<DataPoint> RawDataForMaxDeltaAsync([EnumeratorCancellation] CancellationToken ct = default)
        {
            foreach (DataPoint dp in RawDataForMaxDelta())
            {
                ct.ThrowIfCancellationRequested();
                await Task.Yield();
                yield return dp;
            }
        }
        //---------------------------------------------------------------------
        protected static async IAsyncEnumerable<DataPoint> RawMinDeltaXAsync([EnumeratorCancellation] CancellationToken ct = default)
        {
            foreach (DataPoint dp in RawMinDeltaX())
            {
                ct.ThrowIfCancellationRequested();
                await Task.Yield();
                yield return dp;
            }
        }
#endif
    }
}
