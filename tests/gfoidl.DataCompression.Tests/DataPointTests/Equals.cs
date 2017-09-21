using NUnit.Framework;

namespace gfoidl.DataCompression.Tests.DataPointTests
{
    [TestFixture]
    public class Equals
    {
        [Test]
        public void Type_other_than_DataPoint_given___false()
        {
            var sut  = new DataPoint();
            object o = new object();

            bool actual = sut.Equals(o);

            Assert.IsFalse(actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Same_other_given___true(
            [Values(double.MinValue, double.MinValue + 1, 0, double.MaxValue - 1, double.MaxValue)]double x,
            [Values(double.MinValue, double.MinValue + 1, 0, double.MaxValue - 1, double.MaxValue)]double y)
        {
            var sut   = new DataPoint(x, y);
            var other = new DataPoint(x, y);

            bool actual = sut.Equals(other);

            Assert.IsTrue(actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Same_other_as_object_given___true(
            [Values(double.MinValue, double.MinValue + 1, 0, double.MaxValue - 1, double.MaxValue)]double x,
            [Values(double.MinValue, double.MinValue + 1, 0, double.MaxValue - 1, double.MaxValue)]double y)
        {
            var sut      = new DataPoint(x, y);
            object other = new DataPoint(x, y);

            bool actual = sut.Equals(other);

            Assert.IsTrue(actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Different_other_given___true()
        {
            var sut   = new DataPoint(0, 0);
            var other = new DataPoint(1e-150, 1e-150);

            bool actual = sut.Equals(other);

            Assert.IsFalse(actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Different_other_as_object_given___true()
        {
            var sut   = new DataPoint(0, 0);
            var other = new DataPoint(1e-150, 1e-150);

            bool actual = sut.Equals(other);

            Assert.IsFalse(actual);
        }
    }
}