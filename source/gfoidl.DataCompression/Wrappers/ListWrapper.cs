using System;
using System.Collections;
using System.Collections.Generic;

namespace gfoidl.DataCompression
{
    internal readonly struct ListWrapper : IList<DataPoint>
    {
        private readonly List<DataPoint> _list;
        //---------------------------------------------------------------------
        public ListWrapper(in List<DataPoint> list) => _list = list;
        //---------------------------------------------------------------------
        public DataPoint this[int index]
        {
            get => _list[index];
            set => throw new NotImplementedException();
        }
        //---------------------------------------------------------------------
        public int Count                                      => _list.Count;
        public bool IsReadOnly                                => true;
        public bool Contains(DataPoint item)                  => _list.Contains(item);
        public void CopyTo(DataPoint[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);
        public int IndexOf(DataPoint item)                    => _list.IndexOf(item);
        public IEnumerator<DataPoint> GetEnumerator()         => _list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()               => _list.GetEnumerator();
        //---------------------------------------------------------------------
        public void Add(DataPoint item)               => throw new NotImplementedException();
        public void Clear()                           => throw new NotImplementedException();
        public void Insert(int index, DataPoint item) => throw new NotImplementedException();
        public bool Remove(DataPoint item)            => throw new NotImplementedException();
        public void RemoveAt(int index)               => throw new NotImplementedException();
    }
}