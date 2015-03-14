// Author: Prasanna V. Loganathar
// Created: 8:12 PM 08-03-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using RedScooby.Data.Core;
using RedScooby.Data.Tables;

namespace RedScooby.Helpers
{
    public static class ModelExtensions
    {
        public static TDbObject ToDbObject<TDataModel, TDbObject>(this TDataModel source)
            where TDbObject : DbObject<TDataModel>, new()
            where TDataModel : IPersistentObject

        {
            var res = new TDbObject();
            if (source != null)
            {
                res.PopulateFromModel(source);
            }
            return res;
        }

        public static TDbObject ToDbObjectAndMarkSaved<TDataModel, TDbObject>(this TDataModel source)
            where TDbObject : DbObject<TDataModel>, new()
            where TDataModel : IPersistentObject
        {
            var res = new TDbObject();
            if (source != null)
            {
                source.PersistenceHelper.SetSaved();
                res.PopulateFromModel(source);
            }
            return res;
        }
    }
}
