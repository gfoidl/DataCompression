// (c) gfoidl, all rights reserved

using gfoidl.DataCompression.Wrappers;
using NUnit.Framework;

namespace gfoidl.DataCompression.Tests.Wrappers.ArrayWrapperTests
{
    [TestFixture]
    public class Indexer
    {
        [Test]
        public void Get___OK()
        {
            int[] array = { 0, 1, 2 };
            var sut     = new ArrayWrapper<int>(array);

            int actual = sut[1];

            Assert.AreEqual(array[1], actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Set___OK()
        {
            int[] array = { 0, 1, 2 };
            var sut     = new ArrayWrapper<int>(array);

            sut[2] = 42;

            Assert.AreEqual(42, array[2]);
        }
    }
}
