using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace gfoidl.DataCompression.Benchmarks
{
    //[DisassemblyDiagnoser(printSource: true)]
    public class GradientBenchmarks
    {
        public static void Run()
        {
            var benchs = new GradientBenchmarks();
            benchs.GlobalSetup();
            Console.WriteLine(benchs.Normal());
            Console.WriteLine(benchs.Simd1());
            Console.WriteLine(benchs.Simd2());
            //Console.WriteLine(benchs.Simd3());        // not correct result
#if !DEBUG
            BenchmarkRunner.Run<GradientBenchmarks>();
#endif
        }
        //---------------------------------------------------------------------
        private DataPoint _a;
        private DataPoint _b;
        //---------------------------------------------------------------------
        [GlobalSetup]
        public void GlobalSetup()
        {
            var rnd = new Random(0);
#if DEBUG
            _a = new DataPoint(0, 0);
            _b = new DataPoint(2, 1);
#else
            _a = new DataPoint(rnd.NextDouble(), rnd.NextDouble());
            _b = new DataPoint(rnd.NextDouble(), rnd.NextDouble());
#endif
            Console.WriteLine($"a: {_a}");
            Console.WriteLine($"b: {_b}");
        }
        //---------------------------------------------------------------------
        [Benchmark(Baseline = true)]
        public double Normal() => _a.Gradient(_b);
        //---------------------------------------------------------------------
        [Benchmark]
        public double Simd1() => _a.GradientSimd1(_b);
        //---------------------------------------------------------------------
        [Benchmark]
        public double Simd2() => _a.GradientSimd2(_b);
        //---------------------------------------------------------------------
        //[Benchmark]           // not correct result
        public double Simd3() => _a.GradientSimd3(_b);
    }
    //-------------------------------------------------------------------------
    internal static class DataPointExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Gradient(this DataPoint a, in DataPoint b, bool return0OnEquality = true)
        {
            if (a == b)
            {
                if (return0OnEquality) return 0;
                ThrowHelper.ThrowArgument("a == b", nameof(b));
            }

            return (b.Y - a.Y) / (b.X - a.X);
        }
        //---------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe double GradientSimd1(this DataPoint a, DataPoint b, bool return0OnEquality = true)
        {
            if (a == b)
            {
                if (return0OnEquality) return 0;
                ThrowHelper.ThrowArgument("a == b", nameof(b));
            }

            if (Vector.IsHardwareAccelerated)
            {
                Vector<double> aa = Unsafe.Read<Vector<double>>(&a);
                Vector<double> bb = Unsafe.Read<Vector<double>>(&b);
                Vector<double> cc = bb - aa;

                return cc[1] / cc[0];
            }

            return (b.Y - a.Y) / (b.X - a.X);
        }
        //---------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe double GradientSimd2(this DataPoint a, DataPoint b, bool return0OnEquality = true)
        {
            if (a == b)
            {
                if (return0OnEquality) return 0;
                ThrowHelper.ThrowArgument("a == b", nameof(b));
            }

            if (Vector.IsHardwareAccelerated)
            {
                Vector<double> aa = Unsafe.Read<Vector<double>>(Unsafe.AsPointer(ref a));
                Vector<double> bb = Unsafe.Read<Vector<double>>(Unsafe.AsPointer(ref b));
                Vector<double> cc = bb - aa;

                return cc[1] / cc[0];
            }

            return (b.Y - a.Y) / (b.X - a.X);
        }
        //---------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe double GradientSimd3(this DataPoint a, DataPoint b, bool return0OnEquality = true)
        {
            if (a == b)
            {
                if (return0OnEquality) return 0;
                ThrowHelper.ThrowArgument("a == b", nameof(b));
            }

            if (Vector.IsHardwareAccelerated)
            {
                Vector<double> aa = Unsafe.As<DataPoint, Vector<double>>(ref a);
                Vector<double> bb = Unsafe.As<DataPoint, Vector<double>>(ref b);
                Vector<double> cc = bb - aa;

                return cc[1] / cc[0];
            }

            return (b.Y - a.Y) / (b.X - a.X);
        }
    }
}