// (c) gfoidl, all rights reserved

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace gfoidl.DataCompression.Tests.Compression.NoCompressionTests
{
    public class MoveNextAsync : Base
    {
        [Test]
        public async Task Empty_IAsyncEnumerable___empty_result()
        {
            var sut      = new NoCompression();
            var data     = EmptyAsync();
            var iterator = sut.ProcessAsync(data).GetAsyncEnumerator();

            Assert.IsFalse(await iterator.MoveNextAsync());
        }
        //---------------------------------------------------------------------
        [Test]
        public async Task Empty_IAsyncEnumerable_foreach___empty_result()
        {
            var sut  = new NoCompression();
            var data = EmptyAsync();

            int count = 0;
            await foreach (DataPoint db in sut.ProcessAsync(data))
            {
                count++;
            }

            Assert.AreEqual(0, count);
        }
        //---------------------------------------------------------------------
        [Test]
        public async Task Known_sequence___correct_result()
        {
            var sut      = new NoCompression();
            var data     = KnownSequenceAsync();
            var iterator = sut.ProcessAsync(data).GetAsyncEnumerator();
            var expected = new List<DataPoint>();

            await foreach (DataPoint dp in KnownSequenceAsync())
                expected.Add(dp);

            Assert.Multiple(async () =>
            {
                await iterator.MoveNextAsync();
                Assert.AreEqual(expected[0], iterator.Current);
                await iterator.MoveNextAsync();
                Assert.AreEqual(expected[1], iterator.Current);
                await iterator.MoveNextAsync();
                Assert.AreEqual(expected[2], iterator.Current);
            });
        }
        //---------------------------------------------------------------------
        [Test]
        public async Task Known_sequence_foreach___correct_result()
        {
            var sut      = new NoCompression();
            var data     = KnownSequenceAsync();
            var result   = sut.ProcessAsync(data);
            var expected = new List<DataPoint>();
            var actual   = new List<DataPoint>();

            await foreach (DataPoint dp in KnownSequenceAsync())
                expected.Add(dp);

            await foreach (DataPoint dp in result)
                actual.Add(dp);

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
