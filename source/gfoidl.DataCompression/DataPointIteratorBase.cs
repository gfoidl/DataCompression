namespace gfoidl.DataCompression
{
    /// <summary>
    /// Base class for an iterator for <see cref="DataPoint" />s.
    /// </summary>
    public abstract class DataPointIteratorBase
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
#pragma warning disable CS1591
        protected int       _state = InitialState;
        protected DataPoint _current;
#pragma warning restore CS1591
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
    }
}
