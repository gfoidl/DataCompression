using System.Collections.Generic;
using NUnit.Framework;

#if NETCOREAPP
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
#endif

namespace gfoidl.DataCompression.Tests.Compression.SwingingDoorCompressionTests
{
    [TestFixture]
    public abstract class Base
    {
        private static readonly DataPointSerializer s_ser = new DataPointSerializer();
        //---------------------------------------------------------------------
        protected static IEnumerable<DataPoint> RawDataForTrend()     => s_ser.Read("../../../../../doc/data/swinging-door/trend_raw.csv");
        protected static IEnumerable<DataPoint> ExpectedForTrend()    => s_ser.Read("../../../../../doc/data/swinging-door/trend_compressed.csv");
        protected static IEnumerable<DataPoint> RawDataForMaxDelta()  => s_ser.Read("../../../../../doc/data/swinging-door/maxDelta_raw.csv");
        protected static IEnumerable<DataPoint> ExpectedForMaxDelta() => s_ser.Read("../../../../../doc/data/swinging-door/maxDelta_compressed.csv");
        //---------------------------------------------------------------------
#if NETCOREAPP
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
        protected static async IAsyncEnumerable<DataPoint> RawDataForMaxDeltaAsync([EnumeratorCancellation] CancellationToken ct = default)
        {
            foreach (DataPoint dp in RawDataForMaxDelta())
            {
                ct.ThrowIfCancellationRequested();
                await Task.Yield();
                yield return dp;
            }
        }
#endif
    }
}
