// Author: Prasanna V. Loganathar
// Created: 1:20 PM 24-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using Grace.DependencyInjection;
using RedScooby.Infrastructure.Battery;
using RedScooby.Infrastructure.Location;
using RedScooby.Infrastructure.Networking;
using RedScooby.Models;

namespace RedScooby.Startup
{
    public sealed class WinRtCoreComponentsRegistrar : CoreComponentsServiceRegistrarBase
    {
        public override void Register(DependencyInjectionContainer container, AppModel model)
        {
            base.Register(container, model);
            container.Configure(c =>
            {
                c.SimpleExport<WinRtLocationService>().As<ILocationService>();
                c.SimpleExport<WinRtDataNetworkInfoProvider>().As<IDataNetworkInfoProvider>();
                c.SimpleExport<WinRtMobileNetworkInfoProvider>().As<IMobileNetworkInfoProvider>();
                c.SimpleExport<WindowsPhoneBatteryInfoService>().As<IBatteryInfoService>();
            });
        }
    }
}
