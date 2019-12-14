using System;
using System.Collections.Generic;
using gfoidl.DataCompression.Wrappers;
using NUnit.Framework;

namespace gfoidl.DataCompression.Tests.Wrappers.ListWrapperTests
{
    [TestFixture]
    public class Ctor
    {
        [Test]
        public void List_is_null___throws_ArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ListWrapper<int>(null));
        }
        //---------------------------------------------------------------------
        [Test]
        public void List_is_not_null___OK()
        {
            var list = new List<int> { 0, 1, 2 };

            var actual = new ListWrapper<int>(list);

            Assert.IsNotNull(actual);
        }
    }
}
