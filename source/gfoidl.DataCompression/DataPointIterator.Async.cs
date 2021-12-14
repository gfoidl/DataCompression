// (c) gfoidl, all rights reserved

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using gfoidl.DataCompression.Builders;

namespace gfoidl.DataCompression
{
    public abstract partial class DataPointIterator : IAsyncEnumerable<DataPoint>
    {
        private protected IAsyncEnumerable<DataPoint>? _asyncSource;
        //---------------------------------------------------------------------
        /// <summary>
        /// Sets the algorithm for this <see cref="DataPointIterator" />.
        /// </summary>
        protected internal void SetData(Compression algorithm, IAsyncEnumerable<DataPoint> data)
        {
            if (data is null) ThrowHelper.ThrowArgumentNull(ThrowHelper.ExceptionArgument.data);

            this.SetData(algorithm);
            _asyncSource = data;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Gets an enumerator for the <see cref="DataPoint" />s.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public virtual IAsyncEnumerator<DataPoint> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => this.IterateCore(cancellationToken);
        //---------------------------------------------------------------------
        /// <summary>
        /// Returns an array of the compressed <see cref="DataPoint" />s.
        /// </summary>
        /// <returns>An array of the compressed <see cref="DataPoint" />s.</returns>
        public virtual async ValueTask<DataPoint[]> ToArrayAsync(CancellationToken cancellationToken = default)
        {
            Debug.Assert(_asyncSource != null);

            ICollectionBuilder<DataPoint> arrayBuilder = new ArrayBuilder<DataPoint>(true);
            await this.BuildCollectionAsync(arrayBuilder, cancellationToken).ConfigureAwait(false);
            return ((ArrayBuilder<DataPoint>)arrayBuilder).ToArray();
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Returns a list of the compressed <see cref="DataPoint" />s.
        /// </summary>
        /// <returns>A list of the compressed <see cref="DataPoint" />s.</returns>
        public virtual async ValueTask<List<DataPoint>> ToListAsync(CancellationToken cancellationToken = default)
        {
            Debug.Assert(_asyncSource != null);

            var listBuilder = new ListBuilder<DataPoint>(true);
            await this.BuildCollectionAsync(listBuilder, cancellationToken).ConfigureAwait(false);
            return listBuilder.ToList();
        }
        //---------------------------------------------------------------------
        private async IAsyncEnumerator<DataPoint> IterateCore(CancellationToken cancellationToken)
        {
            Debug.Assert(_asyncSource is not null);

            cancellationToken.ThrowIfCancellationRequested();

            bool isFirst         = true;
            bool isSkipMinDeltaX = false;

            await foreach (DataPoint incoming in _asyncSource.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (isFirst)
                {
                    isFirst = false;
                    yield return incoming;
                    cancellationToken.ThrowIfCancellationRequested();

                    _lastArchived = incoming;
                    this.SnapShot = incoming;
                    this.Init(incoming);
                    continue;
                }

                if (isSkipMinDeltaX)
                {
                    if ((incoming.X - this.SnapShot.X) < _minDeltaX)
                        continue;

                    isSkipMinDeltaX = false;
                    continue;
                }

                _incoming = incoming;           // needed for the sentinel-check outside the loop
                this.IsPointToArchive(incoming, _lastArchived);

                if (!_archive.Archive)
                {
                    this.UpdateFilters(incoming, _lastArchived);
                    this.SnapShot = incoming;
                    continue;
                }

                if (!_archive.MaxDelta && _lastArchived != this.SnapShot)
                {
                    yield return this.SnapShot;
                    cancellationToken.ThrowIfCancellationRequested();

                    _lastArchived = this.SnapShot;
                    this.SnapShot = incoming;

                    if (_archiveIncoming)
                    {
                        yield return incoming;
                        cancellationToken.ThrowIfCancellationRequested();

                        _lastArchived = incoming;
                    }

                    this.Init(incoming);
                    this.UpdateFilters(incoming, _lastArchived);
                    isSkipMinDeltaX = _minDeltaX.HasValue;

                    continue;
                }

                yield return incoming;
                cancellationToken.ThrowIfCancellationRequested();

                _lastArchived   = incoming;
                this.SnapShot = incoming;
                isSkipMinDeltaX = _minDeltaX.HasValue;
                this.Init(incoming);
            }

            cancellationToken.ThrowIfCancellationRequested();
            if (_incoming != _lastArchived)     // sentinel-check
            {
                if (_previousSnapShot != _lastArchived)
                {
                    // Construct a door from the last archived point to the
                    // incoming (final point), and check whether the penultimate
                    // point is to archive or not.
                    this.Init(_incoming);
                    this.UpdateFilters(_incoming, _lastArchived);
                    this.IsPointToArchive(_previousSnapShot, _lastArchived);

                    if (_archive.Archive)
                    {
                        yield return _previousSnapShot;
                    }
                }

                yield return _incoming;
            }
        }
        //---------------------------------------------------------------------
        private protected virtual async ValueTask BuildCollectionAsync<TBuilder>(TBuilder builder, CancellationToken cancellationToken)
            where TBuilder : ICollectionBuilder<DataPoint>
        {
            await foreach (DataPoint dataPoint in this.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                builder.Add(dataPoint);
            }
        }
    }
}
