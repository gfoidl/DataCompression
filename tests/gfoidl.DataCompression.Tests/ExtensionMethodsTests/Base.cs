using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace gfoidl.DataCompression.Tests.ExtensionMethodsTests
{
    [TestFixture]
    public abstract class Base
    {
        protected static IEnumerable<DataPoint> GetDataPoints()
        {
            yield return new DataPoint(0, 0);
            yield return new DataPoint(1, 1);
        }
        //---------------------------------------------------------------------
#if NETCOREAPP
        protected static async IAsyncEnumerable<DataPoint> GetDataPointsAsync()
        {
            foreach (DataPoint dp in GetDataPoints())
            {
                await Task.Yield();
                yield return dp;
            }
        }
#endif
    }
}
