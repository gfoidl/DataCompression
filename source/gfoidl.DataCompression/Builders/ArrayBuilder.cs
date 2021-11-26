// (c) gfoidl, all rights reserved

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

/*
 * Inspired from https://github.com/dotnet/corefx/blob/master/src/Common/src/System/Collections/Generic/LargeArrayBuilder.cs
 * Initial copy from https://github.com/gfoidl/Stochastics/blob/f5b4612c2d7479fd219dea9f79568b91a9bf2e31/source/gfoidl.Stochastics/Builders/ArrayBuilder.cs
 */

namespace gfoidl.DataCompression.Builders
{
    [DebuggerDisplay("Count: {Count}")]
    internal struct ArrayBuilder<T> : ICollectionBuilder<T>
    {
        private const int StartCapacity   = 4;
        private const int ResizeThreshold = 8;
        private readonly int _maxCapacity;

        private List<T[]>? _buffers;
        private T[]        _firstBuffer;
        private T[]        _currentBuffer;
        private int        _index;
        private int        _count;
        //---------------------------------------------------------------------
        public ArrayBuilder(bool initialize) : this(int.MaxValue) { }
        //---------------------------------------------------------------------
        public ArrayBuilder(int maxCapacity) : this()
        {
            _maxCapacity = maxCapacity;
            _firstBuffer = _currentBuffer = new T[StartCapacity];
            _buffers     = null;
        }
        //---------------------------------------------------------------------
        public readonly int Count => _count;
        //---------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T item)
        {
            T[] buffer = _currentBuffer;
            int index  = _index;

            if ((uint)index >= (uint)buffer.Length)
            {
                this.AddWithBufferAllocation(item);
            }
            else
            {
                buffer[index] = item;
                _index        = index + 1;
            }

            _count++;
        }
        //---------------------------------------------------------------------
        // Cold-path
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void AddWithBufferAllocation(T item)
        {
            this.AllocateBuffer();
            int index             = _index;
            _currentBuffer[index] = item;
            _index                = index + 1;
        }
        //---------------------------------------------------------------------
        public void AddRange(IEnumerable<T> items)
        {
            Debug.Assert(items != null);

            if (items is T[] array)
            {
                this.AddRange(array);
                return;
            }

            using IEnumerator<T> enumerator = items!.GetEnumerator();
            T[] destination                 = _currentBuffer;
            int index                       = _index;

            // Continuously read in items from the enumerator, updating _count
            // and _index when we run out of space.
            while (enumerator.MoveNext())
            {
                T item = enumerator.Current;

                if ((uint)index >= (uint)destination.Length)
                    this.AddWithBufferAllocation(item, ref destination, ref index);
                else
                    destination[index] = item;

                index++;
            }

            // Final update to _count and _index.
            _count += index - _index;
            _index  = index;
        }
        //---------------------------------------------------------------------
        private void AddRange(T[] array)
        {
            for (int i = 0; i < array.Length; ++i)
            {
                this.Add(array[i]);
            }
        }
        //---------------------------------------------------------------------
        // Non-inline to improve code quality as uncommon path
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void AddWithBufferAllocation(T item, ref T[] destination, ref int index)
        {
            _count += index - _index;
            _index  = index;
            this.AllocateBuffer();
            destination           = _currentBuffer;
            index                 = _index;
            _currentBuffer[index] = item;
        }
        //---------------------------------------------------------------------
        public readonly T[] ToArray()
        {
            if (_count == 0)
                return Array.Empty<T>();

            if (this.TryMove(out T[] array))
                return array;

            array = new T[_count];
            this.CopyTo(array);

            return array;
        }
        //---------------------------------------------------------------------
        private readonly bool TryMove(out T[] array)
        {
            array = _firstBuffer;
            return _count == _firstBuffer.Length;
        }
        //---------------------------------------------------------------------
        private readonly void CopyTo(T[] array)
        {
            int arrayIndex = 0;
            int count      = _count;

            for (int i = 0; count > 0; ++i)
            {
                T[] buffer = this.GetBuffer(i);
                int toCopy = Math.Min(count, buffer.Length);

#if NETSTANDARD2_1
                buffer.AsSpan(0, toCopy).CopyTo(array.AsSpan(arrayIndex));
#else
                Array.Copy(buffer, 0, array, arrayIndex, toCopy);
#endif

                count      -= toCopy;
                arrayIndex += toCopy;
            }
        }
        //---------------------------------------------------------------------
        private void AllocateBuffer()
        {
            if ((uint)_count < ResizeThreshold)
            {
                int newCapacity = Math.Min(_count == 0 ? StartCapacity : _count * 2, _maxCapacity);
                _currentBuffer  = new T[newCapacity];
                Array.Copy(_firstBuffer, 0, _currentBuffer, 0, _count);
                _firstBuffer = _currentBuffer;
            }
            else
            {
                int newCapacity = ResizeThreshold;

                if (_count != ResizeThreshold)
                {
                    // Example scenario: Let's say _count == 64.
                    // Then our buffers look like this: | 8 | 8 | 16 | 32 |
                    // As you can see, our count will be just double the last buffer.
                    // Now, say _maxCapacity is 100. We will find the right amount to allocate by
                    // doing min(64, 100 - 64). The lhs represents double the last buffer,
                    // the rhs the limit minus the amount we've already allocated.

                    _buffers ??= new List<T[]>();
                    _buffers.Add(_currentBuffer);
                    newCapacity = Math.Min(_count, _maxCapacity - _count);
                }

                _currentBuffer = new T[newCapacity];
                _index         = 0;
            }
        }
        //---------------------------------------------------------------------
        private readonly T[] GetBuffer(int index)
        {
            return index == 0
                ? _firstBuffer
                : index <= _buffers?.Count
                    ? _buffers[index - 1]       // first "buffer" is _firstBuffer resized
                    : _currentBuffer;
        }
    }
}
