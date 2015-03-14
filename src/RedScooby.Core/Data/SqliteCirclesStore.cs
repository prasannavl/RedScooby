// Author: Prasanna V. Loganathar
// Created: 6:11 PM 11-03-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using RedScooby.Data.Tables;
using RedScooby.Helpers;
using RedScooby.Models;
using RedScooby.Utilities;
using SQLite.Net;

namespace RedScooby.Data
{
    public class SqliteCirclesStore : ICirclesStore
    {
        private readonly SqliteStoreManager store;

        public SqliteCirclesStore(SqliteStoreManager storeManager)
        {
            store = storeManager;
        }

        public Task DestroyStoreAsync()
        {
            return TaskCache.Completed;
        }

        public async Task InitializeStoreAsync()
        {
            //TODO: Create fast path with a schema version check
            var types = new Type[]
            {
                typeof (Contact),
                typeof (ContactEmail),
                typeof (ContactPhoneNumber),
                typeof (CircleItem),
                typeof (Circle),
            };


            using (var conn = await GetConnectionAsync(true).ConfigureAwait(false))
            {
                foreach (var type in types)
                {
                    conn.CreateTable(type);
                }

                var innerCircle = new Circle {Id = 0, Name = "my inner circle"};
                conn.InsertOrReplace(innerCircle);
            }
        }

        public async Task<IEnumerable<CircleGlance>> GetAllCirclesAsync()
        {
            using (var conn = await GetConnectionAsync())
            {
                return conn.Query<CircleGlance>("select id, name from Circles");
            }
        }

        public async Task<CircleModel> GetCircleAsync(int id)
        {
            using (var conn = await GetConnectionAsync())
            {
                return GetCircle(conn, id);
            }
        }

        public async Task<ContactModel> GetContactAsync(int id)
        {
            using (var conn = await GetConnectionAsync())
            {
                return GetContact(conn, id);
            }
        }

        public async Task SaveCircleMetadataAsync(CircleModel circle)
        {
            using (var conn = await GetConnectionAsync(true))
            {
                var ctx = circle.ToCircle();
                conn.InsertOrReplace(ctx);
                circle.Id = ctx.Id;
            }
        }

        public async Task SaveContactAsync(ContactModel contactModel)
        {
            using (var conn = await GetConnectionAsync(true))
            {
                conn.RunInTransaction(() =>
                {
                    //  Get actual contact, and resolve unmodified existing email, and phone ids

                    var existingContact =
                        conn.Query<ContactModel>("select * from Contacts where name = ? limit 1",
                            contactModel.Name).SingleOrDefault();

                    IList<ContactPhoneNumber> numbersSaveList = null;
                    IList<ContactEmail> emailsSaveList = null;

                    if (existingContact != null && existingContact.Id.HasValue)
                    {
                        contactModel.Id = existingContact.Id;

                        if (contactModel.PhoneNumbers.Count > 0)
                        {
                            numbersSaveList = new List<ContactPhoneNumber>();

                            var contactPhones = conn.Table<ContactPhoneNumber>()
                                .Where(x => x.ContactId == existingContact.Id).ToArray();

                            contactModel.PhoneNumbers.ForEach(x =>
                            {
                                var existing = contactPhones.FirstOrDefault(c => c.Number == x.Number);
                                if (existing == null)
                                    numbersSaveList.Add(x);
                                else
                                    x.Id = existing.Id;
                            });
                        }

                        if (contactModel.Emails.Count > 0)
                        {
                            emailsSaveList = new List<ContactEmail>();

                            var contactEmails = conn.Table<ContactEmail>()
                                .Where(x => x.ContactId == existingContact.Id).ToArray();

                            contactModel.Emails.ForEach(x =>
                            {
                                var existing = contactEmails.FirstOrDefault(c => c.Email == x.Email);
                                if (existing == null)
                                    emailsSaveList.Add(x);
                                else
                                    x.Id = existing.Id;
                            });

                            //var emailIdsStr = string.Join(",",
                            //    contactModel.Emails.Where(x => x.Id.HasValue).Select(x => x.Id.Value));

                            //conn.Execute(
                            //    "delete from ContactEmails where ContactId = ? and (Id not in (" + emailIdsStr + "))",
                            //    contactModel.Id.Value);
                        }
                    }

                    if (emailsSaveList == null)
                    {
                        emailsSaveList = contactModel.Emails;
                    }
                    if (numbersSaveList == null)
                    {
                        numbersSaveList = contactModel.PhoneNumbers;
                    }

                    var ctx = contactModel.ToContact();
                    conn.InsertOrReplace(ctx);
                    contactModel.Id = ctx.Id;

                    if (contactModel.Id.HasValue)
                    {
                        emailsSaveList.ForEach(x =>
                        {
                            x.ContactId = contactModel.Id.Value;
                            conn.InsertOrReplace(x);
                        });

                        numbersSaveList.ForEach(x =>
                        {
                            x.ContactId = contactModel.Id.Value;
                            conn.InsertOrReplace(x);
                        });
                    }
                });
            }
        }

        public async Task AddToCircleAsync(int circleId, int itemId, CircleItemType itemType)
        {
            using (var conn = await GetConnectionAsync(true))
            {
                var existing =
                    conn.ExecuteScalar<int>(
                        "select 1 from CircleItems where CircleId = ? and ItemId = ? and ItemType = ?",
                        circleId, itemId, itemType);

                if (existing != 1)
                {
                    conn.Insert(new CircleItem {CircleId = circleId, ItemId = itemId, ItemType = itemType});
                }
            }
        }

        public async Task RemoveFromCircleAsync(int circleId, int itemId, CircleItemType itemType)
        {
            using (var conn = await GetConnectionAsync(true))
            {
                conn.Execute("delete from CircleItems where CircleId = ? and ItemId = ? and ItemType = ?", circleId,
                    itemId, itemType);
            }
        }

        public async Task DeleteContactAsync(int id)
        {
            using (var conn = await GetConnectionAsync(true))
            {
                conn.RunInTransaction(() => { DeleteContactUnsafe(conn, id); });
            }
        }

        public async Task DeleteCircleAsync(int id)
        {
            using (var conn = await GetConnectionAsync(true))
            {
                conn.RunInTransaction(() =>
                {
                    var c = conn;
                    c.Execute("delete from CircleItems where CircleId = ?", id);
                    c.Execute("delete from Circles where Id = ?", id);
                });
            }
        }

        private CircleModel GetCircle(SQLiteConnection conn, int id)
        {
            var circle = conn.Query<CircleModel>("select * from Circles where Id = ? limit 1", id).SingleOrDefault();
            if (circle != null && circle.Id.HasValue)
            {
                var items =
                    conn.Table<CircleItem>().Where(x => x.CircleId == circle.Id);

                foreach (var item in items)
                {
                    if (item.ItemType == CircleItemType.Contact)
                    {
                        var contact = GetContact(conn, item.ItemId);
                        if (contact != null) circle.Contacts.Add(contact);
                    }
                    else if (item.ItemType == CircleItemType.Circle)
                    {
                        var childCircle = GetCircle(conn, item.ItemId);
                        if (childCircle != null) circle.ChildCircles.Add(childCircle);
                    }
                }
            }

            return circle;
        }

        private ContactModel GetContact(SQLiteConnection conn, int id)
        {
            var contact = conn.Query<ContactModel>("select * from Contacts where id = ? limit 1", id).SingleOrDefault();
            if (contact == null) return null;

            var contactPhones = conn.Table<ContactPhoneNumber>()
                .Where(x => x.ContactId == id);

            var contactEmails = conn.Table<ContactEmail>()
                .Where(x => x.ContactId == id);

            contact.Emails = new ObservableCollection<ContactEmail>(contactEmails);
            contact.PhoneNumbers = new ObservableCollection<ContactPhoneNumber>(contactPhones);

            return contact;
        }

        private void DeleteContactUnsafe(SQLiteConnection c, int id)
        {
            c.Execute("delete from CircleItems where ItemId = ?", id);
            c.Execute("delete from ContactPhoneNumbers where ContactId = ?", id);
            c.Execute("delete from ContactEmails where ContactId = ?", id);
            c.Execute("delete from Contacts where Id = ?", id);
        }

        private Task<SQLiteConnection> GetConnectionAsync(bool writeAccess = false)
        {
            return Task.Run(() => store.GetCirclesStoreConnection(writeAccess));
        }
    }
}
