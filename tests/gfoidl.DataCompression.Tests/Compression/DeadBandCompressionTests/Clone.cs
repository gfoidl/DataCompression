// (c) gfoidl, all rights reserved

using System.Linq;
using NUnit.Framework;

namespace gfoidl.DataCompression.Tests.Compression.DeadBandCompressionTests
{
    public class Clone : Base
    {
        [Test]
        public void Cloning_iterates_over_fresh_set()
        {
            var sut      = new DeadBandCompression(0.1);
            var data     = KnownSequence().ToArray().Select(dp => dp);
            var expected = KnownSequence().ToArray();
            var filter   = sut.Process(data);

            var iterator = filter.GetEnumerator();
            iterator.MoveNext();

            iterator = filter.Clone().GetEnumerator();

            Assert.Multiple(() =>
            {
                int step = 0;
                Assert.IsTrue(iterator.MoveNext(), $"MoveNext step: {step}");
                Assert.AreEqual(expected[step], iterator.Current, $"Equal step: {step}");
                step++;
                Assert.IsTrue(iterator.MoveNext(), $"MoveNext step: {step}");
                Assert.AreEqual(expected[step], iterator.Current, $"Equal step: {step}");
                step++;
                Assert.IsTrue(iterator.MoveNext(), $"MoveNext step: {step}");
                Assert.AreEqual(expected[step], iterator.Current, $"Equal step: {step}");
                step++;
                Assert.IsFalse(iterator.MoveNext(), $"MoveNext step: {step++}");
            });
        }
    }
}
