using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using gfoidl.DataCompression.Builders;

namespace gfoidl.DataCompression
{
    /// <summary>
    /// Swinging door compression.
    /// </summary>
    /// <remarks>
    /// See documentation for further information.
    /// </remarks>
    public partial class SwingingDoorCompression : Compression
    {
        /// <summary>
        /// Implementation of the compression / filtering.
        /// </summary>
        /// <param name="data">Input data</param>
        /// <returns>The compressed / filtered data.</returns>
        protected override DataPointAsyncIterator ProcessAsyncCore(IAsyncEnumerable<DataPoint> data, CancellationToken ct)
        => new AsyncEnumerableIterator(this, data, cancellationToken: ct);
        //---------------------------------------------------------------------
        private sealed class AsyncEnumerableIterator : DataPointAsyncIterator
        {
            private readonly SwingingDoorCompression     _swingingDoorCompression;
            private readonly IAsyncEnumerable<DataPoint> _source;
            private readonly IAsyncEnumerator<DataPoint> _enumerator;

            private static readonly (double Max, double Min) s_newDoor = (double.PositiveInfinity, double.NegativeInfinity);
            private (double Max, double Min)      _slope;
            private (bool Archive, bool MaxDelta) _archive;
            private DataPoint                     _snapShot;
            private DataPoint                     _lastArchived;
            private DataPoint                     _incoming;
            //-----------------------------------------------------------------
            public AsyncEnumerableIterator(
                SwingingDoorCompression swingingDoorCompression,
                IAsyncEnumerable<DataPoint> source,
                IAsyncEnumerator<DataPoint>? enumerator = null,
                CancellationToken cancellationToken = default)
            {
                if (cancellationToken != default) _cancellationToken = cancellationToken;
                _swingingDoorCompression = swingingDoorCompression;
                _source                  = source;
                _enumerator              = enumerator ?? source.GetAsyncEnumerator(_cancellationToken);
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
                        this.OpenNewDoor(_incoming);
                        _state        = 1;
                        return true;
                    case 1:
                        while (await _enumerator.MoveNextAsync().ConfigureAwait(false))
                        {
                            _incoming = _enumerator.Current;
                            this.IsPointToArchive(_incoming, _lastArchived);

                            if (!_archive.Archive)
                            {
                                this.CloseTheDoor(_incoming, _lastArchived);
                                _snapShot = _incoming;
                                continue;
                            }

                            if (!_archive.MaxDelta && _lastArchived != _snapShot)
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
                        if (_swingingDoorCompression._minDeltaXHasValue)
                            await this.SkipMinDeltaXAsync(_snapShot.X).ConfigureAwait(false);

                        _current = _incoming;
                        _state   = 1;
                        this.OpenNewDoor(_incoming);
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
                if (!(await enumerator.MoveNextAsync().ConfigureAwait(false))) return;

                DataPoint snapShot = enumerator.Current;
                _lastArchived      = snapShot;
                DataPoint incoming = snapShot;          // sentinel, nullable would be possible but to much work around

                builder.Add(snapShot);
                this.OpenNewDoor(snapShot);

                while (await enumerator.MoveNextAsync().ConfigureAwait(false))
                {
                    _cancellationToken.ThrowIfCancellationRequested();

                    incoming = enumerator.Current;
                    this.IsPointToArchive(incoming, _lastArchived);

                    if (!_archive.Archive)
                    {
                        this.CloseTheDoor(incoming, _lastArchived);
                        snapShot = incoming;
                        continue;
                    }

                    if (!_archive.MaxDelta && _lastArchived != snapShot)
                        builder.Add(snapShot);

                    if (_swingingDoorCompression._minDeltaXHasValue)
                    {
                        await this.SkipMinDeltaXAsync(snapShot.X).ConfigureAwait(false);
                        incoming = _incoming;
                    }

                    builder.Add(incoming);
                    this.OpenNewDoor(incoming);
                }

                if (incoming != _lastArchived)          // sentinel-check
                    builder.Add(incoming);
            }
            //-----------------------------------------------------------------
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void IsPointToArchive(in DataPoint incoming, in DataPoint lastArchived)
            {
                if ((incoming.X - lastArchived.X) >= (_swingingDoorCompression._maxDeltaX))
                {
                    _archive.Archive  = true;
                    _archive.MaxDelta = true;
                }
                else
                {
                    // Better to compare via gradient (1 calculation) than comparing to allowed y-values (2 calcuations)
                    // Obviously, the result should be the same ;-)
                    double slopeToIncoming = lastArchived.Gradient(incoming);

                    _archive.Archive  = slopeToIncoming < _slope.Min || _slope.Max < slopeToIncoming;
                    _archive.MaxDelta = false;
                }
            }
            //-----------------------------------------------------------------
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void CloseTheDoor(in DataPoint incoming, in DataPoint lastArchived)
            {
                double upperSlope = lastArchived.Gradient(incoming,  _swingingDoorCompression.CompressionDeviation);
                double lowerSlope = lastArchived.Gradient(incoming, -_swingingDoorCompression.CompressionDeviation);

                if (upperSlope < _slope.Max) _slope.Max = upperSlope;
                if (lowerSlope > _slope.Min) _slope.Min = lowerSlope;
            }
            //-----------------------------------------------------------------
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void OpenNewDoor(in DataPoint incoming)
            {
                _lastArchived = incoming;
                _slope        = s_newDoor;
            }
            //---------------------------------------------------------------------
            [MethodImpl(MethodImplOptions.NoInlining)]
            private async ValueTask SkipMinDeltaXAsync(double snapShotX)
            {
                double minDeltaX = _swingingDoorCompression._minDeltaX;

                while (await _enumerator.MoveNextAsync().ConfigureAwait(false))
                {
                    _cancellationToken.ThrowIfCancellationRequested();

                    DataPoint tmp = _enumerator.Current;

                    if ((tmp.X - snapShotX) > minDeltaX)
                    {
                        _incoming = tmp;
                        break;
                    }
                }
            }
        }
    }
}
