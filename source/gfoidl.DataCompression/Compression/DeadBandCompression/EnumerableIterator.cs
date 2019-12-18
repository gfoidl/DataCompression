using System.Runtime.CompilerServices;

namespace gfoidl.DataCompression.Internal.DeadBand
{
    internal abstract class EnumerableIterator : DeadBandCompressionIterator
    {
        protected EnumerableIterator(DeadBandCompression deadBandCompression)
            : base(deadBandCompression)
        { }
        //-----------------------------------------------------------------
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override ref (bool Archive, bool MaxDelta) IsPointToArchive(in DataPoint incoming, in DataPoint lastArchived)
        {
            ref (bool Archive, bool MaxDelta) archive = ref _archive;

            if (!this.IsMaxDeltaX(ref archive, incoming, lastArchived))
            {
                archive.Archive = incoming.Y < _bounding.Min || _bounding.Max < incoming.Y;
            }

            return ref archive;
        }
        //-----------------------------------------------------------------
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void UpdatePoints(in DataPoint incoming, ref DataPoint snapShot)
        {
            _lastArchived = incoming;
            snapShot = incoming;

            if (!_archive.MaxDelta) this.GetBounding(snapShot);
        }
    }
}
