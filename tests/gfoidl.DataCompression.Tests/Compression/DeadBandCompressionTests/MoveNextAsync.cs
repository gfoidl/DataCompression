// (c) gfoidl, all rights reserved

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace gfoidl.DataCompression.Tests.Compression.DeadBandCompressionTests
{
    public class MoveNextAsync : Base
    {
        [Test]
        public async Task Empty_IAsyncEnumerable___empty_result()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = EmptyAsync();
            var iterator = sut.ProcessAsync(data).GetAsyncEnumerator();

            Assert.IsFalse(await iterator.MoveNextAsync());
        }
        //---------------------------------------------------------------------
        [Test]
        public async Task Empty_IAsyncEnumerable_foreach___empty_result()
        {
            var sut  = new DeadBandCompression(0.1);
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
        public void Known_sequence___correct_result()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = KnownSequenceAsync();
            var iterator = sut.ProcessAsync(data).GetAsyncEnumerator();
            var expected = KnownSequenceExpected(swingingDoor: false).ToArray();

            Assert.Multiple(async () =>
            {
                await iterator.MoveNextAsync();
                Assert.AreEqual(expected[0], iterator.Current);
                await iterator.MoveNextAsync();
                Assert.AreEqual(expected[1], iterator.Current);
                await iterator.MoveNextAsync();
                Assert.AreEqual(expected[2], iterator.Current);
                await iterator.MoveNextAsync();
                Assert.AreEqual(expected[3], iterator.Current);
            });
        }
        //---------------------------------------------------------------------
        [Test]
        public async Task Known_sequence_foreach___correct_result()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = KnownSequenceAsync();
            var result   = sut.ProcessAsync(data);
            var expected = KnownSequenceExpected(swingingDoor: false).ToArray();
            var actual   = new List<DataPoint>();

            await foreach (DataPoint dp in result)
                actual.Add(dp);

            CollectionAssert.AreEqual(expected, actual);
        }
        //---------------------------------------------------------------------
        [Test]
        public async Task InstrumentPrecision_0___input_echoed()
        {
            var sut      = new DeadBandCompression(0);
            var data     = KnownSequenceAsync();
            var result   = sut.ProcessAsync(data);
            var expected = KnownSequence().ToArray();
            var actual   = new List<DataPoint>();

            await foreach (DataPoint dp in result)
                actual.Add(dp);

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
