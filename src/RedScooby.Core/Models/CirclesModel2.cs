// Author: Prasanna V. Loganathar
// Created: 5:53 PM 13-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RedScooby.Common;
using RedScooby.Data;
using RedScooby.Data.Core;
using RedScooby.Helpers;
using RedScooby.Infrastructure.Scaffold;

namespace RedScooby.Models
{
    public sealed class CirclesModel
    {
        public const string InnerCircleName = "my inner circle";
        private readonly ICirclesStore circlesStore;
        private readonly IContactsStore contactsStore;
        private readonly HybridInterlockedMonitor monitor = new HybridInterlockedMonitor();
        private IList<UserCircleIndexItem> circleIndexCache;

        public CirclesModel(ICirclesStore circlesStore, IContactsStore contactsStore)
        {
            this.circlesStore = circlesStore;
            this.contactsStore = contactsStore;
        }

        public Task AcquireLockAsync()
        {
            return monitor.EnterAsync();
        }

        public void ReleaseLock()
        {
            monitor.Exit();
        }

        public async Task<UserCircle> GetCircleFromCircleIndexAsync(string circleName,
            IList<UserCircleIndexItem> circleIndex = null)
        {
            var index = circleIndex ?? (circleIndexCache ?? (circleIndexCache = await GetCirclesIndexAsync()));
            if (index != null)
            {
                var innerCircleIndexItem = index.FirstOrDefault(ix => ix.CircleName == circleName);
                if (innerCircleIndexItem != null)
                {
                    var circle = await GetCircleAsync(innerCircleIndexItem.LocalId);
                    return circle;
                }
            }

            return null;
        }

        public async Task<UserContact> GetContactFromCircleMemberMetadataAsync(CircleMemberMetadata metadata)
        {
            var user = await GetContactAsync(metadata.SourceLocalId);
            var projectedUser = new UserContact
            {
                Id = user.Id,
                Name = user.Name,
                LocalId = user.LocalId,
            };

            foreach (var selectedPhoneIndex in metadata.SelectedPhoneIndices)
            {
                projectedUser.PhoneNumbers.Add(user.PhoneNumbers.ElementAt(selectedPhoneIndex));
            }

            foreach (var selectedEmailIndex in metadata.SelectedEmailIndices)
            {
                projectedUser.Emails.Add(user.PhoneNumbers.ElementAt(selectedEmailIndex));
            }

            return projectedUser;
        }

        public async Task<IList<UserCircleIndexItem>> GetCirclesIndexAsync()
        {
            var index = await circlesStore.GetCirclesIndexAsync();
            if (index != null) return index;

            var innerCircleIndexItem = new UserCircleIndexItem
            {
                CircleName = InnerCircleName,
                LocalId = Guid.NewGuid().ToString()
            };

            index = new List<UserCircleIndexItem> {innerCircleIndexItem};

            var innerCircle = new UserCircle
            {
                CircleName = innerCircleIndexItem.CircleName,
                LocalId = innerCircleIndexItem.LocalId,
            };

            await circlesStore.SaveCircleAsync(innerCircle);
            await circlesStore.SaveCirclesIndexAsync(index);

            return index;
        }

        public Task SaveCirclesIndexAsync(IList<UserCircleIndexItem> circleIndex)
        {
            return circlesStore.SaveCirclesIndexAsync(circleIndex);
        }

        public Task<UserCircle> GetCircleAsync(string localId)
        {
            return circlesStore.GetCircleAsync(localId);
        }

        public Task<string> SaveContactAsync(UserContact contact)
        {
            return contactsStore.SaveContactAsync(contact);
        }

        public Task<UserContact> GetContactAsync(string localId)
        {
            return contactsStore.GetContactAsync(localId);
        }

        public Task<string> SaveCircleAsync(UserCircle circle)
        {
            return circlesStore.SaveCircleAsync(circle);
        }
    }

    public class CircleIndex : PersistentDispatchingObject
    {
        public CircleIndex() { }

        public CircleIndex(IPersistenceHelper helper)
        {
            PersistenceHelper = helper;
            Items = new PersistentDispatchingCollection<UserCircleIndexItem>(helper);
        }

        public PersistentDispatchingCollection<UserCircleIndexItem> Items { get; set; }
    }



    public class UserCircleIndexItem
    {
        public string LocalId { get; set; }
        public int Id { get; set; }
        public string CircleName { get; set; }
    }

    public class UserCircle : PersistentDispatchingObject
    {
        public UserCircle() { }

        public UserCircle(IPersistenceHelper helper)
        {
            PersistenceHelper = helper;
            MembersMetadata = new PersistentDispatchingCollection<CircleMemberMetadata>(helper);
        }

        public string LocalId { get; set; }
        public int Id { get; set; }
        public string CircleName { get; set; }
        public PersistentDispatchingCollection<CircleMemberMetadata> MembersMetadata { get; set; }
    }

    public class CircleMemberMetadata : PersistentDispatchingObject
    {
        [JsonConstructor]
        public CircleMemberMetadata() { }

        public CircleMemberMetadata(IPersistenceHelper helper)
        {
            PersistenceHelper = helper;
            SelectedEmailIndices = new PersistentDispatchingCollection<int>(helper);
            SelectedPhoneIndices = new PersistentDispatchingCollection<int>(helper);
        }

        public string SourceLocalId { get; set; }
        public int Id { get; set; }
        public int SourceId { get; set; }
        public string CircleName { get; set; }
        public PersistentDispatchingCollection<int> SelectedPhoneIndices { get; set; }
        public PersistentDispatchingCollection<int> SelectedEmailIndices { get; set; }
    }

    public class UserContact : PersistentDispatchingObject
    {
        [JsonConstructor]
        public UserContact() { }

        public UserContact(IPersistenceHelper helper)
        {
            PersistenceHelper = helper;
            PhoneNumbers = new PersistentDispatchingCollection<string>(helper);
            Emails = new PersistentDispatchingCollection<string>(helper);
        }

        public string LocalId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public PersistentDispatchingCollection<string> PhoneNumbers { get; set; }
        public PersistentDispatchingCollection<string> Emails { get; set; }
    }
}
