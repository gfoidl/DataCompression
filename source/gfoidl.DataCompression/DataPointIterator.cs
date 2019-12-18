using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using gfoidl.DataCompression.Builders;

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
        protected IEnumerable<DataPoint>? _source;
        protected IEnumerator<DataPoint>? _enumerator;
        protected int                     _state = InitialState;
        protected DataPoint               _current;
        protected DataPoint               _snapShot;
        protected DataPoint               _lastArchived;
        protected DataPoint               _incoming;
#pragma warning restore CS1591
        private readonly int _threadId;
        //---------------------------------------------------------------------
        /// <summary>
        /// Creates an instance of <see cref="DataPointIterator" />.
        /// </summary>
        protected DataPointIterator()
        {
            _threadId = Environment.CurrentManagedThreadId;
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
        private static EmptyIterator? s_emptyIterator;
        /// <summary>
        /// Returns an <see cref="EmptyIterator" />.
        /// </summary>
        public static DataPointIterator Empty => s_emptyIterator ?? (s_emptyIterator = new EmptyIterator());
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
        /// Advances the enumerator to the next element.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the enumerator was successfully advanced to the next element;
        /// <c>false</c> if the enumerator has passed the end of the collection.
        /// </returns>
        public virtual bool MoveNext()
        {
            Debug.Assert(_enumerator != null);

            switch (_state)
            {
                case 0:
                    if (!_enumerator.MoveNext()) return false;
                    _incoming     = _enumerator.Current;
                    _lastArchived = _snapShot;
                    _current      = _incoming;
                    this.Init(_incoming, ref _snapShot);
                    _state        = 1;
                    return true;
                case 1:
                    while (_enumerator.MoveNext())
                    {
                        _incoming = _enumerator.Current;
                        ref var archive = ref this.IsPointToArchive(_incoming, _lastArchived);

                        if (!archive.Archive)
                        {
                            this.UpdateFilters(_incoming, _lastArchived);
                            _snapShot = _incoming;
                            continue;
                        }

                        if (!archive.MaxDelta && _lastArchived != _snapShot)
                        {
                            _current = _snapShot;
                            _state = 2;
                            return true;
                        }

                        goto case 2;
                    }

                    _state = -1;
                    if (_incoming != _lastArchived)     // sentinel check
                    {
                        _current = _incoming;
                        return true;
                    }
                    return false;
                case 2:
                    _current = this.HandleSpecialCaseAfterArchivedPoint(_enumerator, ref _incoming, _snapShot);
                    _state   = 1;
                    this.Init(_incoming, ref _snapShot);
                    return true;
                case InitialState:
                    ThrowHelper.ThrowInvalidOperation(ThrowHelper.ExceptionResource.GetEnumerator_must_be_called_first);
                    return false;
                case DisposedState:
                    ThrowHelper.ThrowIfDisposed(ThrowHelper.ExceptionArgument.iterator);
                    return false;
                default:
                    this.Dispose();
                    return false;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Returns an array of the compressed <see cref="DataPoint" />s.
        /// </summary>
        /// <returns>An array of the compressed <see cref="DataPoint" />s.</returns>
        public virtual DataPoint[] ToArray()
        {
            Debug.Assert(_source != null);

            IEnumerator<DataPoint> enumerator = _source.GetEnumerator();
            var arrayBuilder                  = new ArrayBuilder<DataPoint>(true);
            this.BuildCollection(enumerator, ref arrayBuilder);

            return arrayBuilder.ToArray();
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Returns a list of the compressed <see cref="DataPoint" />s.
        /// </summary>
        /// <returns>A list of the compressed <see cref="DataPoint" />s.</returns>
        public virtual List<DataPoint> ToList()
        {
            Debug.Assert(_source != null);

            IEnumerator<DataPoint> enumerator = _source.GetEnumerator();
            var listBuilder                   = new ListBuilder<DataPoint>(true);
            this.BuildCollection(enumerator, ref listBuilder);

            return listBuilder.ToList();
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
        protected abstract void Init(in DataPoint incoming, ref DataPoint snapShot);
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
        /// Updates the filters.
        /// </summary>
        /// <param name="incoming">The incoming (i.e. latest) <see cref="DataPoint" />.</param>
        /// <param name="lastArchived">The last archived <see cref="DataPoint" />.</param>
        protected virtual void UpdateFilters(in DataPoint incoming, in DataPoint lastArchived) { }
        //---------------------------------------------------------------------
        /// <summary>
        /// Handles special cases after a <see cref="DataPoint" /> got archived.
        /// </summary>
        /// <param name="enumerator">The enumerator.</param>
        /// <param name="incoming">The incoming datapoint.</param>
        /// <param name="snapShot">The snapshot datapoint.</param>
        /// <returns>New incoming datapoint.</returns>
        protected virtual ref DataPoint HandleSpecialCaseAfterArchivedPoint(IEnumerator<DataPoint> enumerator, ref DataPoint incoming, in DataPoint snapShot)
        {
            return ref incoming;
        }
        //---------------------------------------------------------------------
        private void BuildCollection<TBuilder>(IEnumerator<DataPoint> enumerator, ref TBuilder builder)
            where TBuilder : ICollectionBuilder<DataPoint>
        {
            if (!enumerator.MoveNext()) return;

            DataPoint incoming = enumerator.Current;
            _lastArchived      = incoming;
            DataPoint snapShot = default;

            builder.Add(incoming);
            this.Init(incoming, ref snapShot);

            while (enumerator.MoveNext())
            {
                incoming        = enumerator.Current;
                ref var archive = ref this.IsPointToArchive(incoming, _lastArchived);

                if (!archive.Archive)
                {
                    this.UpdateFilters(incoming, _lastArchived);
                    snapShot = incoming;
                    continue;
                }

                if (!archive.MaxDelta && _lastArchived != snapShot)
                {
                    builder.Add(snapShot);
                }

                incoming = this.HandleSpecialCaseAfterArchivedPoint(enumerator, ref incoming, snapShot);

                builder.Add(incoming);
                this.Init(incoming, ref snapShot);
            }

            if (incoming != _lastArchived)          // sentinel-check
                builder.Add(incoming);
        }
    }
}
