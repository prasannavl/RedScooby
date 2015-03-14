// Author: Prasanna V. Loganathar
// Created: 9:27 AM 07-03-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System.Runtime.Serialization;
using System.Threading.Tasks;
using RedScooby.Data.Tables;
using RedScooby.Models;
using RedScooby.Utilities;
using SQLite.Net;

namespace RedScooby.Data
{
    public class SqliteAppModelStore : IAppModelStore
    {
        private readonly SqliteStoreManager store;

        public SqliteAppModelStore(SqliteStoreManager storeManager)
        {
            store = storeManager;
        }

        public async Task<LocalSettingsModel> GetLocalSettingsAsync()
        {
            using (var conn = await GetConnectionAsync().ConfigureAwait(false))
            {
                var model = new LocalSettingsModel(new StreamingContext());
                var settings = conn.Find<LocalSettings>(x => x.Id == 0);
                if (settings == null) return new LocalSettingsModel();

                settings.PopulateModel(model);

                if (settings.HasActiveAssistState)
                {
                    model.LastConcernState = conn.Find<ConcernStatePersistence>(0);
                    model.LastDistressState = conn.Find<DistressPersistenceState>(0);
                    model.LastFeedbackState = conn.Find<FeedbackPersistenceState>(0);
                }

                model.PersistenceHelper.Activate();
                return model;
            }
        }

        public async Task<UserModel> GetUserAsync()
        {
            using (var conn = await GetConnectionAsync().ConfigureAwait(false))
            {
                var model = new UserModel(new StreamingContext());

                var user = conn.Find<User>(x => x.Id == 0);
                if (user != null)
                {
                    user.PopulateModel(model);
                    model.PersistenceHelper.Activate();

                    var session = conn.Find<UserSession>(x => x.Id == 0);
                    if (session != null)
                    {
                        session.PopulateModel(model.Session);
                        model.Session.PersistenceHelper.Activate();
                    }


                    var settings = conn.Find<UserSettings>(x => x.Id == 0);
                    if (settings != null)
                    {
                        settings.PopulateModel(model.Settings);
                        model.Settings.PersistenceHelper.Activate();
                    }

                    return model;
                }
            }

            return new UserModel();
        }

        public Task SaveSettingsAsync(LocalSettingsModel localSettings)
        {
            var list = new SqliteDbUpdateList(GetConnectionAsync(true), 1);
            var hasAssistState = false;

            list.AddNullDeletable(localSettings.LastConcernState, localSettings.PersistenceHelper);
            list.AddNullDeletable(localSettings.LastDistressState, localSettings.PersistenceHelper);
            list.AddNullDeletable(localSettings.LastFeedbackState, localSettings.PersistenceHelper);

            if (list.Items.Count > 0) hasAssistState = true;

            if (localSettings.PersistenceHelper.ShouldSave || hasAssistState)
            {
                list.Add<LocalSettingsModel, LocalSettings>(localSettings,
                    settings => settings.HasActiveAssistState = hasAssistState);
            }

            return list.UpdateAsync();
        }

        public Task SaveUserAsync(UserModel user)
        {
            var list = new SqliteDbUpdateList(GetConnectionAsync(true), 3);

            list.Add<UserModel, User>(user);
            list.Add<UserSessionModel, UserSession>(user.Session);
            list.Add<UserSettingsModel, UserSettings>(user.Settings);

            return list.UpdateAsync();
        }

        public Task DestroyStoreAsync()
        {
            return TaskCache.Completed;
        }

        public async Task InitializeStoreAsync()
        {
            //TODO: Create fast path with a schema version check

            var types = new[]
            {
                typeof (User),
                typeof (UserSettings),
                typeof (UserSession),
                typeof (LocalSettings),
                typeof (ConcernStatePersistence),
                typeof (DistressPersistenceState),
                typeof (FeedbackPersistenceState),
            };

            using (var conn = await GetConnectionAsync(true).ConfigureAwait(false))
            {
                foreach (var type in types)
                {
                    conn.CreateTable(type);
                }
            }
        }

        private Task<SQLiteConnection> GetConnectionAsync(bool writeAccess = false)
        {
            return Task.Run(() => store.GetAppModelStoreConnection(writeAccess));
        }
    }
}
