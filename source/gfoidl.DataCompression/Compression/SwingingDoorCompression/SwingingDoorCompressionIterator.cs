// (c) gfoidl, all rights reserved

using System.Runtime.CompilerServices;
using System;
using System.Diagnostics;

namespace gfoidl.DataCompression.Internal.SwingingDoor
{
    internal abstract class SwingingDoorCompressionIterator : DataPointIterator
    {
        protected static readonly (double Max, double Min) s_newDoor = (double.PositiveInfinity, double.NegativeInfinity);
        //---------------------------------------------------------------------
        protected SwingingDoorCompression? _swingingDoorCompression;
        protected (double Max, double Min) _slope;
        //---------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal sealed override ref (bool Archive, bool MaxDelta) IsPointToArchive(in DataPoint incoming, in DataPoint lastArchived)
        {
            ref (bool Archive, bool MaxDelta) archive = ref _archive;

            if (!this.IsMaxDeltaX(ref archive, lastArchived.X, incoming.X))
            {
                // Better to compare via gradient (1 calculation) than comparing to allowed y-values (2 calcuations)
                // Obviously, the result should be the same ;-)
                double slopeToIncoming = lastArchived.Gradient(incoming);
                archive.Archive        = slopeToIncoming < _slope.Min || _slope.Max < slopeToIncoming;
            }

            return ref archive;
        }
        //---------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void CloseTheDoor(in DataPoint incoming, in DataPoint lastArchived)
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
        //---------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void OpenNewDoor() => _slope = s_newDoor;
        //---------------------------------------------------------------------
        // TODO: check arguments if they are needed
        protected internal sealed override void Init(in DataPoint incoming, ref DataPoint snapShot)             => this.OpenNewDoor();
        protected internal sealed override void UpdateFilters(in DataPoint incoming, in DataPoint lastArchived) => this.CloseTheDoor(incoming, lastArchived);
        protected internal override void Init(int incomingIndex, in DataPoint incoming, ref int snapShotIndex) => throw new NotSupportedException();
        //---------------------------------------------------------------------
        protected override void DisposeCore()
        {
            _swingingDoorCompression = null;
            _slope                   = default;

            base.DisposeCore();
        }
    }
}
