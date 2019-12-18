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
        public virtual async ValueTask<bool> MoveNextAsync()
        {
            _cancellationToken.ThrowIfCancellationRequested();
            Debug.Assert(_asyncEnumerator != null);

            switch (_state)
            {
                case 0:
                    if (!await _asyncEnumerator.MoveNextAsync().ConfigureAwait(false)) return false;
                    _incoming     = _asyncEnumerator.Current;
                    _lastArchived = _incoming;
                    _current      = _incoming;
                    this.Init(_incoming, ref _snapShot);
                    _state        = 1;
                    return true;
                case 1:
                    while (await _asyncEnumerator.MoveNextAsync().ConfigureAwait(false))
                    {
                        _incoming = _asyncEnumerator.Current;
                        this.IsPointToArchive(_incoming, _lastArchived);

                        if (!_archive.Archive)
                        {
                            this.UpdateFilters(_incoming, _lastArchived);
                            _snapShot = _incoming;
                            continue;
                        }

                        if (!_archive.MaxDelta && _lastArchived!=_snapShot)
                        {
                            _current = _snapShot;
                            _state = 2;
                            return true;
                        }

                        goto case 2;
                    }

                    _state = -1;
                    if (_incoming != _lastArchived)       // sentinel check
                    {
                        _current = _incoming;
                        return true;
                    }
                    return false;
                case 2:
                    if (_algorithm._minDeltaXHasValue)
                        await this.SkipMinDeltaXAsync(_snapShot.X).ConfigureAwait(false);

                    _current = _incoming;
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
                    await this.DisposeAsync().ConfigureAwait(false);
                    return false;
            }
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
            IAsyncEnumerator<DataPoint> enumerator     = _asyncSource.GetAsyncEnumerator(_cancellationToken);
            await this.BuildCollectionAsync(enumerator, arrayBuilder).ConfigureAwait(false);

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

            ICollectionBuilder<DataPoint> listBuilder = new ListBuilder<DataPoint>(true);
            IAsyncEnumerator<DataPoint> enumerator    = _asyncSource.GetAsyncEnumerator(_cancellationToken);
            await this.BuildCollectionAsync(enumerator, listBuilder).ConfigureAwait(false);

            return ((ListBuilder<DataPoint>)listBuilder).ToList();
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
        internal async ValueTask BuildCollectionAsync(IAsyncEnumerator<DataPoint> enumerator, ICollectionBuilder<DataPoint> builder)
        {
            if (!(await enumerator.MoveNextAsync().ConfigureAwait(false))) return;

            DataPoint incoming = enumerator.Current;
            _lastArchived      = incoming;
            DataPoint snapShot = default;

            builder.Add(incoming);
            this.Init(incoming, ref snapShot);

            while (await enumerator.MoveNextAsync().ConfigureAwait(false))
            {
                _cancellationToken.ThrowIfCancellationRequested();

                incoming = enumerator.Current;
                this.IsPointToArchive(incoming, _lastArchived);

                if (!_archive.Archive)
                {
                    this.UpdateFilters(incoming, _lastArchived);
                    snapShot = incoming;
                    continue;
                }

                if (!_archive.MaxDelta && _lastArchived != snapShot)
                {
                    builder.Add(snapShot);
                }

                if (_algorithm._minDeltaXHasValue)
                {
                    await this.SkipMinDeltaXAsync(snapShot.X).ConfigureAwait(false);
                    incoming = _incoming;
                }

                builder.Add(incoming);
                this.Init(incoming, ref snapShot);
            }

            if (incoming != _lastArchived)          // sentinel-check
                builder.Add(incoming);
        }
        //---------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.NoInlining)]
        private async ValueTask SkipMinDeltaXAsync(double snapShotX)
        {
            Debug.Assert(_asyncEnumerator != null);
            double minDeltaX = _algorithm._minDeltaX;

            while (await _asyncEnumerator.MoveNextAsync().ConfigureAwait(false))
            {
                DataPoint tmp = _asyncEnumerator.Current;

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
