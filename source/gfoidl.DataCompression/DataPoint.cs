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
            this.X = time.ToOADate();
            this.Y = value;
        }
        //---------------------------------------------------------------------
        public (DateTime Time, double Value) ToTimeValue() => (DateTime.FromOADate(this.X), this.Y);
        //---------------------------------------------------------------------
        public bool Equals(DataPoint other) => this.X == other.X && this.Y == other.Y;
        //---------------------------------------------------------------------
        public override bool Equals(object obj)
        {
            if (obj is DataPoint other) return this.Equals(other);

            return false;
        }
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
    }
}