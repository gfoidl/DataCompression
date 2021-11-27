// (c) gfoidl, all rights reserved

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using gfoidl.DataCompression.Benchmarks.Infrastructure;

namespace gfoidl.DataCompression.Benchmarks
{
    [BenchmarkCategory("Instantiation")]
    [ShortRunJob]
    [GenericTypeArguments(typeof(DeadBandCompressionFactory))]
    [GenericTypeArguments(typeof(SwingingDoorCompressionFactory))]
    public class IteratorInstantiaton<TFactory> : Base where TFactory : ICompressionFactory, new()
    {
        private const int Count = 2;

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
            DataPointIterator iterator    = _compression.Process(_dataPoints);
            return this.Consume(iterator);
        }
        //---------------------------------------------------------------------
        //[Benchmark]
        public async ValueTask<double> SingleIterationASync()
        {
            IAsyncEnumerable<DataPoint> source = this.SourceAsync(Count);
            DataPointIterator iterator         = _compression.ProcessAsync(source);
            return await this.ConsumeAsync(iterator);
        }
        //---------------------------------------------------------------------
        [Benchmark]
        [Arguments(100)]
        [Arguments(1_000)]
        public double MultipleIterations(int n)
        {
            double sum = 0;

            for (int i = 0; i < n; ++i)
            {
                DataPointIterator iterator    = _compression.Process(_dataPoints);
                sum += this.Consume(iterator);
            }

            return sum;
        }
        //---------------------------------------------------------------------
        //[Benchmark]
        [Arguments(100)]
        [Arguments(1_000)]
        public async ValueTask<double> MultipleIterationsAsync(int n)
        {
            double sum = 0;

            for (int i = 0; i < n; ++i)
            {
                IAsyncEnumerable<DataPoint> source = this.SourceAsync(Count);
                DataPointIterator iterator         = _compression.ProcessAsync(source);
                sum += await this.ConsumeAsync(iterator);
            }

            return sum;
        }
    }
}
