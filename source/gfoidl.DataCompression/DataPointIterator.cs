// (c) gfoidl, all rights reserved

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

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
        private protected const int DisposedState        = -3;
        private protected const int InitialState         = -2;
        private protected const int EndOfDataState       = -1;
        private protected const int StartOfDataState     =  0;
        private protected const int IterateState         =  1;
        private protected const int ArchiveIncomingState =  2;
        private protected const int PostArchiveState     =  3;
        private protected const int ArchivePointState    =  4;
        //---------------------------------------------------------------------
        private protected Compression?                  _algorithm;
        private protected int                           _state = InitialState;
        private protected (bool Archive, bool MaxDelta) _archive;

        private protected DataPoint _lastArchived;
        private protected DataPoint _snapShot;
        private protected DataPoint _incoming;

        private int _threadId;

        // Cached here as fields to avoid repeated virtual dispatch for the properties.
        private protected double? _maxDeltaX;
        private protected double? _minDeltaX;
        private protected bool    _archiveIncoming;
        private protected int     _archiveIncomingState;
        //---------------------------------------------------------------------
        /// <summary>
        /// Sets the algorithm for this <see cref="DataPointIterator" />.
        /// </summary>
        protected void SetData(Compression algorithm)
        {
            if (algorithm is null) ThrowHelper.ThrowArgumentNull(ThrowHelper.ExceptionArgument.algorithm);

            _state                = InitialState;
            _threadId             = Environment.CurrentManagedThreadId;
            _algorithm            = algorithm;
            _maxDeltaX            = algorithm.MaxDeltaX;
            _minDeltaX            = algorithm.MinDeltaX;
            _archiveIncoming      = algorithm.ArchiveIncoming;
            _archiveIncomingState = _archiveIncoming ? ArchiveIncomingState : PostArchiveState;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Gets the current item.
        /// </summary>
        public DataPoint Current => _lastArchived;
        //---------------------------------------------------------------------
        /// <summary>
        /// Gets the current item by reference.
        /// </summary>
        public ref DataPoint CurrentByRef => ref _lastArchived;
        //---------------------------------------------------------------------
        private static EmptyDataPointIterator? s_emptyIterator;
        /// <summary>
        /// Returns an <see cref="EmptyDataPointIterator" />.
        /// </summary>
        public static DataPointIterator Empty
        {
            get
            {
                if (Volatile.Read(ref s_emptyIterator) is null)
                {
                    EmptyDataPointIterator iter = new();
                    iter.SetData(NoCompression.s_instance);

                    Interlocked.CompareExchange(ref s_emptyIterator, iter, null);
                }

                return s_emptyIterator!;
            }
        }
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
        public virtual DataPointIterator GetEnumerator()
        {
            DataPointIterator enumerator =
                _state       == InitialState
                && _threadId == Environment.CurrentManagedThreadId
                ? this
                : this.Clone();

            enumerator._state = StartOfDataState;
            return enumerator;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Prepares the algorithm for new data, e.g. opens a new door in the
        /// <see cref="SwingingDoorCompression" />.
        /// </summary>
        /// <param name="incoming">
        /// The <see cref="DataPoint" /> on which the initialisation is based on.
        /// </param>
        protected internal abstract void Init(in DataPoint incoming);
        //---------------------------------------------------------------------
        /// <summary>
        /// Updates the filters.
        /// </summary>
        /// <param name="incoming">The incoming (i.e. latest) <see cref="DataPoint" />.</param>
        /// <param name="lastArchived">The last archived <see cref="DataPoint" />.</param>
        protected internal virtual void UpdateFilters(in DataPoint incoming, in DataPoint lastArchived) { }
        //---------------------------------------------------------------------
        /// <summary>
        /// Determines if the <paramref name="incoming" /> needs to be archived or not.
        /// </summary>
        /// <param name="incoming">The incoming (i.e. latest) <see cref="DataPoint" />.</param>
        /// <param name="lastArchived">The last archived <see cref="DataPoint" />.</param>
        /// <returns>State whether to archive or not plus additional info.</returns>
        protected internal abstract ref (bool Archive, bool MaxDelta) IsPointToArchive(in DataPoint incoming, in DataPoint lastArchived);
        //---------------------------------------------------------------------
        /// <summary>
        /// Determines if the incoming needs to be archived due <see cref="Compression.MaxDeltaX" />.
        /// </summary>
        /// <param name="archive">Archive state.</param>
        /// <param name="lastArchivedX">The x-value of the last archived <see cref="DataPoint" />.</param>
        /// <param name="incomingX">The x-value of the incoming (i.e. latest) <see cref="DataPoint" />.</param>
        /// <returns>
        /// <c>true</c> if <see cref="Compression.MaxDeltaX" /> is the reason for archiving,
        /// <c>false</c> otherwise.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool IsMaxDeltaX(ref (bool Archive, bool MaxDelta) archive, double lastArchivedX, double incomingX)
        {
            if (_maxDeltaX.HasValue && (incomingX - lastArchivedX) >= _maxDeltaX.GetValueOrDefault())
            {
                archive.Archive  = true;
                archive.MaxDelta = true;
                return true;
            }

            archive.MaxDelta = false;
            return false;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Resets the enumerator and its state.
        /// </summary>
        public void Dispose() => this.DisposeCore();
        //---------------------------------------------------------------------
        /// <summary>
        /// Core logic of <see cref="Dispose" />.
        /// </summary>
        protected virtual void DisposeCore()
        {
            _algorithm = null;
            _source    = null;

            if (_enumerator is not null)
            {
                _enumerator.Dispose();
                _enumerator = null;
            }

            _threadId = -1;
            _state    = DisposedState;

            _snapShot     = default;
            _lastArchived = default;
            _incoming     = default;
            _archive      = default;

#if NETSTANDARD2_1
            _asyncSource = null;
#endif
        }
    }
}
