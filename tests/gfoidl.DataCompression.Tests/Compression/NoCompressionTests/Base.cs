// (c) gfoidl, all rights reserved

using System.Collections.Generic;

#if NETCOREAPP
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
#endif

namespace gfoidl.DataCompression.Tests.Compression.NoCompressionTests
{
    public abstract class Base : Compression.Base
    {
        private static readonly DataPointSerializer s_ser = new DataPointSerializer();
        //---------------------------------------------------------------------
        protected static IEnumerable<DataPoint> RawDataForTrend()    => s_ser.Read("../../../../../data/dead-band/trend_raw.csv");
        protected static IEnumerable<DataPoint> RawDataForMaxDelta() => s_ser.Read("../../../../../data/dead-band/maxDelta_raw.csv");
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
