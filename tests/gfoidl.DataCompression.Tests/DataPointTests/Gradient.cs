using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace gfoidl.DataCompression.Tests.DataPointTests
{
    [TestFixture]
    public class Gradient
    {
        [Test]
        public void Gradient_to_same_point___throws_ArgumentException()
        {
            var a = new DataPoint(1, 1);
            var b = a;

            Assert.Throws<ArgumentException>(() => a.Gradient(b, false));
        }
        //---------------------------------------------------------------------
        [Test]
        [TestCaseSource(nameof(A_and_B_given___OK_TestCases))]
        public double A_and_B_given___OK((double, double) ta, (double, double) tb)
        {
            DataPoint a = ta;
            DataPoint b = tb;

            return a.Gradient(b);
        }
        //---------------------------------------------------------------------
        private static IEnumerable<TestCaseData> A_and_B_given___OK_TestCases()
        {
            yield return new TestCaseData((0d, 0d), (0d, 0d)).Returns(0d);
            yield return new TestCaseData((0d, 0d), (1d, 1d)).Returns(1d);
            yield return new TestCaseData((0d, 0d), (1d, -1d)).Returns(-1d);
            yield return new TestCaseData((0d, 0d), (1d, 0d)).Returns(0d);
            yield return new TestCaseData((0d, 0d), (-1d, 0d)).Returns(0d);
            yield return new TestCaseData((0d, 0d), (0d, 1d)).Returns(double.PositiveInfinity);
            yield return new TestCaseData((0d, 0d), (0d, -1d)).Returns(double.NegativeInfinity);
        }
    }
}