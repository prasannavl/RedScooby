// Author: Prasanna V. Loganathar
// Created: 10:14 PM 13-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using RedScooby.Data.Core;
using RedScooby.Data.Scaffold;
using RedScooby.Infrastructure.Framework;
using RedScooby.Models;
using RedScooby.Utilities;

namespace RedScooby.Data
{
    public sealed class ContactsKeyValueStore : PrefixedAsyncKeyValueStoreBase
    {
        public ContactsKeyValueStore(IAsyncKeyValueStore store) : base(store, @"contacts\") { }
    }

    public class ContactsStore : IContactsStore
    {
        private readonly IAsyncKeyValueStore store;

        public ContactsStore(IAsyncKeyValueStore store)
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

        public async Task<string> SaveContactAsync(ContactModel contactModel)
        {
            var id = contactModel.LocalId;
            if (id != null)
            {
                await store.SetAsync(id, contactModel, true);
            }
            else
            {
                Observable.Defer(async () =>
                {
                    id = Guid.NewGuid().ToString();
                    await store.SetAsync(id, contactModel, false);
                    return Observable.Return(Unit.Default);
                })
                    .Do(DelegatesCache.EmptyUnit, ex => ErrorHandler.Current.HandleSilentError(ex, "SaveContactFailure"))
                    .Retry(2);
            }
            return id;
        }

        public async Task<ContactModel> GetContactAsync(string localId)
        {
            var res = await store.TryGetAsync<ContactModel>(localId);
            return res.Success ? res.Result : null;
        }
    }
}