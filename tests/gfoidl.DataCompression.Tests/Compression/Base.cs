// (c) gfoidl, all rights reserved

using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;
using System.IO;

#if NETCOREAPP
using System.Threading.Tasks;
#endif

namespace gfoidl.DataCompression.Tests.Compression
{
    [TestFixture]
    public abstract class Base
    {
        protected static readonly DataPointSerializer s_ser = new();
        //-------------------------------------------------------------------------
        protected static void Print(IEnumerable<DataPoint> dataPoints, string header = null)
        {
            TestContext.WriteLine();

            if (header != null)
            {
                TestContext.WriteLine(header);
            }

            foreach (DataPoint dp in dataPoints)
            {
                TestContext.WriteLine(dp);
            }
        }
        //---------------------------------------------------------------------
        protected static IEnumerable<TestCaseData> TwoDataPointsTestCases()
        {
            // https://docs.nunit.org/articles/nunit/running-tests/Template-Based-Test-Naming.html
            yield return new TestCaseData(KnownSequence()  .Take(2), KnownSequence()  .Take(2).ToList()).SetName("{m} known sequence");
            yield return new TestCaseData(RawDataForTrend().Take(2), RawDataForTrend().Take(2).ToList()).SetName("{m} raw data for trend");

            static IEnumerable<DataPoint> RawDataForTrend()
            {
#if NETCOREAPP
                return s_ser.Read("data/dead-band/trend_raw.csv");
#else
                string basePath = TestContext.CurrentContext.TestDirectory;
                basePath        = Path.Combine(basePath, "data", "dead-band", "trend_raw.csv");
                return s_ser.Read(basePath);
#endif
            }
        }
        //-------------------------------------------------------------------------
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
