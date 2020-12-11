// (c) gfoidl, all rights reserved

using System;
using gfoidl.DataCompression.Wrappers;
using NUnit.Framework;

namespace gfoidl.DataCompression.Tests.Wrappers.ArrayWrapperTests
{
    [TestFixture]
    public class Ctor
    {
        [Test]
        public void List_is_null___throws_ArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ArrayWrapper<int>(null));
        }
        //---------------------------------------------------------------------
        [Test]
        public void List_is_not_null___OK()
        {
            int[] array = { 0, 1, 2 };

            var actual = new ArrayWrapper<int>(array);

            Assert.IsNotNull(actual);
        }
    }
}
