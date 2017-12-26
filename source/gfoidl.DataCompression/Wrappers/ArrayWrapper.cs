using System;
using System.Collections;
using System.Collections.Generic;

namespace gfoidl.DataCompression
{
    internal readonly struct ArrayWrapper : IList<DataPoint>
    {
        private readonly DataPoint[] _arr;
        //---------------------------------------------------------------------
        public ArrayWrapper(in DataPoint[] arr) => _arr = arr;
        //---------------------------------------------------------------------
        public DataPoint this[int index]
        {
            get
            {
                // RCE https://github.com/dotnet/coreclr/pull/9773
                if ((uint)index >= (uint)_arr.Length) ThrowHelper.ThrowArgumentOutOfRange(nameof(index));

                return _arr[index];
            }
            set => throw new NotImplementedException();
        }
        //---------------------------------------------------------------------
        public int Count       => _arr.Length;
        public bool IsReadOnly => true;
        //---------------------------------------------------------------------
        public void Add(DataPoint item)                       => throw new NotImplementedException();
        public void Clear()                                   => throw new NotImplementedException();
        public bool Contains(DataPoint item)                  => throw new NotImplementedException();
        public void CopyTo(DataPoint[] array, int arrayIndex) => throw new NotImplementedException();
        public IEnumerator<DataPoint> GetEnumerator()         => throw new NotImplementedException();
        public int IndexOf(DataPoint item)                    => throw new NotImplementedException();
        public void Insert(int index, DataPoint item)         => throw new NotImplementedException();
        public bool Remove(DataPoint item)                    => throw new NotImplementedException();
        public void RemoveAt(int index)                       => throw new NotImplementedException();
        IEnumerator IEnumerable.GetEnumerator()               => throw new NotImplementedException();
    }
}