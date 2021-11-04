// (c) gfoidl, all rights reserved

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using gfoidl.DataCompression.Builders;

namespace gfoidl.DataCompression.Internal.NoCompression
{
    internal sealed class EnumerableIterator : NoCompressionIterator
    {
        public override DataPointIterator Clone()
        {
            Debug.Assert(_algorithm is not null);
            Debug.Assert(_source    is not null);

            EnumerableIterator clone = new();
            clone.SetData(_algorithm, _source);

            return clone;
        }
        //---------------------------------------------------------------------
        public override bool MoveNext()
        {
            if (_state == InitialState || _enumerator == null)
                ThrowHelper.ThrowInvalidOperation(ThrowHelper.ExceptionResource.GetEnumerator_must_be_called_first);

            if (_enumerator.MoveNext())
            {
                _lastArchived = _enumerator.Current;
                return true;
            }

            return false;
        }
        //---------------------------------------------------------------------
        public override DataPoint[] ToArray()
        {
            Debug.Assert(_source != null);

            var arrayBuilder = new ArrayBuilder<DataPoint>(true);
            arrayBuilder.AddRange(_source);

            return arrayBuilder.ToArray();
        }
        //---------------------------------------------------------------------
        public override List<DataPoint> ToList() => new List<DataPoint>(_source);
        //---------------------------------------------------------------------
        protected internal override void Init(in DataPoint incoming, ref DataPoint snapShot)                                             => throw new NotSupportedException();
        protected internal override void Init(int incomingIndex, in DataPoint incoming, ref int snapShotIndex)                           => throw new NotSupportedException();
        protected internal override ref (bool Archive, bool MaxDelta) IsPointToArchive(in DataPoint incoming, in DataPoint lastArchived) => throw new NotSupportedException();
        //---------------------------------------------------------------------
#if NETSTANDARD2_1
        public override ValueTask<DataPoint[]> ToArrayAsync(CancellationToken ct)    => throw new NotSupportedException();
        public override ValueTask<List<DataPoint>> ToListAsync(CancellationToken ct) => throw new NotSupportedException();
#endif
    }
}
