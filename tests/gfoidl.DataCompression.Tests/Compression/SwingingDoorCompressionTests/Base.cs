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
        protected static int RawMinDeltaXCounter { get; private set; }
        //---------------------------------------------------------------------
        protected static IEnumerable<TestCaseData> IEnumerableTestCases()
        {
            // https://docs.nunit.org/articles/nunit/running-tests/Template-Based-Test-Naming.html
            yield return new TestCaseData(1.0, RawDataForTrend() , ExpectedForTrend()) .SetName("{m} trend");
            yield return new TestCaseData(0.1, RawDataForTrend1(), ExpectedForTrend1()).SetName("{m} trend1");
            yield return new TestCaseData(0.1, RawDataForTrend2(), ExpectedForTrend2()).SetName("{m} trend2");
            yield return new TestCaseData(2.0, RawDataForTrend3(), ExpectedForTrend3()).SetName("{m} trend3");
            yield return new TestCaseData(2.0, RawDataForTrend3Mini(), ExpectedForTrend3Mini()).SetName("{m} trend3_mini");
        }
        //---------------------------------------------------------------------
        protected static IEnumerable<DataPoint> RawDataForTrend()      => s_ser.Read("data/swinging-door/trend_raw.csv");
        protected static IEnumerable<DataPoint> RawDataForTrend1()     => s_ser.Read("data/swinging-door/trend1_raw.csv");
        protected static IEnumerable<DataPoint> RawDataForTrend2()     => s_ser.Read("data/swinging-door/trend2_raw.csv");
        protected static IEnumerable<DataPoint> RawDataForTrend3()     => s_ser.Read("data/swinging-door/trend3_raw.csv");
        protected static IEnumerable<DataPoint> RawDataForTrend3Mini() => s_ser.Read("data/swinging-door/trend3_mini_raw.csv");
        protected static IEnumerable<DataPoint> RawDataForMaxDelta()   => s_ser.Read("data/swinging-door/maxDelta_raw.csv");

        protected static IEnumerable<DataPoint> ExpectedForTrend()      => s_ser.Read("data/swinging-door/trend_compressed.csv");
        protected static IEnumerable<DataPoint> ExpectedForTrend1()     => s_ser.Read("data/swinging-door/trend1_compressed.csv");
        protected static IEnumerable<DataPoint> ExpectedForTrend2()     => s_ser.Read("data/swinging-door/trend2_compressed.csv");
        protected static IEnumerable<DataPoint> ExpectedForTrend3()     => s_ser.Read("data/swinging-door/trend3_compressed.csv");
        protected static IEnumerable<DataPoint> ExpectedForTrend3Mini() => s_ser.Read("data/swinging-door/trend3_mini_compressed.csv");
        protected static IEnumerable<DataPoint> ExpectedForMaxDelta()   => s_ser.Read("data/swinging-door/maxDelta_compressed.csv");
        //---------------------------------------------------------------------
        protected static IEnumerable<DataPoint> RawMinDeltaX()
        {
            RawMinDeltaXCounter++;

            yield return ( 0d,  2d);
            yield return ( 1d,  2d);
            yield return ( 2d,  2d);
            yield return ( 3d,  2d);
            yield return ( 4d,  2d);
            yield return ( 5d, 10d);
            yield return ( 6d,  3d);
            yield return ( 7d,  3d);
            yield return ( 8d,  3d);
            yield return ( 9d,  3d);
        }
        //---------------------------------------------------------------------
        protected static IEnumerable<DataPoint> ExpectedMinDeltaX()
        {
            yield return ( 0d,  2d);
            yield return ( 4d,  2d);
            yield return ( 5d, 10d);
            yield return ( 7d,  3d);
            yield return ( 9d,  3d);
        }
        //---------------------------------------------------------------------
#if NETCOREAPP
        protected static IEnumerable<TestCaseData> IAsyncEnumerableTestCases()
        {
            // https://docs.nunit.org/articles/nunit/running-tests/Template-Based-Test-Naming.html
            yield return new TestCaseData(1.0, RawDataAsync(RawDataForTrend()) , ExpectedForTrend()) .SetName("{m} trend");
            yield return new TestCaseData(0.1, RawDataAsync(RawDataForTrend1()), ExpectedForTrend1()).SetName("{m} trend1");
            yield return new TestCaseData(0.1, RawDataAsync(RawDataForTrend2()), ExpectedForTrend2()).SetName("{m} trend2");
            yield return new TestCaseData(2.0, RawDataAsync(RawDataForTrend3()), ExpectedForTrend3()).SetName("{m} trend3");
            yield return new TestCaseData(2.0, RawDataAsync(RawDataForTrend3Mini()), ExpectedForTrend3Mini()).SetName("{m} trend3_mini");
        }
        //---------------------------------------------------------------------
        protected static async IAsyncEnumerable<DataPoint> RawDataAsync(IEnumerable<DataPoint> rawData, [EnumeratorCancellation] CancellationToken ct = default)
        {
            foreach (DataPoint dp in rawData)
            {
                ct.ThrowIfCancellationRequested();
                await Task.Yield();
                yield return dp;
            }
        }
#endif
    }
}
