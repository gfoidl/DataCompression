using System.Collections.Generic;
using gfoidl.DataCompression.Builders;

namespace gfoidl.DataCompression
{
    public partial class NoCompression
    {
        private sealed class EnumerableIterator : DataPointIterator
        {
            private readonly IEnumerable<DataPoint> _enumerable;
            private readonly IEnumerator<DataPoint> _enumerator;
            //-----------------------------------------------------------------
            public EnumerableIterator(IEnumerable<DataPoint> enumerable)
            {
                _enumerable = enumerable;
                _enumerator = enumerable.GetEnumerator();
            }
            //-----------------------------------------------------------------
            public override DataPointIterator Clone() => new EnumerableIterator(_enumerable);
            //-----------------------------------------------------------------
            public override bool MoveNext()
            {
                if (_state == InitialState)
                    ThrowHelper.ThrowInvalidOperation(ThrowHelper.ExceptionResource.GetEnumerator_must_be_called_first);

                if (_enumerator.MoveNext())
                {
                    _current = _enumerator.Current;
                    return true;
                }

                return false;
            }
            //-----------------------------------------------------------------
            public override DataPoint[] ToArray()
            {
                var arrayBuilder = new ArrayBuilder<DataPoint>(true);
                arrayBuilder.AddRange(_enumerable);

                return arrayBuilder.ToArray();
            }
            //---------------------------------------------------------------------
            public override List<DataPoint> ToList() => new List<DataPoint>(_enumerable);
            //---------------------------------------------------------------------
            public override void Dispose()
            {
                base.Dispose();
                _enumerator.Dispose();
            }
        }
    }
}
