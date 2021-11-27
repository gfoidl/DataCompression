// (c) gfoidl, all rights reserved

using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace gfoidl.DataCompression.Benchmarks
{
    [BenchmarkCategory(Categories.Compression, Categories.Async, Categories.Deadband)]
    public class DeadBandCompressionAsync : Base
    {
        [Benchmark]
        public async ValueTask<double> Enumerate()
        {
            IAsyncEnumerable<DataPoint> source = this.SourceAsync();
            DataPointIterator filtered         = source.DeadBandCompressionAsync(0.1);
            return await this.ConsumeAsync(filtered);
        }
        //---------------------------------------------------------------------
        [Benchmark]
        public async ValueTask<DataPoint[]> ToArray()
        {
            IAsyncEnumerable<DataPoint> source = this.SourceAsync();
            DataPointIterator filtered         = source.DeadBandCompressionAsync(0.1);
            return await filtered.ToArrayAsync();
        }
        //---------------------------------------------------------------------
        [Benchmark]
        public async ValueTask<List<DataPoint>> ToList()
        {
            IAsyncEnumerable<DataPoint> source = this.SourceAsync();
            DataPointIterator filtered         = source.DeadBandCompressionAsync(0.1);
            return await filtered.ToListAsync();
        }
    }
}
