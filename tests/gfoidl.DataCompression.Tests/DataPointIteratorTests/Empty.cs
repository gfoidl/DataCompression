using System;
using NUnit.Framework;

namespace gfoidl.DataCompression.Tests.DataPointIteratorTests
{
    [TestFixture]
    public class Empty
    {
        [Test]
        public void MoveNext___false()
        {
            DataPointIterator sut = DataPointIterator.Empty;

            Assert.IsFalse(sut.MoveNext());
            Assert.AreSame(sut, sut.Clone());
            Assert.AreSame(Array.Empty<DataPoint>(), sut.ToArray());
            Assert.AreEqual(0, sut.ToList().Count);
        }
    }
}
