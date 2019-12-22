using System;
using System.Threading.Tasks;
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
        //---------------------------------------------------------------------
#if NETCOREAPP
        [Test]
        public async Task MoveNextAsync___false()
        {
            DataPointIterator sut = DataPointIterator.Empty;

            Assert.AreSame(Array.Empty<DataPoint>(), await sut.ToArrayAsync());
            Assert.AreEqual(0, (await sut.ToListAsync()).Count);
        }
#endif
    }
}
