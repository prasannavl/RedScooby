// Author: Prasanna V. Loganathar
// Created: 9:21 PM 13-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Threading.Tasks;

namespace RedScooby.Data.Core
{
    public interface ICache : IKeyValueStore
    {
        void Set<T>(string key, T value, TimeSpan? timeSpan = null, CachePriority priority = CachePriority.Normal,
            bool overwrite = true);
    }

    public interface IAsyncCache : ICache, IAsyncKeyValueStore
    {
        Task SetAsync<T>(string key, T value, TimeSpan? timeSpan = null, CachePriority priority = CachePriority.Normal,
            bool overwrite = true);
    }

    public enum CachePriority
    {
        Critical,
        High,
        Normal,
        Low,
        VeryLow
    }
}
