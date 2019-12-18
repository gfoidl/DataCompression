﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace gfoidl.DataCompression.Internal.DeadBand
{
    internal sealed class SequentialEnumerableIterator : EnumerableIterator
    {
        public SequentialEnumerableIterator(
            DeadBandCompression     deadBandCompression,
            IEnumerable<DataPoint>? source,
            IEnumerator<DataPoint>? enumerator = null)
            : base(deadBandCompression)
        {
            _source     = source ?? throw new ArgumentNullException(nameof(source));
            _enumerator = enumerator ?? source.GetEnumerator();
        }
        //-----------------------------------------------------------------
        public override DataPointIterator Clone()                                   => new SequentialEnumerableIterator(_deadBandCompression, _source, _enumerator);
        protected override void Init(in DataPoint incoming, ref DataPoint snapShot) => this.UpdatePoints(incoming, ref snapShot);
        //-----------------------------------------------------------------
#if NETSTANDARD2_1
        public override ValueTask<bool> MoveNextAsync()          => throw new NotSupportedException();
        public override ValueTask<DataPoint[]> ToArrayAsync()    => throw new NotSupportedException();
        public override ValueTask<List<DataPoint>> ToListAsync() => throw new NotSupportedException();
#endif
    }
}