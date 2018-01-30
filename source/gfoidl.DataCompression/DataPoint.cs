using System;

namespace gfoidl.DataCompression
{
    /// <summary>
    /// A (x,y) point.
    /// </summary>
    public readonly struct DataPoint : IEquatable<DataPoint>
    {
        private static readonly DataPoint _origin = new DataPoint();
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
        public static ref readonly DataPoint Origin => ref _origin;
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
            return
                Math.Abs(this.X - other.X) < allowedDelta
                && Math.Abs(this.Y - other.Y) < allowedDelta;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Tests if the given <see cref="object" /> is equal to this one.
        /// </summary>
        /// <param name="obj">The object to compare with this one.</param>
        /// <returns><c>true</c> if equal, <c>false</c> otherwise</returns>
        public override bool Equals(object obj) => obj is DataPoint other && this.Equals(other);
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
            const int primeA = 414_584_089;
            const int primeB = 534_448_147;

            unchecked
            {
                int hash = primeA;
                hash = hash * primeB + this.X.GetHashCode();
                hash = hash * primeB + this.Y.GetHashCode();

                return hash;
            }
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
