// (c) gfoidl, all rights reserved

using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using gfoidl.DataCompression.Benchmarks.Infrastructure;

namespace gfoidl.DataCompression.Benchmarks;

[BenchmarkCategory("Instantiation")]
[ShortRunJob]   // Care only about allocations
[GenericTypeArguments(typeof(DeadBandCompressionFactory))]
[GenericTypeArguments(typeof(SwingingDoorCompressionFactory))]
public class IteratorInstantiaton<TFactory> : Base where TFactory : ICompressionFactory, new()
{
    private const int OperationsForMultiple = 1_000;
    private const int Count                 = 2;

    private readonly DataPoint[]  _dataPoints = { new DataPoint((0, 1)), new DataPoint((1, 0)) };
    private readonly ICompression _compression;
    //---------------------------------------------------------------------
    public IteratorInstantiaton()
    {
        TFactory factory = new();
        _compression     = factory.Create();
    }
    //---------------------------------------------------------------------
    [Benchmark]
    public double SingleIteration()
    {
        using DataPointIterator iterator = _compression.Process(_dataPoints);
        return Consume(iterator);
    }
    //---------------------------------------------------------------------
    [Benchmark]
    public async ValueTask<double> SingleIterationASync()
    {
        IAsyncEnumerable<DataPoint> source = this.SourceAsync(Count);
        using DataPointIterator iterator   = _compression.ProcessAsync(source);
        return await ConsumeAsync(iterator);
    }
    //---------------------------------------------------------------------
    [Benchmark(OperationsPerInvoke = OperationsForMultiple)]
    public double MultipleIterations()
    {
        double sum = 0;

        for (int i = 0; i < OperationsForMultiple; ++i)
        {
            using DataPointIterator iterator = _compression.Process(_dataPoints);
            sum += Consume(iterator);
        }

        return sum;
    }
    //---------------------------------------------------------------------
    [Benchmark(OperationsPerInvoke = OperationsForMultiple)]
    public async ValueTask<double> MultipleIterationsAsync()
    {
        double sum = 0;

        for (int i = 0; i < OperationsForMultiple; ++i)
        {
            IAsyncEnumerable<DataPoint> source = this.SourceAsync(Count);
            using DataPointIterator iterator   = _compression.ProcessAsync(source);
            sum += await ConsumeAsync(iterator);
        }

        return sum;
    }
}
