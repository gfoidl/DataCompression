using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using gfoidl.DataCompression.Builders;

namespace gfoidl.DataCompression.Internal.NoCompression
{
    internal sealed class AsyncEnumerableIterator : NoCompressionIterator
    {
        private     readonly IAsyncEnumerable<DataPoint> _enumerable;
        private new readonly IAsyncEnumerator<DataPoint> _enumerator;
        //---------------------------------------------------------------------
        public AsyncEnumerableIterator(Compression compression, IAsyncEnumerable<DataPoint> enumerable, CancellationToken ct)
            : base(compression)
        {
            _enumerable = enumerable;
            _enumerator = enumerable.GetAsyncEnumerator(ct);
        }
        //---------------------------------------------------------------------
        public override async ValueTask<bool> MoveNextAsync()
        {
            if (_state == InitialState)
                ThrowHelper.ThrowInvalidOperation(ThrowHelper.ExceptionResource.GetEnumerator_must_be_called_first);

            if (await _enumerator.MoveNextAsync().ConfigureAwait(false))
            {
                _current = _enumerator.Current;
                return true;
            }

            return false;
        }
        //---------------------------------------------------------------------
        public override async ValueTask<DataPoint[]> ToArrayAsync()
        {
            ICollectionBuilder<DataPoint> arrayBuilder = new ArrayBuilder<DataPoint>(true);
            await this.BuildCollectionAsync(arrayBuilder).ConfigureAwait(false);
            return ((ArrayBuilder<DataPoint>)arrayBuilder).ToArray();
        }
        //---------------------------------------------------------------------
        public override async ValueTask<List<DataPoint>> ToListAsync()
        {
            var listBuilder = new ListBuilder<DataPoint>(true);
            await this.BuildCollectionAsync(listBuilder).ConfigureAwait(false);
            return listBuilder.ToList();
        }
        //---------------------------------------------------------------------
        private async ValueTask BuildCollectionAsync<TBuilder>(TBuilder builder)
            where TBuilder : ICollectionBuilder<DataPoint>
        {
            await foreach (DataPoint dataPoint in _enumerable.WithCancellation(_cancellationToken).ConfigureAwait(false))
            {
                builder.Add(dataPoint);
            }
        }
        //---------------------------------------------------------------------
        public override async ValueTask DisposeAsync()
        {
            await base.DisposeAsync().ConfigureAwait(false);
            await _enumerator.DisposeAsync().ConfigureAwait(false);
        }
        //---------------------------------------------------------------------
        public override DataPointIterator Clone()                                                                                        => throw new NotSupportedException();
        public override bool MoveNext()                                                                                                  => throw new NotSupportedException();
        public override DataPoint[] ToArray()                                                                                            => throw new NotSupportedException();
        public override List<DataPoint> ToList()                                                                                         => throw new NotSupportedException();
        protected internal override void Init(in DataPoint incoming, ref DataPoint snapShot)                                             => throw new NotSupportedException();
        protected internal override void Init(int incomingIndex, in DataPoint incoming, ref int snapShotIndex)                           => throw new NotSupportedException();
        protected internal override ref (bool Archive, bool MaxDelta) IsPointToArchive(in DataPoint incoming, in DataPoint lastArchived) => throw new NotSupportedException();
    }
}
