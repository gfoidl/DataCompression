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
            yield return (0,  1);
            yield return (1,  2);
            yield return (2,  2);
            yield return (3, -1);
            yield return (4,  3);
        }
        //---------------------------------------------------------------------
        protected static IEnumerable<DataPoint> KnownSequenceExpected(bool swingingDoor = true)
        {
            yield return (0, 1);

            if (!swingingDoor)
            {
                yield return (1, 2);
            }

            yield return (2,  2);
            yield return (3, -1);
            yield return (4,  3);
        }
        //---------------------------------------------------------------------
        protected static IEnumerable<DataPoint> KnownSequenceWithConstantPart()
        {
            yield return (0,  0);
            yield return (1,  1);
            yield return (2,  1);
            yield return (3,  1);
            yield return (4,  1);
            yield return (5,  1);
            yield return (6, -1);
            yield return (7,  4);
        }
        //---------------------------------------------------------------------
        protected static IEnumerable<DataPoint> KnownSequenceWithConstantPartExpected(bool swingingDoor = true)
        {
            yield return (0,  0);

            if (!swingingDoor)
            {
                yield return (1, 1);
            }

            yield return (5,  1);
            yield return (6, -1);
            yield return (7,  4);
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
            yield return (2, 2);
            await Task.Yield();
            yield return (3, -1);
            await Task.Yield();
            yield return (4, 3);
        }
#endif
    }
}
