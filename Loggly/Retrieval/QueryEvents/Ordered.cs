﻿#region Apache 2 License
// Copyright (c) Applied Duality, Inc., All rights reserved.
// See License.txt in the project root for license information.
#endregion

using System;
using System.Linq.Expressions;
using System.Net.Http;
using System.Runtime.CompilerServices;
using Loggly;

namespace Loggly.Retrieval
{
    public struct OrderedEvents
    {
        HttpClient _client;
        Func<FilteredEvents.Event, FilteredEvents.Bool> _pattern;
        Func<DatedEvents.Event, DatedEvents.Bool> _timeRange;
        bool _descending;

        public OrderedEvents
            (HttpClient client, Func<FilteredEvents.Event, FilteredEvents.Bool> pattern, Func<DatedEvents.Event, DatedEvents.Bool> timeRange, bool descending)
        {
            _client = client;
            _pattern = pattern;
            _timeRange = timeRange;
            _descending = descending;
        }

        public ProjectedEvents<Loggly.Event> Select(Expression<Func<Loggly.Event, Loggly.Event>> selector)
        {
            return new ProjectedEvents<Loggly.Event>(_client, _pattern, _timeRange, _descending, selector);
        }
        public SkippedEvents<Loggly.Event> Skip(int n)
        {
            return new SkippedEvents<Loggly.Event>(_client, _pattern, _timeRange, _descending, x=>x, n);
        }
        public TakenEvents<Loggly.Event> Take(int n)
        {
            return new TakenEvents<Loggly.Event>(_client, _pattern, _timeRange, _descending, x=>x, 0, n);
        }

        public TaskAwaiter<Loggly.Event[]> GetAwaiter()
        {
            return this.Take(10).GetAwaiter();
        }

        public struct Time { }
        public struct Event
        {
            public Time Time { get { return new Time(); } }
        }
    }
}