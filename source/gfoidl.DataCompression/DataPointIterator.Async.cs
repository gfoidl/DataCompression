﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace gfoidl.DataCompression
{
    public abstract partial class DataPointIterator : IAsyncEnumerable<DataPoint>, IAsyncEnumerator<DataPoint>
    {
        /// <summary>
        /// The <see cref="CancellationToken" />.
        /// </summary>
        protected CancellationToken _cancellationToken;
        //---------------------------------------------------------------------
        /// <summary>
        /// Gets an enumerator for the <see cref="DataPoint" />s.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public DataPointIterator GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            if (cancellationToken != default)
                _cancellationToken = cancellationToken;

            _state = 0;
            return this;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Advances the enumerator to the next element.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the enumerator was successfully advanced to the next element;
        /// <c>false</c> if the enumerator has passed the end of the collection.
        /// </returns>
        public abstract ValueTask<bool> MoveNextAsync();
        //---------------------------------------------------------------------
        /// <summary>
        /// Returns an array of the compressed <see cref="DataPoint" />s.
        /// </summary>
        /// <returns>An array of the compressed <see cref="DataPoint" />s.</returns>
        public abstract ValueTask<DataPoint[]> ToArrayAsync();
        //---------------------------------------------------------------------
        /// <summary>
        /// Returns a list of the compressed <see cref="DataPoint" />s.
        /// </summary>
        /// <returns>A list of the compressed <see cref="DataPoint" />s.</returns>
        public abstract ValueTask<List<DataPoint>> ToListAsync();
        //---------------------------------------------------------------------
        /// <summary>
        /// Resets the enumerator and its state.
        /// </summary>
        public virtual ValueTask DisposeAsync()
        {
            this.Dispose();
            return default;
        }
        //---------------------------------------------------------------------
#pragma warning disable CS1591
        IAsyncEnumerator<DataPoint> IAsyncEnumerable<DataPoint>.GetAsyncEnumerator(CancellationToken cancellationToken) => this.GetAsyncEnumerator(cancellationToken);
#pragma warning restore CS1591
        //---------------------------------------------------------------------
        private sealed partial class EmptyIterator
        {
            public override ValueTask<bool> MoveNextAsync()          => new ValueTask<bool>(false);
            public override ValueTask<DataPoint[]> ToArrayAsync()    => new ValueTask<DataPoint[]>(Array.Empty<DataPoint>());
            public override ValueTask<List<DataPoint>> ToListAsync() => new ValueTask<List<DataPoint>>(new List<DataPoint>());
        }
    }
}
