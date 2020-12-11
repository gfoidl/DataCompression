// (c) gfoidl, all rights reserved

using System;
using System.Collections;
using System.Collections.Generic;
using gfoidl.DataCompression.Wrappers;
using NUnit.Framework;

namespace gfoidl.DataCompression.Tests.Wrappers.ListWrapperTests
{
    [TestFixture]
    public class NotImplementedMembers
    {
        [Test]
        public void Not_implemented_members___throws_NotSupported()
        {
            var list = new List<int> { 0, 1, 2 };
            var sut  = new ListWrapper<int>(list);

            Assert.Throws<NotSupportedException>(() => Assert.IsFalse(sut.IsReadOnly));
            Assert.Throws<NotSupportedException>(() => sut.Add(42));
            Assert.Throws<NotSupportedException>(() => sut.Clear());
            Assert.Throws<NotSupportedException>(() => sut.Contains(1));
            Assert.Throws<NotSupportedException>(() => sut.CopyTo(new int[3], 0));
            Assert.Throws<NotSupportedException>(() => sut.GetEnumerator());
            Assert.Throws<NotSupportedException>(() => sut.IndexOf(1));
            Assert.Throws<NotSupportedException>(() => sut.Insert(0, 0));
            Assert.Throws<NotSupportedException>(() => sut.Remove(1));
            Assert.Throws<NotSupportedException>(() => sut.RemoveAt(0));
            Assert.Throws<NotSupportedException>(() => (sut as IEnumerable)?.GetEnumerator());
        }
    }
}
