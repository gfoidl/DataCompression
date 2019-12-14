using System;
using System.Collections;
using System.Collections.Generic;

namespace gfoidl.DataCompression.Wrappers
{
#pragma warning disable CS1591
    public readonly struct ArrayWrapper<T> : IList<T>
    {
        private readonly T[] _array;
        //---------------------------------------------------------------------
        public ArrayWrapper(T[]? array) => _array = array ?? throw new ArgumentNullException(nameof(array));
        //---------------------------------------------------------------------
        public T this[int index]
        {
            get => _array[index];
            set => _array[index] = value;
        }
        //---------------------------------------------------------------------
        public int Count => _array.Length;
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
