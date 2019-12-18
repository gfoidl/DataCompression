using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using gfoidl.DataCompression.Builders;

namespace gfoidl.DataCompression
{
    /// <summary>
    /// Dead band compression.
    /// </summary>
    /// <remarks>
    /// See documentation for further information.
    /// </remarks>
    public partial class DeadBandCompression : Compression
    {
        /// <summary>
        /// Implementation of the compression / filtering.
        /// </summary>
        /// <param name="data">Input data</param>
        /// <param name="ct">The token for cancellation.</param>
        /// <returns>The compressed / filtered data.</returns>
        protected override DataPointIterator ProcessAsyncCore(IAsyncEnumerable<DataPoint> data, CancellationToken ct)
            => new AsyncEnumerableIterator(this, data, cancellationToken: ct);
        //---------------------------------------------------------------------
        private sealed class AsyncEnumerableIterator : DeadBandCompressionEnumerableIterator
        {
            private readonly IAsyncEnumerable<DataPoint> _source;
            private readonly IAsyncEnumerator<DataPoint> _enumerator;
            //-----------------------------------------------------------------
            public AsyncEnumerableIterator(
                DeadBandCompression deadBandCompression,
                IAsyncEnumerable<DataPoint> source,
                IAsyncEnumerator<DataPoint>? enumerator = null,
                CancellationToken cancellationToken     = default)
                : base(deadBandCompression)
            {
                if (cancellationToken != default) _cancellationToken = cancellationToken;
                _source     = source;
                _enumerator = enumerator ?? source.GetAsyncEnumerator(_cancellationToken);
            }
            //-----------------------------------------------------------------
            public override async ValueTask<bool> MoveNextAsync()
            {
                _cancellationToken.ThrowIfCancellationRequested();

                switch (_state)
                {
                    case 0:
                        if (!(await _enumerator.MoveNextAsync().ConfigureAwait(false))) return false;
                        _snapShot     = _enumerator.Current;
                        _lastArchived = _snapShot;
                        _incoming     = _snapShot;          // sentinel, nullable would be possible but to much work around
                        _current      = _snapShot;
                        this.GetBounding(_snapShot);
                        _state        = 1;
                        return true;
                    case 1:
                        while (await _enumerator.MoveNextAsync().ConfigureAwait(false))
                        {
                            _incoming               = _enumerator.Current;
                            var (archive, maxDelta) = this.IsPointToArchive(_incoming);

                            if (!archive)
                            {
                                _snapShot = _incoming;
                                continue;
                            }

                            if (!maxDelta && _lastArchived != _snapShot)
                            {
                                _current = _snapShot;
                                _state   = 2;
                                return true;
                            }

                            goto case 2;
                        }

                        _state = -1;
                        if (_incoming != _lastArchived)     // sentinel-check
                        {
                            _current = _incoming;
                            return true;
                        }
                        return false;
                    case 2:
                        _current = _incoming;
                        _state   = 1;
                        this.UpdatePoints(_incoming, ref _snapShot);
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
            //-----------------------------------------------------------------
            public override async ValueTask<DataPoint[]> ToArrayAsync()
            {
                ICollectionBuilder<DataPoint> arrayBuilder = new ArrayBuilder<DataPoint>(true);
                IAsyncEnumerator<DataPoint> enumerator     = _source.GetAsyncEnumerator(_cancellationToken);
                await this.BuildCollectionAsync(enumerator, arrayBuilder).ConfigureAwait(false);

                return ((ArrayBuilder<DataPoint>)arrayBuilder).ToArray();
            }
            //-----------------------------------------------------------------
            public override async ValueTask<List<DataPoint>> ToListAsync()
            {
                ICollectionBuilder<DataPoint> listBuilder = new ListBuilder<DataPoint>(true);
                IAsyncEnumerator<DataPoint> enumerator    = _source.GetAsyncEnumerator(_cancellationToken);
                await this.BuildCollectionAsync(enumerator, listBuilder).ConfigureAwait(false);

                return ((ListBuilder<DataPoint>)listBuilder).ToList();
            }
            //-----------------------------------------------------------------
            public async ValueTask BuildCollectionAsync(IAsyncEnumerator<DataPoint> enumerator, ICollectionBuilder<DataPoint> builder)
            {
                if (!(await enumerator.MoveNextAsync())) return;

                DataPoint snapShot = enumerator.Current;
                _lastArchived      = snapShot;
                DataPoint incoming = snapShot;          // sentinel, nullable would be possible but to much work around

                builder.Add(snapShot);
                this.GetBounding(snapShot);

                while (await enumerator.MoveNextAsync().ConfigureAwait(false))
                {
                    _cancellationToken.ThrowIfCancellationRequested();

                    incoming                = enumerator.Current;
                    var (archive, maxDelta) = this.IsPointToArchive(incoming);

                    if (!archive)
                    {
                        snapShot = incoming;
                        continue;
                    }

                    if (!maxDelta && _lastArchived != snapShot)
                        builder.Add(snapShot);

                    builder.Add(incoming);
                    this.UpdatePoints(incoming, ref snapShot);
                }

                if (incoming != _lastArchived)          // sentinel-check
                    builder.Add(incoming);
            }
            //---------------------------------------------------------------------
            public override async ValueTask DisposeAsync()
            {
                await base.DisposeAsync().ConfigureAwait(false);
                await _enumerator.DisposeAsync().ConfigureAwait(false);
            }
            //---------------------------------------------------------------------
            public override DataPointIterator Clone() => throw new NotSupportedException();
            public override bool MoveNext()           => throw new NotSupportedException();
            public override DataPoint[] ToArray()     => throw new NotSupportedException();
            public override List<DataPoint> ToList()  => throw new NotSupportedException();
        }
    }
}
