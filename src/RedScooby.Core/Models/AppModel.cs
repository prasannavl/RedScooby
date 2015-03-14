// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System.Collections.Generic;
using System.Threading.Tasks;
using RedScooby.Data;
using RedScooby.Data.Core;

namespace RedScooby.Models
{
    public sealed class AppModel : IAsyncStoreFoundation
    {
        private readonly IAppModelStore store;

        public AppModel(IAppModelStore store)
        {
            this.store = store;
        }

        public UserModel User { get; set; }
        public LocalSettingsModel LocalSettings { get; set; }

        Task IAsyncStoreFoundation.InitializeStoreAsync()
        {
            return store.InitializeStoreAsync();
        }

        Task IAsyncStoreFoundation.DestroyStoreAsync()
        {
            return store.DestroyStoreAsync();
        }

        public async Task LoadAllAsync()
        {
            var userTask = store.GetUserAsync();
            var settingsTask = store.GetLocalSettingsAsync();

            await Task.WhenAll(userTask, settingsTask).ConfigureAwait(false);

            User = userTask.Result;
            LocalSettings = settingsTask.Result;
        }

        public async Task SaveAllAsync()
        {
            var tasks = new List<Task>(2) {SaveUserAsync(), SaveLocalSettingsAsync()};
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        public Task SaveUserAsync()
        {
            var user = User;
            return store.SaveUserAsync(user);
        }

        public Task SaveLocalSettingsAsync()
        {
            var settings = LocalSettings;
            return store.SaveSettingsAsync(settings);
        }
    }
}
