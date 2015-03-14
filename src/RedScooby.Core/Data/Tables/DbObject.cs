// Author: Prasanna V. Loganathar
// Created: 9:46 AM 05-03-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using RedScooby.Data.Core;
using SQLite.Net.Attributes;

namespace RedScooby.Data.Tables
{
    public class DbObject
    {
        public DateTimeOffset LastSaved { get; set; }
        public DateTimeOffset LastSynced { get; set; }

        public virtual PersistenceHelper CreatePersistenceHelper()
        {
            return new PersistenceHelper(LastSaved, LastSaved);
        }

        public virtual void PopulateFromPersistenceHelper(IPersistenceHelper helper)
        {
            if (helper == null) return;
            LastSaved = helper.LastSavedDate;
        }
    }

    public class DbObjectWithId : DbObject, IObjectWithId
    {
        [PrimaryKey]
        public int Id { get; set; }
    }

    public class DbObject<TModel> : DbObject where TModel : IPersistentObject
    {
        public virtual void PopulateModel(TModel model)
        {
            model.PersistenceHelper = CreatePersistenceHelper();
        }

        public virtual void PopulateFromModel(TModel model)
        {
            PopulateFromPersistenceHelper(model.PersistenceHelper);
        }
    }

    public interface IObjectWithId
    {
        int Id { get; set; }
    }


    public class DbObjectWithId<TModel> : DbObject<TModel>, IObjectWithId where TModel : IPersistentObject
    {
        [PrimaryKey]
        public int Id { get; set; }
    }
}
