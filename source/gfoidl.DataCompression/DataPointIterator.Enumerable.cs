using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using gfoidl.DataCompression.Builders;

namespace gfoidl.DataCompression
{
    public abstract partial class DataPointIterator
    {
#pragma warning disable CS1591
        protected IEnumerable<DataPoint>? _source;
        protected IEnumerator<DataPoint>? _enumerator;
#pragma warning restore CS1591
        //---------------------------------------------------------------------
        /// <summary>
        /// Sets the algorithm for this <see cref="DataPointIterator" />.
        /// </summary>
        protected internal void SetData(Compression algorithm, IEnumerable<DataPoint> data)
        {
            if (data is null) ThrowHelper.ThrowArgumentNull(ThrowHelper.ExceptionArgument.data);

            this.SetData(algorithm);
            _source     = data;
            _enumerator = data.GetEnumerator();
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Advances the enumerator to the next element.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the enumerator was successfully advanced to the next element;
        /// <c>false</c> if the enumerator has passed the end of the collection.
        /// </returns>
        public virtual bool MoveNext()
        {
            Debug.Assert(_enumerator != null);

            switch (_state)
            {
                case 0:
                    if (!_enumerator.MoveNext()) return false;
                    _incoming     = _enumerator.Current;
                    _lastArchived = _snapShot;
                    _current      = _incoming;
                    this.Init(_incoming, ref _snapShot);
                    _state        = 1;
                    return true;
                case 1:
                    while (_enumerator.MoveNext())
                    {
                        _incoming = _enumerator.Current;
                        ref var archive = ref this.IsPointToArchive(_incoming, _lastArchived);

                        if (!archive.Archive)
                        {
                            this.UpdateFilters(_incoming, _lastArchived);
                            _snapShot = _incoming;
                            continue;
                        }

                        if (!archive.MaxDelta && _lastArchived != _snapShot)
                        {
                            _current = _snapShot;
                            _state = 2;
                            return true;
                        }

                        goto case 2;
                    }

                    _state = -1;
                    if (_incoming != _lastArchived)     // sentinel check
                    {
                        _current = _incoming;
                        return true;
                    }
                    return false;
                case 2:
                    _current = this.HandleSkipMinDeltaX(_enumerator, ref _incoming, _snapShot.X);
                    _state   = 1;
                    this.Init(_incoming, ref _snapShot);
                    return true;
                case InitialState:
                    ThrowHelper.ThrowInvalidOperation(ThrowHelper.ExceptionResource.GetEnumerator_must_be_called_first);
                    return false;
                case DisposedState:
                    ThrowHelper.ThrowIfDisposed(ThrowHelper.ExceptionArgument.iterator);
                    return false;
                default:
                    return false;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Returns an array of the compressed <see cref="DataPoint" />s.
        /// </summary>
        /// <returns>An array of the compressed <see cref="DataPoint" />s.</returns>
        public virtual DataPoint[] ToArray()
        {
            Debug.Assert(_source != null);

            IEnumerator<DataPoint> enumerator = _source.GetEnumerator();
            var arrayBuilder                  = new ArrayBuilder<DataPoint>(true);
            this.BuildCollection(enumerator, ref arrayBuilder);

            return arrayBuilder.ToArray();
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Returns a list of the compressed <see cref="DataPoint" />s.
        /// </summary>
        /// <returns>A list of the compressed <see cref="DataPoint" />s.</returns>
        public virtual List<DataPoint> ToList()
        {
            Debug.Assert(_source != null);

            IEnumerator<DataPoint> enumerator = _source.GetEnumerator();
            var listBuilder                   = new ListBuilder<DataPoint>(true);
            this.BuildCollection(enumerator, ref listBuilder);

            return listBuilder.ToList();
        }
        //---------------------------------------------------------------------
        private void BuildCollection<TBuilder>(IEnumerator<DataPoint> enumerator, ref TBuilder builder)
            where TBuilder : ICollectionBuilder<DataPoint>
        {
            if (!enumerator.MoveNext()) return;

            DataPoint incoming = enumerator.Current;
            _lastArchived      = incoming;
            DataPoint snapShot = default;

            builder.Add(incoming);
            this.Init(incoming, ref snapShot);

            while (enumerator.MoveNext())
            {
                incoming        = enumerator.Current;
                ref var archive = ref this.IsPointToArchive(incoming, _lastArchived);

                if (!archive.Archive)
                {
                    this.UpdateFilters(incoming, _lastArchived);
                    snapShot = incoming;
                    continue;
                }

                if (!archive.MaxDelta && _lastArchived != snapShot)
                {
                    builder.Add(snapShot);
                }

                incoming = this.HandleSkipMinDeltaX(enumerator, ref incoming, snapShot.X);

                builder.Add(incoming);
                this.Init(incoming, ref snapShot);
            }

            if (incoming != _lastArchived)          // sentinel-check
                builder.Add(incoming);
        }
        //---------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ref DataPoint HandleSkipMinDeltaX(IEnumerator<DataPoint> enumerator, ref DataPoint incoming, double snapShotX)
        {
            Debug.Assert(_algorithm is not null);

            if (_algorithm._minDeltaXHasValue)
            {
                this.SkipMinDeltaX(enumerator, ref incoming, snapShotX);
            }

            return ref incoming;
        }
        //---------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void SkipMinDeltaX(IEnumerator<DataPoint> enumerator, ref DataPoint incoming, double snapShotX)
        {
            Debug.Assert(_algorithm is not null);

            double minDeltaX = _algorithm._minDeltaX;

            while (enumerator.MoveNext())
            {
                DataPoint tmp = enumerator.Current;

                if ((tmp.X - snapShotX) > minDeltaX)
                {
                    incoming = tmp;
                    break;
                }
            }
        }
    }
}
