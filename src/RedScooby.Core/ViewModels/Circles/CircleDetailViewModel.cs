// Author: Prasanna V. Loganathar
// Created: 6:14 AM 13-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using RedScooby.Data.Tables;
using RedScooby.Infrastructure.Composition;
using RedScooby.Infrastructure.Framework;
using RedScooby.Models;

namespace RedScooby.ViewModels.Circles
{
    public class CircleDetailViewModel : ViewModelBase
    {
        private readonly CirclesModel model;
        private readonly ThumbnailProviderWrapper thumbnailProviderWrapper = new ThumbnailProviderWrapper();
        private CircleModel currentCircle;
        private string circleName;
        private ObservableCollection<UserContactWithThumbnail> members;

        public CircleDetailViewModel(CirclesModel model)
        {
            this.model = model;
        }

        public string CircleName
        {
            get { return circleName; }
            private set { SetAndNotifyIfChanged(ref circleName, value); }
        }

        public ObservableCollection<UserContactWithThumbnail> Members
        {
            get { return members; }
            private set { SetAndNotifyIfChanged(ref members, value); }
        }

        public async Task SetContextAsync(int circleId)
        {
            var circle = await model.GetCircleAsync(circleId).ConfigureAwait(false);
            currentCircle = circle;

            if (circle != null)
            {
                CircleName = circle.Name;
                Members = new ObservableCollection<UserContactWithThumbnail>();
                foreach (var contact in circle.Contacts)
                {
                    var item = contact;
                    DispatchHelper.Current.Run(() => { Members.Add(CreateUserContactWithThumbnailFor(item)); });
                }
            }
        }

        public void SetThumbnailProvider(Func<ContactModel, Task<Stream>> provider)
        {
            thumbnailProviderWrapper.Provider = provider;
            RefreshChangeNotifications();
        }

        public async Task AddContactToCircle(ContactModel contactModel)
        {
            if (contactModel == null)
                throw new ArgumentNullException("contactModel");

            await model.SaveContactAsync(contactModel);
            if (contactModel.Id.HasValue)
            {
                await model.AddToCircleAsync(currentCircle.Id.Value, contactModel.Id.Value, CircleItemType.Contact);

                var existing = Members.FirstOrDefault(x => x.Contact.Name == contactModel.Name);
                if (existing != null)
                    DispatchHelper.Current.Run(() => { Members.Add(CreateUserContactWithThumbnailFor(contactModel)); });
            }
        }

        public async Task RemoveContactFromCircle(UserContactWithThumbnail userContact)
        {
            if (userContact == null)
                return;
            if (!userContact.Contact.Id.HasValue)
                return;

            await
                model.RemoveFromCircleAsync(currentCircle.Id.Value, userContact.Contact.Id.Value, CircleItemType.Contact);
            Members.Remove(userContact);
        }

        private UserContactWithThumbnail CreateUserContactWithThumbnailFor(ContactModel contactModel)
        {
            return new UserContactWithThumbnail
            {
                Contact = contactModel,
                ThumbnailProvider = () => thumbnailProviderWrapper.Provider(contactModel),
            };
        }

        public class UserContactWithThumbnail
        {
            public ContactModel Contact { get; set; }
            public Func<Task<Stream>> ThumbnailProvider { get; set; }
        }

        private class ThumbnailProviderWrapper
        {
            public Func<ContactModel, Task<Stream>> Provider { get; set; }
        }
    }
}
