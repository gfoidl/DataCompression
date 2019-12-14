using System.Collections.Generic;
using gfoidl.DataCompression.Wrappers;
using NUnit.Framework;

namespace gfoidl.DataCompression.Tests.Wrappers.ListWrapperTests
{
    [TestFixture]
    public class Indexer
    {
        [Test]
        public void Get___OK()
        {
            var list = new List<int> { 0, 1, 2 };
            var sut  = new ListWrapper<int>(list);

            int actual = sut[1];

            Assert.AreEqual(list[1], actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public void Set___OK()
        {
            var list = new List<int> { 0, 1, 2 };
            var sut  = new ListWrapper<int>(list);

            sut[2] = 42;

            Assert.AreEqual(42, list[2]);
        }
    }
}
