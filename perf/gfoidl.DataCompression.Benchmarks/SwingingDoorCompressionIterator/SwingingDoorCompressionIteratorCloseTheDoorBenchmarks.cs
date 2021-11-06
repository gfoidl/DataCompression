// (c) gfoidl, all rights reserved

using System.Diagnostics;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using Algorithm = gfoidl.DataCompression.SwingingDoorCompression;

namespace gfoidl.DataCompression.Benchmarks
{
    [DisassemblyDiagnoser]
    public class SwingingDoorCompressionIteratorCloseTheDoorBenchmarks
    {
        private readonly Algorithm       _swingingDoorCompression = new(1d);
        private (double Max, double Min) _slope                   = (double.PositiveInfinity, double.NegativeInfinity);

        private DataPoint _lastArchived = (1d, 0.5);
        private DataPoint _incoming     = (2d, 2d);
        //---------------------------------------------------------------------
        [Benchmark(Baseline = true)]
        public void Current() => this.Current(_incoming, _lastArchived);

        [Benchmark]
        public void Inlined() => this.Inlined(_incoming, _lastArchived);

        [Benchmark]
        public void Inlined1() => this.Inlined1(_incoming, _lastArchived);

        [Benchmark]
        public void Inlined2() => this.Inlined2(_incoming, _lastArchived);
        //---------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Current(in DataPoint incoming, in DataPoint lastArchived)
        {
            Debug.Assert(_swingingDoorCompression is not null);

            // TODO: vectorize
            double upperSlope = lastArchived.Gradient(incoming,  _swingingDoorCompression.CompressionDeviation);
            double lowerSlope = lastArchived.Gradient(incoming, -_swingingDoorCompression.CompressionDeviation);

            if (upperSlope < _slope.Max) _slope.Max = upperSlope;
            if (lowerSlope > _slope.Min) _slope.Min = lowerSlope;
        }
        //---------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Inlined(in DataPoint incoming, in DataPoint lastArchived)
        {
            Debug.Assert(_swingingDoorCompression is not null);

            double delta_x = incoming.X - lastArchived.X;

            double upperSlope;
            double lowerSlope;

            if (delta_x == 0)
            {
                upperSlope = lastArchived.GradientEquality(incoming, return0OnEquality: true);
                lowerSlope = lastArchived.GradientEquality(incoming, return0OnEquality: true);
            }
            else
            {
                double delta_y      = incoming.Y - lastArchived.Y;
                double delta_yUpper = delta_y + _swingingDoorCompression.CompressionDeviation;
                double delta_yLower = delta_y - _swingingDoorCompression.CompressionDeviation;

                upperSlope = delta_yUpper / delta_x;
                lowerSlope = delta_yLower / delta_x;
            }

            if (upperSlope < _slope.Max) _slope.Max = upperSlope;
            if (lowerSlope > _slope.Min) _slope.Min = lowerSlope;
        }
        //---------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Inlined1(in DataPoint incoming, in DataPoint lastArchived)
        {
            Debug.Assert(_swingingDoorCompression is not null);

            double delta_x = incoming.X - lastArchived.X;

            if (delta_x == 0)
            {
                GradientEquality(incoming, lastArchived);
                return;
            }

            double delta_y      = incoming.Y - lastArchived.Y;
            double delta_yUpper = delta_y + _swingingDoorCompression.CompressionDeviation;
            double delta_yLower = delta_y - _swingingDoorCompression.CompressionDeviation;

            double upperSlope = delta_yUpper / delta_x;
            double lowerSlope = delta_yLower / delta_x;

            if (upperSlope < _slope.Max) _slope.Max = upperSlope;
            if (lowerSlope > _slope.Min) _slope.Min = lowerSlope;

            [MethodImpl(MethodImplOptions.NoInlining)]
            void GradientEquality(in DataPoint incoming, in DataPoint lastArchived)
            {
                double upperSlope = lastArchived.GradientEquality(incoming, return0OnEquality: true);
                double lowerSlope = lastArchived.GradientEquality(incoming, return0OnEquality: true);

                if (upperSlope < _slope.Max) _slope.Max = upperSlope;
                if (lowerSlope > _slope.Min) _slope.Min = lowerSlope;
            }
        }
        //---------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Inlined2(in DataPoint incoming, in DataPoint lastArchived)
        {
            Debug.Assert(_swingingDoorCompression is not null);

            double delta_x = incoming.X - lastArchived.X;

            if (delta_x > 0)
            {
                double delta_y      = incoming.Y - lastArchived.Y;
                double delta_yUpper = delta_y + _swingingDoorCompression.CompressionDeviation;
                double delta_yLower = delta_y - _swingingDoorCompression.CompressionDeviation;

                double upperSlope = delta_yUpper / delta_x;
                double lowerSlope = delta_yLower / delta_x;

                if (upperSlope < _slope.Max) _slope.Max = upperSlope;
                if (lowerSlope > _slope.Min) _slope.Min = lowerSlope;
            }
            else
            {
                GradientEquality(incoming, lastArchived);
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            void GradientEquality(in DataPoint incoming, in DataPoint lastArchived)
            {
                double upperSlope = lastArchived.GradientEquality(incoming, return0OnEquality: true);
                double lowerSlope = lastArchived.GradientEquality(incoming, return0OnEquality: true);

                if (upperSlope < _slope.Max) _slope.Max = upperSlope;
                if (lowerSlope > _slope.Min) _slope.Min = lowerSlope;
            }
        }
    }
}
