// Author: Prasanna V. Loganathar
// Created: 9:41 PM 11-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System.Threading.Tasks;
using RedScooby.Models;
using RedScooby.Utilities;

namespace RedScooby.Data.Design
{
    public sealed class SampleAppModelStore : IAppModelStore
    {
        private readonly SampleAppModel model = new SampleAppModel();

        public Task<UserModel> GetUserAsync()
        {
            return Task.FromResult(model.GetUserModel());
        }

        public Task<LocalSettingsModel> GetLocalSettingsAsync()
        {
            return Task.FromResult(model.GetSettingsModel());
        }

        public Task SaveUserAsync(UserModel user)
        {
            return TaskCache.Completed;
        }

        public Task SaveSettingsAsync(LocalSettingsModel settings)
        {
            return TaskCache.Completed;
        }

        public Task InitializeStoreAsync()
        {
            return TaskCache.Completed;
        }

        public Task DestroyStoreAsync()
        {
            return TaskCache.Completed;
        }
    }
}
