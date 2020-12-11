// (c) gfoidl, all rights reserved

using System.Collections.Generic;
using gfoidl.DataCompression.Builders;
using NUnit.Framework;

namespace gfoidl.DataCompression.Tests.Builders.ArrayBuilderTests
{
    [TestFixture]
    public class Add
    {
        [Test, TestCaseSource(nameof(Add_items_ToArray___correct_result_TestCases))]
        public void Add_items_ToArray___correct_result(int size)
        {
            var expected = new int[size];
            var sut      = new ArrayBuilder<int>(true);

            for (int i = 0; i < size; ++i)
            {
                expected[i] = i;
                sut.Add(i);
            }

            int[] actual = sut.ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        private static IEnumerable<TestCaseData> Add_items_ToArray___correct_result_TestCases()
        {
            yield return new TestCaseData(0);
            yield return new TestCaseData(1);
            yield return new TestCaseData(2);
            yield return new TestCaseData(4);
            yield return new TestCaseData(8);
            yield return new TestCaseData(16);
            yield return new TestCaseData(100);
            yield return new TestCaseData(1_000);
            yield return new TestCaseData(1_001);
        }
    }
}
