﻿#region Apache 2 License
// Copyright (c) Applied Duality, Inc., All rights reserved.
// See License.txt in the project root for license information.
#endregion

using System;
using System.Linq.Expressions;
using System.Net.Http;
using System.Runtime.CompilerServices;

namespace Loggly.Retrieval
{
    public struct SkippedEvents<T>
    {
        HttpClient _client;
        Func<FilteredEvents.Event, FilteredEvents.Bool> _pattern;
        Func<DatedEvents.Event, DatedEvents.Bool> _timeRange;
        bool _descending;
        Expression<Func<Event, T>> _selector;
        int _skip;

        public SkippedEvents
            (HttpClient client
            , Func<FilteredEvents.Event, FilteredEvents.Bool> pattern
            , Func<DatedEvents.Event, DatedEvents.Bool> timeRange
            , bool descending
            , Expression<Func<Event, T>> selector
            , int skip
            )
        {
            _client = client;
            _pattern = pattern;
            _timeRange = timeRange;
            _descending = descending;
            _selector = selector;
            _skip = skip;
        }

        public TakenEvents<T> Take(int n)
        {
            return new TakenEvents<T>(_client, _pattern, _timeRange, _descending, _selector, _skip, n);
        }

        public TaskAwaiter<T[]> GetAwaiter()
        {
            return this.Take(10).GetAwaiter();
        }
    }
}