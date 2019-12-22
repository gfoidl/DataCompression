﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using gfoidl.DataCompression.Builders;

namespace gfoidl.DataCompression.Internal.NoCompression
{
    internal sealed class AsyncEnumerableIterator : NoCompressionIterator
    {
        public AsyncEnumerableIterator(Compression compression, IAsyncEnumerable<DataPoint> enumerable)
            : base(compression)
            => _asyncSource = enumerable;
        //---------------------------------------------------------------------
        public override IAsyncEnumerator<DataPoint> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => this.IterateCore(cancellationToken);
        //---------------------------------------------------------------------
        private async IAsyncEnumerator<DataPoint> IterateCore(CancellationToken cancellationToken)
        {
            Debug.Assert(_asyncSource != null);

            await foreach (DataPoint dataPoint in _asyncSource.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                yield return dataPoint;
            }
        }
        //---------------------------------------------------------------------
        private protected override async ValueTask BuildCollectionAsync<TBuilder>(TBuilder builder, CancellationToken cancellationToken)
        {
            await foreach (DataPoint dataPoint in _asyncSource.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
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
