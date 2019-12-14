using System.Collections.Generic;
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
    }
}
