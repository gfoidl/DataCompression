using System;
using NUnit.Framework;

namespace gfoidl.DataCompression.Tests.DataPointTests
{
    [TestFixture]
    public class ToTimeValue
    {
        [Test]
        public void DateTime_and_value_given___OK()
        {
            var dt       = new DateTime(1982, 7, 22, 23, 35, 40);
            double value = Math.PI;

            var sut = new DataPoint(dt, value);

            var actual = sut.ToTimeValue();

            Assert.AreEqual(dt, actual.Time);
            Assert.AreEqual(value, actual.Value);
        }
    }
}