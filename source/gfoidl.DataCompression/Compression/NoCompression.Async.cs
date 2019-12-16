﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using gfoidl.DataCompression.Builders;

namespace gfoidl.DataCompression
{
    public partial class NoCompression
    {
        private sealed class AsyncEnumerableIterator : DataPointIterator
        {
            private readonly IAsyncEnumerable<DataPoint> _enumerable;
            private readonly IAsyncEnumerator<DataPoint> _enumerator;
            //-----------------------------------------------------------------
            public AsyncEnumerableIterator(IAsyncEnumerable<DataPoint> enumerable, CancellationToken ct)
            {
                _enumerable = enumerable;
                _enumerator = enumerable.GetAsyncEnumerator(ct);
            }
            //-----------------------------------------------------------------
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
            //-----------------------------------------------------------------
            public override async ValueTask<DataPoint[]> ToArrayAsync()
            {
                ICollectionBuilder<DataPoint> arrayBuilder = new ArrayBuilder<DataPoint>(true);
                await this.BuildCollectionAsync(arrayBuilder).ConfigureAwait(false);
                return ((ArrayBuilder<DataPoint>)arrayBuilder).ToArray();
            }
            //-----------------------------------------------------------------
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
            //-----------------------------------------------------------------
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
