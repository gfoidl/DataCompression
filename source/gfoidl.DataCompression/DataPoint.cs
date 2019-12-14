using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace gfoidl.DataCompression
{
    /// <summary>
    /// A (x,y) point.
    /// </summary>
    public readonly struct DataPoint : IEquatable<DataPoint>
    {
        private static readonly DataPoint s_origin = new DataPoint();
        //---------------------------------------------------------------------
        /// <summary>
        /// x value
        /// </summary>
        public double X { get; }
        //---------------------------------------------------------------------
        /// <summary>
        /// y value
        /// </summary>
        public double Y { get; }
        //---------------------------------------------------------------------
        /// <summary>
        /// The Origin, a <see cref="DataPoint" /> with (0, 0).
        /// </summary>
        public static ref readonly DataPoint Origin => ref s_origin;
        //---------------------------------------------------------------------
        /// <summary>
        /// Creates a new <see cref="DataPoint" />
        /// </summary>
        /// <param name="point">A tuple of (x,y)</param>
        public DataPoint((double x, double y) point)
        {
            this.X = point.x;
            this.Y = point.y;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Creates a new <see cref="DataPoint" />
        /// </summary>
        /// <param name="x">x value</param>
        /// <param name="y">y value</param>
        public DataPoint(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Creates a new <see cref="DataPoint" />
        /// </summary>
        /// <param name="time">
        /// time value -- is converted to <see cref="X" /> by using <see cref="DateTime.Ticks" />
        /// </param>
        /// <param name="value"></param>
        public DataPoint(DateTime time, double value)
        {
            this.X = time.Ticks;
            this.Y = value;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Converts the (x,y) point to (t,y) point
        /// </summary>
        /// <returns>(t,y) point</returns>
        /// <remarks>
        /// For conversion of <see cref="X" /> to time <see cref="DateTime.Ticks" />
        /// is used.
        /// </remarks>
        public (DateTime Time, double Value) ToTimeValue() => (new DateTime((long)this.X), this.Y);
        //---------------------------------------------------------------------
        /// <summary>
        /// Calculates the gradient from this <see cref="DataPoint" /> to the
        /// <see cref="DataPoint" /> given with <paramref name="b" />.
        /// </summary>
        /// <param name="b">The second <see cref="DataPoint" />.</param>
        /// <param name="return0OnEquality">
        /// When <c>true</c> and <c>this.X == b.X</c> then <c>0</c> is returned,
        /// otherwise an <see cref="ArgumentException" /> is thrown.
        /// </param>
        /// <returns>The gradient from this to <paramref name="b" />.</returns>
        /// <exception cref="ArgumentException">
        /// Is thrown when <c>this.X == b.X</c>, and <paramref name="return0OnEquality"/>
        /// is <c>false</c>.
        /// </exception>
        [DebuggerStepThrough]
        public double Gradient(in DataPoint b, bool return0OnEquality = true)
        {
            double delta_y = b.Y - this.Y;
            double delta_x = b.X - this.X;

            if (delta_x == 0d) return this.GradientEquality(b, return0OnEquality);

            return delta_y / delta_x;
        }
        //---------------------------------------------------------------------
        [DebuggerStepThrough]
        internal double Gradient(in DataPoint b, double deltaY, bool return0OnEquality = true)
        {
            double delta_y = b.Y + deltaY - this.Y;
            double delta_x = b.X - this.X;

            if (delta_x == 0d) return this.GradientEquality(b, return0OnEquality);

            return delta_y / delta_x;
        }
        //---------------------------------------------------------------------
        // Uncommon code-path
        [MethodImpl(MethodImplOptions.NoInlining)]
        private double GradientEquality(in DataPoint b, bool return0OnEquality)
        {
            if (return0OnEquality)
            {
                if (this.Y == b.Y)
                {
                    return 0;
                }
                else
                {
                    return this.Y < b.Y
                        ? double.PositiveInfinity
                        : double.NegativeInfinity;
                }
            }
            else
            {
                ThrowHelper.ThrowArgument(ThrowHelper.ExceptionResource.Gradient_A_eq_B);
                return 0;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Calculates the y-value of a point given by the current <see cref="DataPoint" />,
        /// the <paramref name="gradient" /> and an abscissa value <paramref name="x" />.
        /// </summary>
        /// <param name="gradient">The gradient.</param>
        /// <param name="x">The abscissa value.</param>
        /// <returns>The point's y-value.</returns>
        /// <remarks>
        /// <c>y = this.Y + gradient * (x - this.X)</c>
        /// </remarks>
        public double CalculatePoint(double gradient, double x)
        {
            double y = this.Y;
            x       -= this.X;

            if (x == 0) return y;

            return y + gradient * x;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Tests if the given <see cref="DataPoint" /> is equal to this one.
        /// </summary>
        /// <param name="other">The <see cref="DataPoint" /> to compare with this one.</param>
        /// <returns><c>true</c> if equal, <c>false</c> otherwise</returns>
        public bool Equals(DataPoint other) => this.X == other.X && this.Y == other.Y;
        //---------------------------------------------------------------------
        /// <summary>
        /// Tests if the given <see cref="DataPoint" /> is equal to this one.
        /// </summary>
        /// <param name="other">The <see cref="DataPoint" /> to compare with this one.</param>
        /// <param name="allowedDelta">The allowed tolerance.</param>
        /// <returns><c>true</c> if equal, <c>false</c> otherwise</returns>
        public bool Equals(DataPoint other, double allowedDelta)
        {
            return Math.Abs(this.X - other.X) < allowedDelta
                && Math.Abs(this.Y - other.Y) < allowedDelta;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Tests if the given <see cref="object" /> is equal to this one.
        /// </summary>
        /// <param name="obj">The object to compare with this one.</param>
        /// <returns><c>true</c> if equal, <c>false</c> otherwise</returns>
        public override bool Equals(object? obj) => obj is DataPoint other && this.Equals(other);
        //---------------------------------------------------------------------
        /// <summary>
        /// Tests for equality between the given <see cref="DataPoint" />s.
        /// </summary>
        /// <param name="a">First <see cref="DataPoint" /></param>
        /// <param name="b">Second <see cref="DataPoint" /></param>
        /// <returns><c>true</c> if equal, <c>false</c> otherwise</returns>
        public static bool operator ==(DataPoint a, DataPoint b) => a.Equals(b);
        //---------------------------------------------------------------------
        /// <summary>
        /// Tests for inequality between the given <see cref="DataPoint" />s.
        /// </summary>
        /// <param name="a">First <see cref="DataPoint" /></param>
        /// <param name="b">Second <see cref="DataPoint" /></param>
        /// <returns><c>true</c> if not equal, <c>false</c> if equal</returns>
        public static bool operator !=(DataPoint a, DataPoint b) => !a.Equals(b);
        //---------------------------------------------------------------------
        /// <summary>
        /// Returns the hash code for this intance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
#if NETCOREAPP
            return HashCode.Combine(this.X, this.Y);
#else
            const int primeA = 414_584_089;
            const int primeB = 534_448_147;

            unchecked
            {
                int hash = primeA;
                hash = hash * primeB + this.X.GetHashCode();
                hash = hash * primeB + this.Y.GetHashCode();

                return hash;
            }
#endif
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Returns a string that represents the current <see cref="DataPoint" />.
        /// </summary>
        /// <returns>
        /// A string that represents the current object in the form (x,y).
        /// </returns>
        public override string ToString() => $"({this.X}, {this.Y})";
        //---------------------------------------------------------------------
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static implicit operator DataPoint((double, double) tuple)   => new DataPoint(tuple);
        public static implicit operator DataPoint((DateTime, double) tuple) => new DataPoint(tuple.Item1, tuple.Item2);
        public static implicit operator (DateTime, double) (DataPoint dp)   => dp.ToTimeValue();
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
