using System.Collections.Generic;
using System.Linq;
using gfoidl.DataCompression.Builders;
using NUnit.Framework;

namespace gfoidl.DataCompression.Tests.Builders.ArrayBuilderTests
{
    [TestFixture]
    public class AddRange
    {
        [Test]
        public void IEnumerable___correct_result()
        {
            int[] expected = Getvalues().ToArray();
            var sut        = new ArrayBuilder<int>(true);

            sut.AddRange(Getvalues());

            int[] actual = sut.ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Array___correct_result()
        {
            int[] expected = Getvalues().ToArray();
            var sut        = new ArrayBuilder<int>(true);

            sut.AddRange(expected);

            int[] actual = sut.ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        private static IEnumerable<int> Getvalues()
        {
            yield return 0;
            yield return 1;
            yield return 2;
            yield return 4;
            yield return 8;
            yield return 16;
            yield return 100;
            yield return 1_000;
            yield return 1_001;
        }
    }
}
