using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace gfoidl.DataCompression.Benchmarks
{
    [MemoryDiagnoser]
    public abstract class Base
    {
        private const int Count = 1_000_000;
        private Random? _rnd;
        //---------------------------------------------------------------------
        [GlobalSetup]
        public void GlobalSetup()
        {
            _rnd = new Random(42);
        }
        //---------------------------------------------------------------------
        protected IEnumerable<DataPoint> Source()
        {
            for (int i = 0; i < Count; ++i)
            {
                double x = i;
                double y = _rnd!.NextDouble();

                yield return (x, y);
            }
        }
        //---------------------------------------------------------------------
        protected async IAsyncEnumerable<DataPoint> SourceAsync()
        {
            foreach (DataPoint dataPoint in this.Source())
            {
                await Task.Yield();
                yield return dataPoint;
            }
        }
        //---------------------------------------------------------------------
        protected double Consume(DataPointIterator iterator)
        {
            double sum = 0;

            foreach (DataPoint dataPoint in iterator)
            {
                sum += dataPoint.Y;
            }

            return sum;
        }
        //---------------------------------------------------------------------
        protected async ValueTask<double> ConsumeAsync(DataPointIterator iterator)
        {
            double sum = 0;

            await foreach (DataPoint dataPoint in iterator)
            {
                sum += dataPoint.Y;
            }

            return sum;
        }
    }
}
