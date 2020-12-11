// (c) gfoidl, all rights reserved

using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace gfoidl.DataCompression.Tests.DataPointTests
{
    [TestFixture]
    public class CalculatePoint
    {
        [Test]
        [TestCase(double.MinValue)]
        [TestCase(double.MaxValue)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NaN)]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(Math.PI)]
        public void X_same_as_of_Point___same_point_returned(double gradient)
        {
            DataPoint a = (42, 42);

            double actual = a.CalculatePoint(gradient, a.X);

            Assert.AreEqual(a.Y, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        [TestCaseSource(nameof(Point_gradient_x_given___correct_Point_returned_TestCases))]
        public void Point_gradient_x_given___correct_Point_returned(double gradient, double x, (double X, double Y) expected)
        {
            DataPoint a = (1d, 1d);

            double actual = a.CalculatePoint(gradient, x);

            Assert.AreEqual(expected.Y, actual, 1e-6, "y");
        }
        //---------------------------------------------------------------------
        private static IEnumerable<TestCaseData> Point_gradient_x_given___correct_Point_returned_TestCases()
        {
            yield return new TestCaseData( 1.5, 2d, (2d,  2.5));
            yield return new TestCaseData( 1.5, 0d, (0d, -0.5));
            yield return new TestCaseData(-1.5, 2d, (2d, -0.5));
            yield return new TestCaseData(-1.5, 0d, (0d,  2.5));
        }
    }
}