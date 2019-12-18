using System;
using System.Collections.Generic;
using System.Text;

namespace gfoidl.DataCompression
{
    public abstract partial class DataPointIterator
    {
        /// <summary>
        /// Base class for an indexed <see cref="DataPointIterator" />.
        /// </summary>
        /// <typeparam name="TList">The type of the list-wrapper.</typeparam>
        protected abstract class DataPointIndexedIterator<TList> : DataPointIterator
            where TList : notnull, IList<DataPoint>
        {
#pragma warning disable CS1591
            protected readonly TList _list;
            protected int _snapShotIndex;
            protected int _lastArchivedIndex;
            protected int _incomingIndex;
#pragma warning restore CS1591
            //-----------------------------------------------------------------
            protected DataPointIndexedIterator(Compression compression, TList source)
                : base(compression)
                => _list = source;
            //-----------------------------------------------------------------
        }
    }
}
