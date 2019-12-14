﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using gfoidl.DataCompression.Builders;

namespace gfoidl.DataCompression
{
    public partial class SwingingDoorCompression
    {
        private sealed class EnumerableIterator : SwingingDoorCompressionIterator
        {
            private readonly IEnumerable<DataPoint> _source;
            private readonly IEnumerator<DataPoint> _enumerator;
            private DataPoint                       _snapShot;
            //-----------------------------------------------------------------
            public EnumerableIterator(
                SwingingDoorCompression swingingDoorCompression,
                IEnumerable<DataPoint> source,
                IEnumerator<DataPoint> enumerator)
                : base(swingingDoorCompression)
            {
                _source     = source;
                _enumerator = enumerator;
            }
            //-----------------------------------------------------------------
            public override DataPointIterator Clone() => new EnumerableIterator(_swingingDoorCompression, _source, _enumerator);
            //-----------------------------------------------------------------
            public override bool MoveNext()
            {
                switch (_state)
                {
                    default:
                        this.Dispose();
                        return false;
                    case 0:
                        _snapShot     = _enumerator.Current;
                        _lastArchived = _snapShot;
                        _incoming     = _snapShot;          // sentinel, nullable would be possible but to much work around
                        _current      = _snapShot;
                        this.OpenNewDoor(_incoming);
                        _state        = 1;
                        return true;
                    case 1:
                        while (_enumerator.MoveNext())
                        {
                            _incoming       = _enumerator.Current;
                            this.IsPointToArchive(_incoming, _lastArchived);
                            ref var archive = ref _archive;

                            if (!archive.Archive)
                            {
                                this.CloseTheDoor(_incoming, _lastArchived);
                                _snapShot = _incoming;
                                continue;
                            }

                            if (!archive.MaxDelta)
                            {
                                _current = _snapShot;
                                _state   = 2;
                                return true;
                            }

                            goto case 2;
                        }

                        _state = -1;
                        if (_incoming != _lastArchived)     // sentinel-check
                        {
                            _current = _incoming;
                            return true;
                        }
                        return false;
                    case 2:
                        if (_swingingDoorCompression._minDeltaXHasValue)
                            this.SkipMinDeltaX(_snapShot);

                        _current = _incoming;
                        _state   = 1;
                        this.OpenNewDoor(_incoming);
                        return true;
                    case InitialState:
                        ThrowHelper.ThrowInvalidOperation(ThrowHelper.ExceptionResource.GetEnumerator_must_be_called_first);
                        return false;
                    case DisposedState:
                        ThrowHelper.ThrowIfDisposed(ThrowHelper.ExceptionArgument.iterator);
                        return false;
                }
            }
            //---------------------------------------------------------------------
            public override DataPoint[] ToArray()
            {
                IEnumerator<DataPoint> enumerator = _source.GetEnumerator();

                var arrayBuilder = new ArrayBuilder<DataPoint>(true);
                this.BuildCollection(enumerator, ref arrayBuilder);

                return arrayBuilder.ToArray();
            }
            //---------------------------------------------------------------------
            public override List<DataPoint> ToList()
            {
                IEnumerator<DataPoint> enumerator = _source.GetEnumerator();

                var listBuilder = new ListBuilder<DataPoint>(true);
                this.BuildCollection(enumerator, ref listBuilder);

                return listBuilder.ToList();
            }
            //---------------------------------------------------------------------
            private void BuildCollection<TBuilder>(IEnumerator<DataPoint> enumerator, ref TBuilder builder)
                where TBuilder : ICollectionBuilder<DataPoint>
            {
                enumerator.MoveNext();
                DataPoint snapShot = enumerator.Current;
                _lastArchived      = snapShot;
                DataPoint incoming = snapShot;          // sentinel, nullable would be possible but to much work around

                builder.Add(snapShot);
                this.OpenNewDoor(snapShot);

                while (enumerator.MoveNext())
                {
                    incoming        = enumerator.Current;
                    this.IsPointToArchive(incoming, _lastArchived);
                    ref var archive = ref _archive;

                    if (!archive.Archive)
                    {
                        this.CloseTheDoor(incoming, _lastArchived);
                        snapShot = incoming;
                        continue;
                    }

                    if (!archive.MaxDelta)
                        builder.Add(snapShot);

                    if (_swingingDoorCompression._minDeltaXHasValue)
                    {
                        this.SkipMinDeltaX(snapShot);
                        incoming = _incoming;
                    }

                    builder.Add(incoming);
                    this.OpenNewDoor(incoming);
                }

                if (incoming != _lastArchived)          // sentinel-check
                    builder.Add(incoming);
            }
            //---------------------------------------------------------------------
            [MethodImpl(MethodImplOptions.NoInlining)]
            private void SkipMinDeltaX(in DataPoint snapShot)
            {
                double snapShot_x = snapShot.X;
                double minDeltaX  = _swingingDoorCompression._minDeltaX;

                while (_enumerator.MoveNext())
                {
                    DataPoint tmp = _enumerator.Current;

                    if ((tmp.X - snapShot_x) > minDeltaX)
                    {
                        _incoming = tmp;
                        break;
                    }
                }
            }
        }
    }
}
