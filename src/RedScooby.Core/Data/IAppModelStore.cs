// Author: Prasanna V. Loganathar
// Created: 8:53 PM 07-03-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System.Threading.Tasks;
using RedScooby.Data.Core;
using RedScooby.Models;

namespace RedScooby.Data
{
    public interface IAppModelStore : IAsyncStoreFoundation
    {
        Task<LocalSettingsModel> GetLocalSettingsAsync();
        Task<UserModel> GetUserAsync();
        Task SaveSettingsAsync(LocalSettingsModel localSettings);
        Task SaveUserAsync(UserModel user);
    }
}
