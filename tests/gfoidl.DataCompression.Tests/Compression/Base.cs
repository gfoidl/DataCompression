// (c) gfoidl, all rights reserved

using System.Collections.Generic;
using NUnit.Framework;

#if NETCOREAPP
using System.Threading.Tasks;
#endif

namespace gfoidl.DataCompression.Tests.Compression
{
    [TestFixture]
    public abstract class Base
    {
        protected static IEnumerable<DataPoint> Empty()
        {
            yield break;
        }
        //---------------------------------------------------------------------
        protected static IEnumerable<DataPoint> KnownSequence()
        {
            yield return (0, 1);
            yield return (1, 2);
            yield return (2, -1);
        }
        //---------------------------------------------------------------------
#if NETCOREAPP
        protected static async IAsyncEnumerable<DataPoint> EmptyAsync()
        {
            await Task.Yield();
            yield break;
        }
        //---------------------------------------------------------------------
        protected static async IAsyncEnumerable<DataPoint> KnownSequenceAsync()
        {
            await Task.Yield();
            yield return (0, 1);
            await Task.Yield();
            yield return (1, 2);
            await Task.Yield();
            yield return (2, -1);
        }
#endif
    }
}
