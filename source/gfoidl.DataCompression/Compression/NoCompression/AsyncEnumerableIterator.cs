using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using gfoidl.DataCompression.Builders;

namespace gfoidl.DataCompression.Internal.NoCompression
{
    internal sealed class AsyncEnumerableIterator : NoCompressionIterator
    {
        public AsyncEnumerableIterator(Compression compression, IAsyncEnumerable<DataPoint> enumerable, CancellationToken ct)
            : base(compression)
        {
            _asyncSource = enumerable;

            if (ct != default)
                _cancellationToken = ct;
        }
        //---------------------------------------------------------------------
        public override IAsyncEnumerator<DataPoint> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            if (cancellationToken == default)
                cancellationToken = _cancellationToken;
            else
                _cancellationToken = cancellationToken;

            return this.IterateCore(cancellationToken);
        }
        //---------------------------------------------------------------------
        public override ValueTask<bool> MoveNextAsync()
        {
            ThrowHelper.ThrowInvalidOperation(ThrowHelper.ExceptionResource.GetEnumerator_must_be_called_first);
            return default;
        }
        //---------------------------------------------------------------------
        private async IAsyncEnumerator<DataPoint> IterateCore(CancellationToken cancellationToken)
        {
            Debug.Assert(_asyncSource != null);

            cancellationToken.ThrowIfCancellationRequested();

            await foreach (DataPoint dataPoint in _asyncSource.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return dataPoint;
            }
        }
        //---------------------------------------------------------------------
        private protected override async ValueTask BuildCollectionAsync(ICollectionBuilder<DataPoint> builder)
        {
            await foreach (DataPoint dataPoint in _asyncSource.WithCancellation(_cancellationToken).ConfigureAwait(false))
            {
                _cancellationToken.ThrowIfCancellationRequested();
                builder.Add(dataPoint);
            }
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
