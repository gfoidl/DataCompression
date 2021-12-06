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
        protected internal sealed override ref (bool Archive, bool MaxDelta) IsPointToArchive(in DataPoint incoming, in DataPoint lastArchived)
        {
            ref (bool Archive, bool MaxDelta) archive = ref _archive;

            if (!this.IsMaxDeltaX(ref archive, lastArchived.X, incoming.X))
            {
                archive.Archive = incoming.Y < _bounding.Min || _bounding.Max < incoming.Y;
            }

            return ref archive;
        }
        //---------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdatePoints(in DataPoint incoming)
        {
            if (!_archive.MaxDelta)
            {
                this.GetBounding(incoming);
            }
        }
        //---------------------------------------------------------------------
        protected internal sealed override void Init(in DataPoint incoming) => this.UpdatePoints(incoming);

        // Override even if empty body, but this type is sealed so the virtual dispatch could be
        // eliminated by the JIT.
        protected internal sealed override void UpdateFilters(in DataPoint incoming, in DataPoint lastArchived) { }
        //---------------------------------------------------------------------
        protected override void DisposeCore()
        {
            _deadBandCompression = null;
            _bounding            = default;

            base.DisposeCore();
        }
    }
}
