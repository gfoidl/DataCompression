using System;
using NUnit.Framework;

namespace gfoidl.DataCompression.Tests.Compression.NoCompressionTests
{
    public class MoveNextAsync : Base
    {
        [Test]
        public void MoveNext_without_GetEnumerator___throws_InvalidOperation()
        {
            var sut  = new NoCompression();
            var data = RawDataForTrendAsync();

            var iterator = sut.ProcessAsync(data);

            Assert.ThrowsAsync<InvalidOperationException>(async () => await iterator.MoveNextAsync());
        }
    }
}
