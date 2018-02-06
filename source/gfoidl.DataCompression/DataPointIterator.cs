using System;
using System.Collections;
using System.Collections.Generic;

namespace gfoidl.DataCompression
{
    /// <summary>
    /// Base class for an iterator for <see cref="DataPoint" />s.
    /// </summary>
    /// <remarks>
    /// The state at creation is set to <see cref="InitialState" />.
    /// </remarks>
    public abstract class DataPointIterator : IEnumerable<DataPoint>, IEnumerator<DataPoint>
    {
        /// <summary>
        /// The initial state of the iterator.
        /// </summary>
        protected const int InitialState = -2;
        //---------------------------------------------------------------------
        /// <summary>
        /// The state when the iterator is disposed.
        /// </summary>
        protected const int DisposedState = -3;
        //---------------------------------------------------------------------
        private readonly int _threadId;
#pragma warning disable CS1591
        protected int        _state;
        protected DataPoint  _current;
#pragma warning restore CS1591
        //---------------------------------------------------------------------
        /// <summary>
        /// Creates an instance of <see cref="DataPointIterator" />.
        /// </summary>
        protected DataPointIterator()
        {
            _threadId = Environment.CurrentManagedThreadId;
            _state    = InitialState;
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
        private static EmptyIterator _emptyIterator;
        /// <summary>
        /// Returns an <see cref="EmptyIterator" />.
        /// </summary>
        public static DataPointIterator Empty => _emptyIterator ?? (_emptyIterator = new EmptyIterator());
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
        public abstract bool MoveNext();
        //---------------------------------------------------------------------
        /// <summary>
        /// Returns an array of the compressed <see cref="DataPoint" />s.
        /// </summary>
        /// <returns>An array of the compressed <see cref="DataPoint" />s.</returns>
        public abstract DataPoint[] ToArray();
        //---------------------------------------------------------------------
        /// <summary>
        /// Resets the enumerator and its state.
        /// </summary>
        public virtual void Dispose()
        {
            _current = DataPoint.Origin;
            _state   = DisposedState;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// An <see cref="DataPointIterator" /> that represents an empty 
        /// collection.
        /// </summary>
        public class EmptyIterator : DataPointIterator
        {
            /// <summary>
            /// Clones the <see cref="DataPointIterator" />.
            /// </summary>
            /// <returns>The cloned <see cref="DataPointIterator" />.</returns>
            public override DataPointIterator Clone() => this;
            //-----------------------------------------------------------------
            /// <summary>
            /// Advances the enumerator to the next element.
            /// </summary>
            /// <returns>
            /// <c>true</c> if the enumerator was successfully advanced to the next element;
            /// <c>false</c> if the enumerator has passed the end of the collection.
            /// </returns>
            public override bool MoveNext() => false;
            //---------------------------------------------------------------------
            /// <summary>
            /// Returns an array of the compressed <see cref="DataPoint" />s.
            /// </summary>
            /// <returns>An array of the compressed <see cref="DataPoint" />s.</returns>
            public override DataPoint[] ToArray() => Array.Empty<DataPoint>();
        }
    }
}
