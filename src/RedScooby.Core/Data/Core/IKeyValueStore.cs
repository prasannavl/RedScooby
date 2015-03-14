// Author: Prasanna V. Loganathar
// Created: 9:21 PM 13-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Threading.Tasks;
using RedScooby.Common;

namespace RedScooby.Data.Core
{
    public interface IKeyValueStore : IStoreFoundation
    {
        bool Exists(string key);
        T Get<T>(string key);
        void Remove(string key);
        void Set<T>(string key, T data, bool overwrite = true);
        bool TryGet<T>(string key, out T result);
    }

    public interface IAsyncKeyValueStore : IKeyValueStore, IAsyncStoreFoundation
    {
        Task<bool> ExistsAsync(string key);
        Task<T> GetAsync<T>(string key);
        Task RemoveAsync(string key);
        Task SetAsync<T>(string key, T data, bool overwrite = true);
        Task<AsyncResult<T>> TryGetAsync<T>(string key);
    }

    public interface IStoreFoundation
    {
        void DestroyStore();
        void InitializeStore();
    }

    public interface IAsyncStoreFoundation
    {
        Task DestroyStoreAsync();
        Task InitializeStoreAsync();
    }

    /// <summary>
    ///     Base for KeyValueStore with default implementations for TryGet.
    ///     Warning: These implementations are inefficient, and are only used as a reasonable default
    ///     for cases without appropriate override. Override them for efficient usage.
    /// </summary>
    public abstract class KeyValueStoreBase : IKeyValueStore
    {
        public virtual bool TryGet<T>(string key, out T result)
        {
            try
            {
                result = Get<T>(key);
                return true;
            }
            catch
            {
                result = default(T);
                return false;
            }
        }

        public abstract void Set<T>(string key, T data, bool overwrite = true);
        public abstract bool Exists(string key);
        public abstract T Get<T>(string key);
        public abstract void Remove(string key);
        public abstract void InitializeStore();
        public abstract void DestroyStore();
    }

    /// <summary>
    ///     Base for AsyncKeyValueStore with default implementations for TryGet from KeyValueStore, Set, Get,
    ///     Remove and Exists synchronous methods using Task.Run and Wait over the async methods.
    ///     Warning: These implementations are inefficient, and are only used as a reasonable default
    ///     for cases without appropriate override. Override them for efficient usage.
    /// </summary>
    public abstract class AsyncKeyValueStoreBase : KeyValueStoreBase, IAsyncKeyValueStore
    {
        public virtual async Task<AsyncResult<T>> TryGetAsync<T>(string key)
        {
            try
            {
                var res = await GetAsync<T>(key);
                return new AsyncResult<T>(res);
            }
            catch (Exception ex)
            {
                return new AsyncResult<T>(ex, default(T));
            }
        }

        public abstract Task<bool> ExistsAsync(string key);
        public abstract Task SetAsync<T>(string key, T data, bool overwrite = true);
        public abstract Task<T> GetAsync<T>(string key);
        public abstract Task RemoveAsync(string key);
        public abstract Task InitializeStoreAsync();
        public abstract Task DestroyStoreAsync();

        public override void Set<T>(string key, T data, bool overwrite = true)
        {
            Task.Run(() => SetAsync(key, data, overwrite)).Wait();
        }

        public override bool Exists(string key)
        {
            return Task.Run(() => ExistsAsync(key)).Result;
        }

        public override T Get<T>(string key)
        {
            return Task.Run(() => GetAsync<T>(key)).Result;
        }

        public override void Remove(string key)
        {
            Task.Run(() => RemoveAsync(key)).Wait();
        }

        public override void InitializeStore()
        {
            Task.Run(() => InitializeStoreAsync()).Wait();
        }

        public override void DestroyStore()
        {
            Task.Run(() => DestroyStoreAsync()).Wait();
        }
    }
}
