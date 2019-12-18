using System.Runtime.CompilerServices;

namespace gfoidl.DataCompression.Internal.SwingingDoor
{
    internal abstract class SwingingDoorCompressionIterator : DataPointIterator
    {
        protected static readonly (double Max, double Min) s_newDoor = (double.PositiveInfinity, double.NegativeInfinity);
        //---------------------------------------------------------------------
        protected readonly SwingingDoorCompression _swingingDoorCompression;
        protected (double Max, double Min)         _slope;
        //---------------------------------------------------------------------
        protected SwingingDoorCompressionIterator(SwingingDoorCompression swingingDoorCompression)
            : base(swingingDoorCompression)
            => _swingingDoorCompression = swingingDoorCompression;
        //-----------------------------------------------------------------
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override ref (bool Archive, bool MaxDelta) IsPointToArchive(in DataPoint incoming, in DataPoint lastArchived)
        {
            ref (bool Archive, bool MaxDelta) archive = ref _archive;

            if (!this.IsMaxDeltaX(ref archive, incoming.X, lastArchived.X))
            {
                // Better to compare via gradient (1 calculation) than comparing to allowed y-values (2 calcuations)
                // Obviously, the result should be the same ;-)
                double slopeToIncoming = lastArchived.Gradient(incoming);
                archive.Archive        = slopeToIncoming < _slope.Min || _slope.Max < slopeToIncoming;
            }

            return ref archive;
        }
        //-----------------------------------------------------------------
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void CloseTheDoor(in DataPoint incoming, in DataPoint lastArchived)
        {
            double upperSlope = lastArchived.Gradient(incoming,  _swingingDoorCompression.CompressionDeviation);
            double lowerSlope = lastArchived.Gradient(incoming, -_swingingDoorCompression.CompressionDeviation);

            if (upperSlope < _slope.Max) _slope.Max = upperSlope;
            if (lowerSlope > _slope.Min) _slope.Min = lowerSlope;
        }
        //-----------------------------------------------------------------
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void OpenNewDoor(in DataPoint incoming)
        {
            _lastArchived = incoming;
            _slope        = s_newDoor;
        }
        //-----------------------------------------------------------------
        protected override void Init(in DataPoint incoming, ref DataPoint snapShot)             => this.OpenNewDoor(incoming);
        protected override void UpdateFilters(in DataPoint incoming, in DataPoint lastArchived) => this.CloseTheDoor(incoming, lastArchived);
    }
}
