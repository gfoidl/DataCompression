using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace gfoidl.DataCompression
{
    /// <summary>
    /// Base class for an iterator for <see cref="DataPoint" />s.
    /// </summary>
    /// <remarks>
    /// The state at creation is set to <see cref="InitialState" />.
    /// </remarks>
    public abstract partial class DataPointIterator : IEnumerable<DataPoint>, IEnumerator<DataPoint>
    {
        /// <summary>
        /// The state when the iterator is disposed.
        /// </summary>
        protected const int DisposedState = -3;
        //---------------------------------------------------------------------
        /// <summary>
        /// The initial state of the iterator.
        /// </summary>
        protected const int InitialState = -2;
        //---------------------------------------------------------------------
#pragma warning disable CS1591
        protected readonly Compression          _algorithm;
        protected int                           _state = InitialState;
        protected DataPoint                     _current;
        protected DataPoint                     _snapShot;
        protected DataPoint                     _lastArchived;
        protected DataPoint                     _incoming;
        protected (bool Archive, bool MaxDelta) _archive;
#pragma warning restore CS1591
        private readonly int _threadId;
        //---------------------------------------------------------------------
        /// <summary>
        /// Creates an instance of <see cref="DataPointIterator" />.
        /// </summary>
        protected DataPointIterator(Compression algorithm)
        {
            _threadId  = Environment.CurrentManagedThreadId;
            _algorithm = algorithm;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Gets the current item.
        /// </summary>
        public DataPoint Current => _current;
        //---------------------------------------------------------------------
        /// <summary>
        /// Gets the current item by reference.
        /// </summary>
        public ref DataPoint CurrentByRef => ref _current;
        //---------------------------------------------------------------------
        private static EmptyDataPointIterator? s_emptyIterator;
        /// <summary>
        /// Returns an <see cref="EmptyDataPointIterator" />.
        /// </summary>
        public static DataPointIterator Empty => s_emptyIterator ?? (s_emptyIterator = new EmptyDataPointIterator(NoCompression.s_instance));
        //---------------------------------------------------------------------
#pragma warning disable CS1591
        object IEnumerator.Current                                    => this.Current;
        IEnumerator<DataPoint> IEnumerable<DataPoint>.GetEnumerator() => this.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()                       => this.GetEnumerator();
        void IEnumerator.Reset()                                      => ThrowHelper.ThrowNotSupported();
#pragma warning restore CS1591
        //---------------------------------------------------------------------
        /// <summary>
        /// Clones the <see cref="DataPointIterator" />.
        /// </summary>
        /// <returns>The cloned <see cref="DataPointIterator" />.</returns>
        public abstract DataPointIterator Clone();
        //---------------------------------------------------------------------
        /// <summary>
        /// Gets an enumerator for the <see cref="DataPoint" />s.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public DataPointIterator GetEnumerator()
        {
            DataPointIterator enumerator =
                _state       == InitialState
                && _threadId == Environment.CurrentManagedThreadId
                ? this
                : this.Clone();

            enumerator._state = 0;
            return enumerator;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Resets the enumerator and its state.
        /// </summary>
        public virtual void Dispose()
        {
            _current = DataPoint.Origin;
            _state   = DisposedState;
            _enumerator?.Dispose();
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Prepares the algorithm for new data, e.g. opens a new door in the
        /// <see cref="SwingingDoorCompression" />.
        /// </summary>
        /// <param name="incoming">
        /// The <see cref="DataPoint" /> on which the initialisation is based on.
        /// </param>
        /// <param name="snapShot">The last snapshot.</param>
        protected abstract void Init(in DataPoint incoming, ref DataPoint snapShot);
        //---------------------------------------------------------------------
        /// <summary>
        /// Updates the filters.
        /// </summary>
        /// <param name="incoming">The incoming (i.e. latest) <see cref="DataPoint" />.</param>
        /// <param name="lastArchived">The last archived <see cref="DataPoint" />.</param>
        protected virtual void UpdateFilters(in DataPoint incoming, in DataPoint lastArchived) { }
        //---------------------------------------------------------------------
        /// <summary>
        /// Determines if the <paramref name="incoming" /> needs to be archived or not.
        /// </summary>
        /// <param name="incoming">The incoming (i.e. latest) <see cref="DataPoint" />.</param>
        /// <param name="lastArchived">The last archived <see cref="DataPoint" />.</param>
        /// <returns>State whether to archive or not plus additional info.</returns>
        protected abstract ref (bool Archive, bool MaxDelta) IsPointToArchive(in DataPoint incoming, in DataPoint lastArchived);
        //---------------------------------------------------------------------
        /// <summary>
        /// Determines if the incoming needs to be archived due <see cref="Compression.MaxDeltaX" />.
        /// </summary>
        /// <param name="archive">Archive state.</param>
        /// <param name="incomingX">The x-value of the incoming (i.e. latest) <see cref="DataPoint" />.</param>
        /// <param name="lastArchivedX">The x-value of the last archived <see cref="DataPoint" />.</param>
        /// <returns>
        /// <c>true</c> if <see cref="Compression.MaxDeltaX" /> is the reason for archiving,
        /// <c>false</c> otherwise.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool IsMaxDeltaX(ref (bool Archive, bool MaxDelta) archive, double incomingX, double lastArchivedX)
        {
            if ((incomingX - lastArchivedX) >= _algorithm._maxDeltaX)
            {
                archive.Archive  = true;
                archive.MaxDelta = true;
                return true;
            }
            else
            {
                archive.MaxDelta = false;
                return false;
            }
        }
    }
}
