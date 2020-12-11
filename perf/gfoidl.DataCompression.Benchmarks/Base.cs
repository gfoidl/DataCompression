// (c) gfoidl, all rights reserved

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
        protected IEnumerable<DataPoint> Source(int count = Count)
        {
            for (int i = 0; i < count; ++i)
            {
                double x = i;
                double y = _rnd!.NextDouble();

                yield return (x, y);
            }
        }
        //---------------------------------------------------------------------
        protected async IAsyncEnumerable<DataPoint> SourceAsync(int count = Count)
        {
            foreach (DataPoint dataPoint in this.Source(count))
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
