using System;
using System.Collections;
using System.Collections.Generic;

namespace gfoidl.DataCompression.Wrappers
{
#pragma warning disable CS1591
    public readonly struct ListWrapper<T> : IList<T>
    {
        private readonly List<T> _list;
        //---------------------------------------------------------------------
        public ListWrapper(List<T>? list) => _list = list ?? throw new ArgumentNullException(nameof(list));
        //---------------------------------------------------------------------
        public T this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }
        //---------------------------------------------------------------------
        public int Count => _list.Count;
        //---------------------------------------------------------------------
        public bool IsReadOnly                        => throw new NotImplementedException();
        public void Add(T item)                       => throw new NotImplementedException();
        public void Clear()                           => throw new NotImplementedException();
        public bool Contains(T item)                  => throw new NotImplementedException();
        public void CopyTo(T[] array, int arrayIndex) => throw new NotImplementedException();
        public IEnumerator<T> GetEnumerator()         => throw new NotImplementedException();
        public int IndexOf(T item)                    => throw new NotImplementedException();
        public void Insert(int index, T item)         => throw new NotImplementedException();
        public bool Remove(T item)                    => throw new NotImplementedException();
        public void RemoveAt(int index)               => throw new NotImplementedException();
        IEnumerator IEnumerable.GetEnumerator()       => throw new NotImplementedException();
    }
#pragma warning restore CS1591
}
