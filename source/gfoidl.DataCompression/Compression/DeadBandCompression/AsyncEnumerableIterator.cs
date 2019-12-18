﻿using System;
using System.Collections.Generic;
using System.Threading;

namespace gfoidl.DataCompression.Internal.DeadBand
{
    internal sealed class AsyncEnumerableIterator : DeadBandCompressionIterator
    {
        public AsyncEnumerableIterator(
            DeadBandCompression          deadBandCompression,
            IAsyncEnumerable<DataPoint>  source,
            IAsyncEnumerator<DataPoint>? enumerator        = null,
            CancellationToken            cancellationToken = default)
            : base(deadBandCompression)
        {
            if (cancellationToken != default) _cancellationToken = cancellationToken;
            _asyncSource     = source;
            _asyncEnumerator = enumerator ?? source.GetAsyncEnumerator(_cancellationToken);
        }
        //---------------------------------------------------------------------
        public override DataPointIterator Clone() => throw new NotSupportedException();
        public override bool MoveNext()           => throw new NotSupportedException();
        public override DataPoint[] ToArray()     => throw new NotSupportedException();
        public override List<DataPoint> ToList()  => throw new NotSupportedException();
    }
}
