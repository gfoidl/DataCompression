using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

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
        /// <returns>The compressed / filtered data.</returns>
        protected override DataPointAsyncIterator ProcessAsyncCore(IAsyncEnumerable<DataPoint> data, CancellationToken ct)
            => new AsyncEnumerableIterator(this, data, data.GetAsyncEnumerator(ct));
        //---------------------------------------------------------------------
        private sealed class AsyncEnumerableIterator : DataPointAsyncIterator
        {
            private readonly DeadBandCompression _deadBandCompression;
            private readonly IAsyncEnumerable<DataPoint> _source;
            private readonly IAsyncEnumerator<DataPoint> _enumerator;

            private (double Min, double Max) _bounding;
            private (bool Archive, bool MaxDelta) _archive;
            private DataPoint _snapShot;
            private DataPoint _lastArchived;
            private DataPoint _incoming;
            //-----------------------------------------------------------------
            public AsyncEnumerableIterator(
                DeadBandCompression deadBandCompression,
                IAsyncEnumerable<DataPoint> source,
                IAsyncEnumerator<DataPoint> enumerator)
            {
                _deadBandCompression = deadBandCompression;
                _source = source;
                _enumerator = enumerator;
            }
            //-----------------------------------------------------------------
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void GetBounding(in DataPoint dataPoint)
            {
                double y = dataPoint.Y;

                // Produces better code than updating _bounding directly
                ref (double Min, double Max) bounding = ref _bounding;

                bounding.Min = y - _deadBandCompression.InstrumentPrecision;
                bounding.Max = y + _deadBandCompression.InstrumentPrecision;
            }
            //-----------------------------------------------------------------
            public override async ValueTask<bool> MoveNextAsync()
            {
                switch (_state)
                {
                    default:
                        await this.DisposeAsync().ConfigureAwait(false);
                        return false;
                    case -1:
                        await _enumerator.MoveNextAsync().ConfigureAwait(false);
                        _state = 0;
                        return true;
                    case 0:
                        _snapShot = _enumerator.Current;
                        _lastArchived = _snapShot;
                        _incoming = _snapShot;      // sentinel
                        _current = _snapShot;
                        this.GetBounding(_snapShot);
                        _state = 1;
                        return true;
                    case 1:
                        while (await _enumerator.MoveNextAsync().ConfigureAwait(false))
                        {
                            _incoming = _enumerator.Current;
                            this.IsPointToArchive(_incoming);

                            if (!_archive.Archive)
                            {
                                _snapShot = _incoming;
                                continue;
                            }

                            if (!_archive.MaxDelta)
                            {
                                _current = _snapShot;
                                _state = 2;
                                return true;
                            }

                            goto case 2;
                        }

                        _state = -1;
                        if (_incoming != _lastArchived)
                        {
                            _current = _incoming;
                            return true;
                        }
                        return false;
                    case 2:
                        _current = _incoming;
                        _state = 1;
                        this.UpdatePoints(_incoming, ref _snapShot);
                        return true;
                    case InitialState:
                        ThrowHelper.ThrowInvalidOperation(ThrowHelper.ExceptionResource.GetEnumerator_must_be_called_first);
                        return false;
                    case DisposedState:
                        ThrowHelper.ThrowIfDisposed(ThrowHelper.ExceptionArgument.iterator);
                        return false;
                }
            }
            //-----------------------------------------------------------------
            public override ValueTask<DataPoint[]> ToArrayAsync()
            {
                throw new NotImplementedException();
            }
            //-----------------------------------------------------------------
            public override ValueTask<List<DataPoint>> ToListAsync()
            {
                throw new NotImplementedException();
            }
            //-----------------------------------------------------------------
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private ref (bool Archive, bool MaxDelta) IsPointToArchive(in DataPoint incoming)
            {
                ref (bool Archive, bool MaxDelta) archive = ref _archive;

                if ((incoming.X - _lastArchived.X) >= (_deadBandCompression._maxDeltaX))
                {
                    archive.Archive = true;
                    archive.MaxDelta = true;
                }
                else
                {
                    archive.Archive = incoming.Y < _bounding.Min || _bounding.Max < incoming.Y;
                    archive.MaxDelta = false;
                }

                return ref archive;
            }
            //-----------------------------------------------------------------
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void UpdatePoints(in DataPoint incoming, ref DataPoint snapShot)
            {
                _lastArchived = incoming;
                snapShot = incoming;

                if (!_archive.MaxDelta) this.GetBounding(snapShot);
            }
        }
    }
}
