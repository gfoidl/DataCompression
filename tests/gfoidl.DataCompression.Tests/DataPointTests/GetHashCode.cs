// (c) gfoidl, all rights reserved

using NUnit.Framework;

namespace gfoidl.DataCompression.Tests.DataPointTests
{
    [TestFixture]
    public class GetHashCode
    {
        /*
         * Rule:
         * Equal object must have the same hash code!
         * Nothing more, nothing less.
         */
        [Test]
        public void Two_equal_DataPoints___same_hash_code(
            [Values(double.MinValue, double.MinValue + 1, 0, double.MaxValue - 1, double.MaxValue)]double x,
            [Values(double.MinValue, double.MinValue + 1, 0, double.MaxValue - 1, double.MaxValue)]double y)
        {
            var sut   = new DataPoint(x, y);
            var other = new DataPoint(x, y);

            int h1 = sut.GetHashCode();
            int h2 = sut.GetHashCode();

            Assert.AreEqual(h1, h2);
        }
    }
}