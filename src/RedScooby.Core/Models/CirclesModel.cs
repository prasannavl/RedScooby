// Author: Prasanna V. Loganathar
// Created: 6:24 PM 11-03-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using RedScooby.Data;
using RedScooby.Data.Core;
using RedScooby.Data.Tables;
using RedScooby.Infrastructure.Scaffold;

namespace RedScooby.Models
{
    public class CirclesModel : IAsyncStoreFoundation, ICirclesStore
    {
        private readonly SqliteCirclesStore store;
        private readonly TaskCompletionSource<bool> initTcs = new TaskCompletionSource<bool>();
        private int initSyncPoint;
        private IEnumerable<CircleGlance> allCirclesCache;

        public CirclesModel(SqliteCirclesStore store)
        {
            this.store = store;
        }

        Task IAsyncStoreFoundation.InitializeStoreAsync()
        {
            return store.InitializeStoreAsync();
        }

        Task IAsyncStoreFoundation.DestroyStoreAsync()
        {
            return store.DestroyStoreAsync();
        }

        public async Task AddToCircleAsync(int circleId, int itemId, CircleItemType itemType)
        {
            await EnsureInitialized();
            await store.AddToCircleAsync(circleId, itemId, itemType);
        }

        public async Task DeleteCircleAsync(int id)
        {
            await EnsureInitialized();
            await store.DeleteCircleAsync(id);
        }

        public async Task DeleteContactAsync(int id)
        {
            await EnsureInitialized();
            await store.DeleteContactAsync(id);
        }

        public async Task RemoveFromCircleAsync(int circleId, int itemId, CircleItemType itemType)
        {
            await EnsureInitialized();
            await store.RemoveFromCircleAsync(circleId, itemId, itemType);
        }

        public async Task<IEnumerable<CircleGlance>> GetAllCirclesAsync()
        {
            await EnsureInitialized();
            return (allCirclesCache = await store.GetAllCirclesAsync());
        }

        public async Task<CircleModel> GetCircleAsync(int id)
        {
            await EnsureInitialized();
            return await store.GetCircleAsync(id);
        }

        public async Task<ContactModel> GetContactAsync(int id)
        {
            await EnsureInitialized();
            return await store.GetContactAsync(id);
        }

        public async Task SaveCircleMetadataAsync(CircleModel circleModel)
        {
            await EnsureInitialized();
            await store.SaveCircleMetadataAsync(circleModel);
        }

        public async Task SaveContactAsync(ContactModel contactModel)
        {
            await EnsureInitialized();
            await store.SaveContactAsync(contactModel);
        }

        public async Task<IEnumerable<CircleGlance>> GetAllCirclesCache()
        {
            await EnsureInitialized();
            return allCirclesCache ?? (allCirclesCache = await store.GetAllCirclesAsync());
        }

        private async Task EnsureInitialized()
        {
            var state = Interlocked.CompareExchange(ref initSyncPoint, 1, 0);
            if (state == 2)
            {
                return;
            }
            if (state == 0)
            {
                await store.InitializeStoreAsync();
                initTcs.TrySetResult(true);
                Interlocked.Exchange(ref initSyncPoint, 2);
            }
            else if (state == 1)
            {
                await initTcs.Task;
            }
        }
    }

    public class CircleModel : ChangeDispatchingObject
    {
        public CircleModel()
        {
            Contacts = new ObservableCollection<ContactModel>();
            ChildCircles = new ObservableCollection<CircleModel>();
        }

        public int? Id { get; set; }
        public string RemoteId { get; set; }
        public string Name { get; set; }
        public ObservableCollection<ContactModel> Contacts { get; set; }
        public ObservableCollection<CircleModel> ChildCircles { get; set; }

        public Circle ToCircle()
        {
            return new Circle
            {
                Id = Id,
                Name = Name,
                RemoteId = RemoteId,
            };
        }
    }

    public class ContactModel : ChangeDispatchingObject
    {
        public ContactModel()
        {
            PhoneNumbers = new ObservableCollection<ContactPhoneNumber>();
            Emails = new ObservableCollection<ContactEmail>();
        }

        public int? Id { get; set; }
        public string RemoteId { get; set; }
        public string LocalStoreId { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public ObservableCollection<ContactPhoneNumber> PhoneNumbers { get; set; }
        public ObservableCollection<ContactEmail> Emails { get; set; }

        public Contact ToContact()
        {
            return new Contact
            {
                Name = Name,
                Notes = Notes,
                LocalStoreId = LocalStoreId,
                RemoteId = RemoteId,
                Id = Id,
            };
        }
    }
}
