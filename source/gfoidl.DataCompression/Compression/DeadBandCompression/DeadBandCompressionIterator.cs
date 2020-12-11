// (c) gfoidl, all rights reserved

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace gfoidl.DataCompression.Internal.DeadBand
{
    internal abstract class DeadBandCompressionIterator : DataPointIterator
    {
        protected DeadBandCompression?     _deadBandCompression;
        protected (double Min, double Max) _bounding;
        //---------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void GetBounding(in DataPoint dataPoint)
        {
            Debug.Assert(_deadBandCompression is not null);

            double y = dataPoint.Y;

            // Produces better code than updating _bounding directly
            ref (double Min, double Max) bounding = ref _bounding;

            bounding.Min = y - _deadBandCompression.InstrumentPrecision;
            bounding.Max = y + _deadBandCompression.InstrumentPrecision;
        }
        //---------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal override ref (bool Archive, bool MaxDelta) IsPointToArchive(in DataPoint incoming, in DataPoint lastArchived)
        {
            ref (bool Archive, bool MaxDelta) archive = ref _archive;

            if (!this.IsMaxDeltaX(ref archive, incoming.X, lastArchived.X))
            {
                archive.Archive = incoming.Y < _bounding.Min || _bounding.Max < incoming.Y;
            }

            return ref archive;
        }
        //---------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void UpdatePoints(in DataPoint incoming, ref DataPoint snapShot)
        {
            _lastArchived = incoming;
            snapShot      = incoming;

            if (!_archive.MaxDelta) this.GetBounding(snapShot);
        }
        //---------------------------------------------------------------------
        protected internal override void Init(in DataPoint incoming, ref DataPoint snapShot)                   => this.UpdatePoints(incoming, ref snapShot);
        protected internal override void Init(int incomingIndex, in DataPoint incoming, ref int snapShotIndex) => throw new NotSupportedException();
        //---------------------------------------------------------------------
        protected override void DisposeCore()
        {
            _deadBandCompression = null;
            _bounding            = default;

            base.DisposeCore();
        }
    }
}
