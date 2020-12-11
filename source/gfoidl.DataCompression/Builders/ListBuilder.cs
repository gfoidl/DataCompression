// (c) gfoidl, all rights reserved

using System.Collections.Generic;

namespace gfoidl.DataCompression.Builders
{
    internal struct ListBuilder<T> : ICollectionBuilder<T>
    {
        private readonly List<T> _list;
        //---------------------------------------------------------------------
        public ListBuilder(bool initialize) => _list = new List<T>();
        //---------------------------------------------------------------------
        public void Add(T item) => _list.Add(item);
        //---------------------------------------------------------------------
        public readonly List<T> ToList() => _list;
    }
}
