// Author: Prasanna V. Loganathar
// Created: 3:13 AM 15-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System.Threading.Tasks;
using RedScooby.Common;
using RedScooby.Data.Core;

namespace RedScooby.Data.Scaffold
{
    public class PrefixedAsyncKeyValueStoreBase : IAsyncKeyValueStore
    {
        private readonly IAsyncKeyValueStore store;
        private readonly string prefix;

        public PrefixedAsyncKeyValueStoreBase(IAsyncKeyValueStore store, string prefix)
        {
            this.store = store;
            this.prefix = prefix;
        }

        public Task<bool> ExistsAsync(string key)
        {
            return store.ExistsAsync(prefix + key);
        }

        public Task<T> GetAsync<T>(string key)
        {
            return store.GetAsync<T>(prefix + key);
        }

        public Task RemoveAsync(string key)
        {
            return store.RemoveAsync(prefix + key);
        }

        public Task SetAsync<T>(string key, T data, bool overwrite = true)
        {
            return store.SetAsync(prefix + key, data, overwrite);
        }

        public Task<AsyncResult<T>> TryGetAsync<T>(string key)
        {
            return store.TryGetAsync<T>(prefix + key);
        }

        public Task InitializeStoreAsync()
        {
            return store.InitializeStoreAsync();
        }

        public Task DestroyStoreAsync()
        {
            return store.RemoveAsync(prefix);
        }

        public bool Exists(string key)
        {
            return store.Exists(prefix + key);
        }

        public T Get<T>(string key)
        {
            return store.Get<T>(prefix + key);
        }

        public void Remove(string key)
        {
            store.Remove(prefix + key);
        }

        public void Set<T>(string key, T data, bool overwrite = true)
        {
            store.Set(prefix + key, data, overwrite);
        }

        public bool TryGet<T>(string key, out T result)
        {
            return store.TryGet(prefix + key, out result);
        }

        public void InitializeStore()
        {
            store.InitializeStore();
        }

        public void DestroyStore()
        {
            store.Remove(prefix);
        }
    }
}
