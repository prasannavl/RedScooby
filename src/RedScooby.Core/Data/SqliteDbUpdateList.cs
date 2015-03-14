// Author: Prasanna V. Loganathar
// Created: 3:40 PM 10-03-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RedScooby.Data.Core;
using RedScooby.Data.Tables;
using RedScooby.Helpers;
using SQLite.Net;

namespace RedScooby.Data
{
    internal class SqliteDbUpdateList
    {
        public IList<DbUpdateData> Items;
        private readonly Task<SQLiteConnection> connection;

        public SqliteDbUpdateList(Task<SQLiteConnection> connection, int capacity = 4)
        {
            this.connection = connection;
            Items = new List<DbUpdateData>(capacity);
        }

        public void Add<TModel, TDbObject>(TModel model, Action<TDbObject> postModifierAction = null)
            where TModel : IPersistentObject
            where TDbObject : DbObject<TModel>, new()
        {
            if (model.PersistenceHelper.ShouldSave)
            {
                var dbo = model.ToDbObjectAndMarkSaved<TModel, TDbObject>();
                postModifierAction.InvokeIfNotNullWith(dbo);
                Items.Add(new DbUpdateData(dbo));
            }
        }

        public void AddNullDeletable<TModel, TDbObject>(TModel model)
            where TModel : IPersistentObject
            where TDbObject : DbObject<TModel>, new()
        {
            var mode = DbUpdateMode.Remove;
            if (model != null)
            {
                if (!model.PersistenceHelper.ShouldSave) return;
                mode = DbUpdateMode.InsertOrReplace;
            }

            Items.Add(new DbUpdateData(model.ToDbObjectAndMarkSaved<TModel, TDbObject>(), mode));
        }

        public void AddNullDeletable<TDbObject>(TDbObject model, IPersistenceHelper persistenceHelper)
            where TDbObject : DbObject, new()
        {
            if (!persistenceHelper.ShouldSave) return;

            var mode = DbUpdateMode.InsertOrReplace;
            if (model == null)
            {
                model = new TDbObject();
                mode = DbUpdateMode.Remove;
            }

            Items.Add(new DbUpdateData(model, mode));
        }

        public async Task UpdateAsync()
        {
            var list = Items;
            if (list.Count > 0)
            {
                using (var conn = await connection.ConfigureAwait(false))
                {
                    var c = conn;
                    c.RunInTransaction(() =>
                    {
                        foreach (var dbUpdateData in list)
                        {
                            if (dbUpdateData.Mode == DbUpdateMode.InsertOrReplace)
                            {
                                c.InsertOrReplace(dbUpdateData.Data);
                            }
                            else
                            {
                                c.Delete(dbUpdateData.Data);
                            }
                        }
                    });
                }
            }
        }

        internal class DbUpdateData
        {
            public DbUpdateData(DbObject data, DbUpdateMode mode = DbUpdateMode.InsertOrReplace)
            {
                Data = data;
                Mode = mode;
            }

            public DbObject Data { get; set; }
            public DbUpdateMode Mode { get; set; }
        }

        internal enum DbUpdateMode
        {
            InsertOrReplace,
            Remove,
        }
    }
}
