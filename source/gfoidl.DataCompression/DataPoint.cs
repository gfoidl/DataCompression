using System;

namespace gfoidl.DataCompression
{
    public struct DataPoint : IEquatable<DataPoint>
    {
        public double X { get; }
        public double Y { get; }
        //---------------------------------------------------------------------
        public DataPoint((double x, double y) point)
        {
            this.X = point.x;
            this.Y = point.y;
        }
        //---------------------------------------------------------------------
        public DataPoint(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }
        //---------------------------------------------------------------------
        public DataPoint(DateTime time, double value)
        {
            this.X = time.Ticks;
            this.Y = value;
        }
        //---------------------------------------------------------------------
        public (DateTime Time, double Value) ToTimeValue() => (new DateTime((long)this.X), this.Y);
        //---------------------------------------------------------------------
        public bool Equals(DataPoint other) => this.X == other.X && this.Y == other.Y;
        //---------------------------------------------------------------------
        public override bool Equals(object obj)
        {
            if (obj is DataPoint other) return this.Equals(other);

            return false;
        }
        //---------------------------------------------------------------------
        public static bool operator ==(DataPoint a, DataPoint b) => a.Equals(b);
        public static bool operator !=(DataPoint a, DataPoint b) => !a.Equals(b);
        //---------------------------------------------------------------------
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
        public override string ToString() => $"({this.X}, {this.Y})";
        //---------------------------------------------------------------------
        public static implicit operator DataPoint((double, double) tuple)   => new DataPoint(tuple);
        public static implicit operator DataPoint((DateTime, double) tuple) => new DataPoint(tuple.Item1, tuple.Item2);
        public static implicit operator (DateTime, double) (DataPoint dp)   => dp.ToTimeValue();
    }
}