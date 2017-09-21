﻿using NUnit.Framework;

namespace gfoidl.DataCompression.Tests.DataPointTests
{
    [TestFixture]
    public class Ctor
    {
        [Test]
        public void X_and_Y_given___correct_property_values(
            [Values(double.MinValue, double.MinValue + 1, 0, double.MaxValue - 1, double.MaxValue)]double x,
            [Values(double.MinValue, double.MinValue + 1, 0, double.MaxValue - 1, double.MaxValue)]double y)
        {
            var actual = new DataPoint(x, y);

            Assert.AreEqual(x, actual.X, Constants.Epsilon);
            Assert.AreEqual(y, actual.Y, Constants.Epsilon);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Tuple_given___correct_property_values(
            [Values(double.MinValue, double.MinValue + 1, 0, double.MaxValue - 1, double.MaxValue)]double x,
            [Values(double.MinValue, double.MinValue + 1, 0, double.MaxValue - 1, double.MaxValue)]double y)
        {
            (double x, double y) tuple = (x, y);

            var actual = new DataPoint(tuple);

            Assert.AreEqual(x, actual.X, Constants.Epsilon);
            Assert.AreEqual(y, actual.Y, Constants.Epsilon);
        }
    }
}