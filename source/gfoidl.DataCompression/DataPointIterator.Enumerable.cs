// (c) gfoidl, all rights reserved

using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using gfoidl.DataCompression.Builders;

namespace gfoidl.DataCompression
{
    public abstract partial class DataPointIterator
    {
        private protected IEnumerable<DataPoint>? _source;
        private protected IEnumerator<DataPoint>? _enumerator;
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
                    _lastArchived = _incoming;
                    _snapShot     = _incoming;
                    _state        = 1;
                    this.Init(_incoming, ref _snapShot);
                    return true;
                case 1:
                    while (_enumerator.MoveNext())
                    {
                        _incoming       = _enumerator.Current;
                        ref var archive = ref this.IsPointToArchive(_incoming, _lastArchived);

                        if (!archive.Archive)
                        {
                            this.UpdateFilters(_incoming, _lastArchived);
                            _snapShot = _incoming;
                            continue;
                        }

                        if (!archive.MaxDelta && _lastArchived != _snapShot)
                        {
                            _lastArchived = _snapShot;
                            _snapShot     = _incoming;
                            _state        = 2;
                            return true;
                        }

                        Debug.Assert(archive.Archive && archive.MaxDelta);
                        goto case MaxDeltaXState;
                    }

                    _state = -1;
                    if (_incoming != _lastArchived)     // sentinel check
                    {
                        _lastArchived = _incoming;
                        return true;
                    }
                    goto default;
                case 2:
                    this.Init(_incoming, ref _snapShot);
                    this.UpdateFilters(_incoming, _lastArchived);
                    this.HandleSkipMinDeltaX(_enumerator);
                    goto case 1;
                case MaxDeltaXState:
                    _lastArchived = _incoming;
                    _snapShot     = _incoming;
                    _state        = 1;
                    this.Init(_incoming, ref _snapShot);
                    this.HandleSkipMinDeltaX(_enumerator);
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
            var arrayBuilder                  = new ArrayBuilder<DataPoint>(initialize: true);
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
            var listBuilder                   = new ListBuilder<DataPoint>(initialize: true);
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
            DataPoint snapShot = incoming;

            this.Init(incoming, ref snapShot);
            builder.Add(incoming);

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
                    _lastArchived = snapShot;
                    snapShot      = incoming;

                    this.Init(incoming, ref snapShot);
                    this.UpdateFilters(incoming, _lastArchived);
                    this.HandleSkipMinDeltaX(enumerator);
                    continue;
                }

                Debug.Assert(archive.Archive && archive.MaxDelta);
                _lastArchived = incoming;
                snapShot      = incoming;
                builder.Add(incoming);
                this.Init(incoming, ref snapShot);
                this.HandleSkipMinDeltaX(enumerator);
            }

            if (incoming != _lastArchived)          // sentinel-check
                builder.Add(incoming);
        }
        //---------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void HandleSkipMinDeltaX(IEnumerator<DataPoint> enumerator)
        {
            Debug.Assert(_algorithm is not null);

            if (_algorithm.MinDeltaX.HasValue)
            {
                this.SkipMinDeltaX(enumerator);
            }
        }
        //---------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void SkipMinDeltaX(IEnumerator<DataPoint> enumerator)
        {
            Debug.Assert(_algorithm  is not null);
            Debug.Assert(_algorithm.MinDeltaX.HasValue);

            double minDeltaX = _algorithm.MinDeltaX.GetValueOrDefault();
            double snapShotX = _snapShot.X;

            while (enumerator.MoveNext())
            {
                DataPoint tmp = enumerator.Current;

                if ((tmp.X - snapShotX) >= minDeltaX)
                {
                    break;
                }
            }
        }
    }
}
