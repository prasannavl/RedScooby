// Author: Prasanna V. Loganathar
// Created: 9:12 PM 13-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System.Collections.Generic;
using System.Threading.Tasks;
using RedScooby.Data.Core;
using RedScooby.Data.Tables;
using RedScooby.Models;

namespace RedScooby.Data
{
    public interface ICirclesStore : IAsyncStoreFoundation
    {
        Task AddToCircleAsync(int circleId, int itemId, CircleItemType itemType);
        Task DeleteCircleAsync(int id);
        Task DeleteContactAsync(int id);
        Task<IEnumerable<CircleGlance>> GetAllCirclesAsync();
        Task<CircleModel> GetCircleAsync(int id);
        Task<ContactModel> GetContactAsync(int id);
        Task RemoveFromCircleAsync(int circleId, int itemId, CircleItemType itemType);
        Task SaveCircleMetadataAsync(CircleModel circleModel);
        Task SaveContactAsync(ContactModel contactModel);
    }

    public class CircleGlance
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
