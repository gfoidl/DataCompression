// (c) gfoidl, all rights reserved

using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace gfoidl.DataCompression.Tests.Compression;

[TestFixture(typeof(DeadBandCompression))]
[TestFixture(typeof(NoCompression))]
[TestFixture(typeof(SwingingDoorCompression))]
public class DisposeTests<TCompression> : Base where TCompression : ICompression, new()
{
    private ICompression _sut = new TCompression();
    //-------------------------------------------------------------------------
    [Test]
    public void Enumerable_Dispose_after_foreach___OK()
    {
        using DataPointIterator filtered = _sut.Process(KnownSequence().Take(1));
        _ = Consume(filtered);
    }
    //-------------------------------------------------------------------------
    [Test]
    public void Array_Dispose_after_foreach___OK()
    {
        using DataPointIterator filtered = _sut.Process(KnownSequence().Take(1).ToArray());
        _ = Consume(filtered);
    }
    //-------------------------------------------------------------------------
    [Test]
    public void List_Dispose_after_foreach___OK()
    {
        using DataPointIterator filtered = _sut.Process(KnownSequence().Take(1).ToList());
        _ = Consume(filtered);
    }
    //-------------------------------------------------------------------------
    private static double Consume(DataPointIterator dataPointIterator)
    {
        double sum = 0;

        foreach (DataPoint dataPoint in dataPointIterator)
        {
            sum += dataPoint.Y;
        }

        return sum;
    }
    //-------------------------------------------------------------------------
#if NETCOREAPP
    [Test]
    public async Task AsyncEnumerable_Dispose_after_foreach___OK()
    {
        using DataPointIterator filtered = _sut.ProcessAsync(KnownSequenceAsync());
        _ = await ConsumeAsync(filtered);
    }
    //-------------------------------------------------------------------------
    private static async ValueTask<double> ConsumeAsync(DataPointIterator dataPointIterator)
    {
        double sum = 0;

        await foreach (DataPoint dataPoint in dataPointIterator)
        {
            sum += dataPoint.Y;
        }

        return sum;
    }
#endif
}
