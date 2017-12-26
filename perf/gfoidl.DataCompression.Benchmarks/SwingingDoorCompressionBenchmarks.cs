using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace gfoidl.DataCompression.Benchmarks
{
	//[DisassemblyDiagnoser(printSource: true)]
	public class SwingingDoorCompressionBenchmarks
	{
		public static void Run()
		{
			var benchs = new SwingingDoorCompressionBenchmarks();
			benchs.N   = 1_000;
			benchs.GlobalSetup();
			Console.WriteLine(benchs.IEnumerable());
			Console.WriteLine(benchs.List());
			Console.WriteLine(benchs.Array());
#if !DEBUG
			BenchmarkRunner.Run<SwingingDoorCompressionBenchmarks>();
#endif
		}
		//---------------------------------------------------------------------
		[Params(1_000, 10_000)]
		public int N { get; set; }
		//---------------------------------------------------------------------
		private IEnumerable<DataPoint> _enumerable;
		private List<DataPoint> 	   _list;
		private DataPoint[] 		   _array;
		//---------------------------------------------------------------------
		[GlobalSetup]
		public void GlobalSetup()
		{
			var rnd = new Random(0);

			_list = Enumerable
				.Range(0, this.N)
				.Select(i => new DataPoint(i, rnd.NextDouble()))
				.ToList();

			_enumerable = _list.Select(d => d);
			_array 		= _enumerable.ToArray();
		}
		//---------------------------------------------------------------------
		[Benchmark(Baseline = true)]
		public int IEnumerable() => _enumerable.SwingingDoorCompression(0.25).Count();
		//---------------------------------------------------------------------
		[Benchmark]
		public int List() => _list.SwingingDoorCompression(0.25).Count();
		//---------------------------------------------------------------------
		[Benchmark]
		public int Array() => _array.SwingingDoorCompression(0.25).Count();
	}
}