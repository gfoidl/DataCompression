using System.Runtime.CompilerServices;

namespace gfoidl.DataCompression.Internal.DeadBand
{
    internal abstract class DeadBandCompressionIterator : DataPointIterator
    {
        protected readonly DeadBandCompression _deadBandCompression;
        protected (double Min, double Max)     _bounding;
        //-----------------------------------------------------------------
        protected DeadBandCompressionIterator(DeadBandCompression deadBandCompression)
            : base(deadBandCompression)
            => _deadBandCompression = deadBandCompression;
        //---------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void GetBounding(in DataPoint dataPoint)
        {
            double y = dataPoint.Y;

            // Produces better code than updating _bounding directly
            ref (double Min, double Max) bounding = ref _bounding;

            bounding.Min = y - _deadBandCompression.InstrumentPrecision;
            bounding.Max = y + _deadBandCompression.InstrumentPrecision;
        }
    }
}
