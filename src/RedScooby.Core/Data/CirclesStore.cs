// Author: Prasanna V. Loganathar
// Created: 10:14 PM 13-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Windows.UI.WebUI.Core;
using RedScooby.Data.Core;
using RedScooby.Data.Scaffold;
using RedScooby.Infrastructure.Framework;
using RedScooby.Models;
using RedScooby.Utilities;

namespace RedScooby.Data
{
    public sealed class CirclesKeyValueStore : PrefixedAsyncKeyValueStoreBase
    {
        public CirclesKeyValueStore(IAsyncKeyValueStore store) : base(store, @"circles\") { }
    }

    public class CirclesStore : ICirclesStore
    {
        private const string circlesIndexKey = @"_index";
        private readonly IAsyncKeyValueStore store;

        public CirclesStore(IAsyncKeyValueStore store)
        {
            this.store = store;
        }

        public Task InitializeStoreAsync()
        {
            return store.InitializeStoreAsync();
        }

        public Task DestroyStoreAsync()
        {
            return store.DestroyStoreAsync();
        }

        public async Task<IList<UserCircleIndexItem>> GetCirclesIndexAsync()
        {
            var res = await store.TryGetAsync<IList<UserCircleIndexItem>>(circlesIndexKey);
            return res.Success ? res.Result : null;
        }

        public async Task<CircleModel> GetCircleAsync(string localId)
        {
            var res = await store.TryGetAsync<CircleModel>(localId);
            return res.Success ? res.Result : null;
        }

        public async Task SaveCirclesIndexAsync(IList<UserCircleIndexItem> circleIndex)
        {
            await store.SetAsync(circlesIndexKey, circleIndex, true);
        }

        public async Task<string> SaveCircleAsync(CircleModel circleModel)
        {
            var id = circleModel.LocalId;
            if (id != null)
            {
                await store.SetAsync(id, circleModel, true);
            }
            else
            {
                Observable.Defer(async () =>
                {
                    id = Guid.NewGuid().ToString();
                    await store.SetAsync(id, circleModel, false);
                    return Observable.Return(Unit.Default);
                })
                    .Do(DelegatesCache.EmptyUnit, ex => ErrorHandler.Current.HandleSilentError(ex, "SaveCircleFailure"))
                    .Retry(2);
            }
            return id;
        }
    }
}