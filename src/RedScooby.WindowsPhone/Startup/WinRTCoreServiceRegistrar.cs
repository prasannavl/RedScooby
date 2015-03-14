// Author: Prasanna V. Loganathar
// Created: 10:22 PM 23-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using Grace.DependencyInjection;
using RedScooby.Data;
using SQLite.Net.Interop;
using SQLite.Net.Platform.WinRT;

namespace RedScooby.Startup
{
    public sealed class WinRtCoreServiceRegistrar : CoreServiceRegistrarBase
    {
        public override void Register(DependencyInjectionContainer container)
        {
            base.Register(container);

            container.Configure(c =>
            {
                c.SimpleExport<SqliteAppModelStore>().As<IAppModelStore>();
                c.SimpleExport<SQLitePlatformWinRT>().As<ISQLitePlatform>();
                c.SimpleExport<SqliteStoreManager>().AndSingleton();
            });
        }
    }
}
