// Author: Prasanna V. Loganathar
// Created: 9:27 AM 07-03-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Windows.Storage;
using SQLite.Net;
using SQLite.Net.Interop;

namespace RedScooby.Data
{
    public class SqliteStoreManager
    {
        private const string appModelFileName = "user.rds";
        private const string circlesFileName = "circles.rds";
        private const string cacheFileName = "cache.rds";
        private const string syncFileName = "sync.rds";
        private readonly ISQLitePlatform platform;
        private readonly IDictionary<string, TableMapping> tableMapsCache;

        public SqliteStoreManager(ISQLitePlatform platform)
        {
            this.platform = platform;
            tableMapsCache = new ConcurrentDictionary<string, TableMapping>();
        }

        public SQLiteConnection GetAppModelStoreConnection(bool writeAccess = false)
        {
            var path = GetPath(appModelFileName);
            return CreateConnection(path, writeAccess);
        }

        public SQLiteConnection GetCirclesStoreConnection(bool writeAccess = false)
        {
            var path = GetPath(circlesFileName);
            return CreateConnection(path, writeAccess);
        }

        public SQLiteConnection GetCacheStoreConnection(bool writeAccess = false)
        {
            var path = GetPath(cacheFileName);
            return CreateConnection(path, writeAccess);
        }

        private static string GetPath(string genericPath)
        {
#if NETFX_CORE
            return Path.Combine(ApplicationData.Current.LocalFolder.Path, genericPath);
#else
            return appModelFileName;
#endif
        }

        private SQLiteConnection CreateConnection(string path, bool writeAccess)
        {
            SQLiteOpenFlags flag;

            if (writeAccess) flag = SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex;
            else flag = SQLiteOpenFlags.ReadOnly | SQLiteOpenFlags.FullMutex;

            return new SQLiteConnection(platform, path, flag, true, null, tableMapsCache);
        }
    }
}
