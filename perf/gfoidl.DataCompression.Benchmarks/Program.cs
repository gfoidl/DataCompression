using BenchmarkDotNet.Running;
using gfoidl.DataCompression.Benchmarks;

BenchmarkSwitcher.FromAssembly(typeof(Base).Assembly).Run(args);
