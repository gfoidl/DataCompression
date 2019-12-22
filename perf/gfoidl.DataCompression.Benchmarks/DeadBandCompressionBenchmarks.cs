﻿using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace gfoidl.DataCompression.Benchmarks
{
    public class DeadBandCompressionBenchmarks : Base
    {
        [Benchmark]
        public double Enumerate()
        {
            IEnumerable<DataPoint> source = this.Source();
            DataPointIterator filtered    = source.DeadBandCompression(0.1);
            return this.Consume(filtered);
        }
        //---------------------------------------------------------------------
        [Benchmark]
        public DataPoint[] ToArray()
        {
            IEnumerable<DataPoint> source = this.Source();
            DataPointIterator filtered    = source.DeadBandCompression(0.1);
            return filtered.ToArray();
        }
        //---------------------------------------------------------------------
        [Benchmark]
        public List<DataPoint> ToList()
        {
            IEnumerable<DataPoint> source = this.Source();
            DataPointIterator filtered    = source.DeadBandCompression(0.1);
            return filtered.ToList();
        }
    }
}
