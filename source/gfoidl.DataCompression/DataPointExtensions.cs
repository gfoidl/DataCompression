﻿using System;

namespace gfoidl.DataCompression
{
    internal static class DataPointExtensions
    {
        public static double Gradient(this DataPoint a, DataPoint b, bool return0OnEquality = true)
        {
            if (a == b)
            {
                if (return0OnEquality) return 0;
                throw new ArgumentException(Strings.Gradient_A_eq_B, nameof(b));
            }

            return (b.Y - a.Y) / (b.X - a.X);
        }
        //---------------------------------------------------------------------
        public static DataPoint CalculatePoint(this DataPoint a, double gradient, double x)
        {
            if (a.X == x) return a;

            double y = a.Y + gradient * (x - a.X);

            return (x, y);
        }
    }
}