using System;
using System.Collections.Generic;

namespace gfoidl.DataCompression
{
    /// <summary>
    /// Base class for an iterator for <see cref="DataPoint" />s.
    /// </summary>
    /// <remarks>
    /// The state at creation is set to <see cref="DataPointIteratorBase.InitialState" />.
    /// </remarks>
    public abstract partial class DataPointIterator
    {
        /// <summary>
        /// An <see cref="DataPointIterator" /> that represents an empty 
        /// collection.
        /// </summary>
        private sealed partial class EmptyIterator : DataPointIterator
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
            //---------------------------------------------------------------------
            /// <summary>
            /// Returns a list of the compressed <see cref="DataPoint" />s.
            /// </summary>
            /// <returns>A list of the compressed <see cref="DataPoint" />s.</returns>
            public override List<DataPoint> ToList() => new List<DataPoint>();
        }
    }
}
