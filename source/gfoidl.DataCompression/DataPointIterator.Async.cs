using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using gfoidl.DataCompression.Builders;

namespace gfoidl.DataCompression
{
    public abstract partial class DataPointIterator : IAsyncEnumerable<DataPoint>, IAsyncEnumerator<DataPoint>
    {
#pragma warning disable CS1591
        protected IAsyncEnumerable<DataPoint>? _asyncSource;
        protected IAsyncEnumerator<DataPoint>? _asyncEnumerator;
#pragma warning restore CS1591
        //---------------------------------------------------------------------
        /// <summary>
        /// The <see cref="CancellationToken" />.
        /// </summary>
        protected CancellationToken _cancellationToken;
        //---------------------------------------------------------------------
        /// <summary>
        /// Gets an enumerator for the <see cref="DataPoint" />s.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public virtual IAsyncEnumerator<DataPoint> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            if (cancellationToken == default)
                cancellationToken = _cancellationToken;
            else
                _cancellationToken = cancellationToken;

            return this.IterateCore(cancellationToken);
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Advances the enumerator to the next element.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the enumerator was successfully advanced to the next element;
        /// <c>false</c> if the enumerator has passed the end of the collection.
        /// </returns>
        public virtual ValueTask<bool> MoveNextAsync()
        {
            ThrowHelper.ThrowInvalidOperation(ThrowHelper.ExceptionResource.GetEnumerator_must_be_called_first);
            return default;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Returns an array of the compressed <see cref="DataPoint" />s.
        /// </summary>
        /// <returns>An array of the compressed <see cref="DataPoint" />s.</returns>
        public virtual async ValueTask<DataPoint[]> ToArrayAsync()
        {
            Debug.Assert(_asyncSource != null);

            ICollectionBuilder<DataPoint> arrayBuilder = new ArrayBuilder<DataPoint>(true);
            await this.BuildCollectionAsync(arrayBuilder).ConfigureAwait(false);
            return ((ArrayBuilder<DataPoint>)arrayBuilder).ToArray();
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Returns a list of the compressed <see cref="DataPoint" />s.
        /// </summary>
        /// <returns>A list of the compressed <see cref="DataPoint" />s.</returns>
        public virtual async ValueTask<List<DataPoint>> ToListAsync()
        {
            Debug.Assert(_asyncSource != null);

            var listBuilder = new ListBuilder<DataPoint>(true);
            await this.BuildCollectionAsync(listBuilder).ConfigureAwait(false);
            return listBuilder.ToList();
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Resets the enumerator and its state.
        /// </summary>
        public virtual async ValueTask DisposeAsync()
        {
            this.Dispose();

            if (_asyncEnumerator != null)
            {
                await _asyncEnumerator.DisposeAsync().ConfigureAwait(false);
            }
        }
        //---------------------------------------------------------------------
        private async IAsyncEnumerator<DataPoint> IterateCore(CancellationToken cancellationToken)
        {
            Debug.Assert(_asyncSource != null);

            cancellationToken.ThrowIfCancellationRequested();

            bool isFirst = true;
            bool isSkipMinDeltaX = false;

            await foreach (DataPoint incoming in _asyncSource.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (isFirst)
                {
                    isFirst = false;
                    _lastArchived = incoming;
                    yield return incoming;
                    cancellationToken.ThrowIfCancellationRequested();
                    this.Init(incoming, ref _snapShot);
                    continue;
                }

                if (isSkipMinDeltaX)
                {
                    if ((incoming.X - _snapShot.X) <= _algorithm._minDeltaX)
                        continue;

                    isSkipMinDeltaX = false;
                    yield return incoming;
                    cancellationToken.ThrowIfCancellationRequested();
                    this.Init(incoming, ref _snapShot);
                }

                _incoming = incoming;
                this.IsPointToArchive(incoming, _lastArchived);

                if (!_archive.Archive)
                {
                    this.UpdateFilters(incoming, _lastArchived);
                    _snapShot = incoming;
                    continue;
                }

                if (!_archive.MaxDelta && _lastArchived != _snapShot)
                {
                    yield return _snapShot;
                    cancellationToken.ThrowIfCancellationRequested();
                }

                if (_algorithm._minDeltaXHasValue)
                {
                    isSkipMinDeltaX = true;
                    continue;
                }

                yield return incoming;
                cancellationToken.ThrowIfCancellationRequested();
                this.Init(incoming, ref _snapShot);
            }

            cancellationToken.ThrowIfCancellationRequested();
            if (_incoming != _lastArchived)     // sentinel-check
                yield return _incoming;
        }
        //---------------------------------------------------------------------
        private protected virtual async ValueTask BuildCollectionAsync(ICollectionBuilder<DataPoint> builder)
        {
            await foreach (DataPoint dataPoint in this.WithCancellation(_cancellationToken).ConfigureAwait(false))
            {
                _cancellationToken.ThrowIfCancellationRequested();
                builder.Add(dataPoint);
            }
        }
        //---------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.NoInlining)]
        private async ValueTask SkipMinDeltaXAsync(IAsyncEnumerator<DataPoint> asyncEnumerator, double snapShotX)
        {
            double minDeltaX = _algorithm._minDeltaX;

            while (await asyncEnumerator.MoveNextAsync().ConfigureAwait(false))
            {
                DataPoint tmp = asyncEnumerator.Current;

                if ((tmp.X - snapShotX) > minDeltaX)
                {
                    _incoming = tmp;
                    break;
                }
            }
        }
        //---------------------------------------------------------------------
#pragma warning disable CS1591
        IAsyncEnumerator<DataPoint> IAsyncEnumerable<DataPoint>.GetAsyncEnumerator(CancellationToken cancellationToken) => this.GetAsyncEnumerator(cancellationToken);
#pragma warning restore CS1591
    }
}
