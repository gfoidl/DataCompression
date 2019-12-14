using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace gfoidl.DataCompression.Tests.Compression.DeadBandCompressionTests
{
    public class MoveNextAsync : Base
    {
        [Test]
        public void MoveNext_without_GetEnumerator___throws_InvalidOperation()
        {
            var sut  = new DeadBandCompression(1);
            var data = RawDataForTrendAsync();

            var iterator = sut.ProcessAsync(data);

            Assert.ThrowsAsync<InvalidOperationException>(async () => await iterator.MoveNextAsync());
        }
        //---------------------------------------------------------------------
        [Test]
        public async Task Empty_IAsyncEnumerable___empty_result()
        {
            var sut  = new DeadBandCompression(0.1);
            var data = Empty();

            var iterator = sut.ProcessAsync(data);

            Assert.IsFalse(await iterator.MoveNextAsync());
            //-----------------------------------------------------------------
            static async IAsyncEnumerable<DataPoint> Empty()
            {
                await Task.Yield();
                yield break;
            }
        }
    }
}
