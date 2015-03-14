// Author: Prasanna V. Loganathar
// Created: 10:39 AM 24-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using Windows.Security.ExchangeActiveSyncProvisioning;
using Ailon.WP.Utils;
using Grace.DependencyInjection;
using RedScooby.Data;
using RedScooby.Infrastructure.Flashlight;
using RedScooby.Infrastructure.Siren;
using RedScooby.Models;

namespace RedScooby.Startup
{
    public sealed class WinRtComponentsRegistrar : ComponentsServiceRegistrarBase
    {
        public override void Register(DependencyInjectionContainer container, AppModel model)
        {
            base.Register(container, model);

            container.Configure(c =>
            {
                var clientInfo = new EasClientDeviceInformation();
                var device = PhoneNameResolver.Resolve(clientInfo.SystemManufacturer, clientInfo.SystemProductName);

                RegisterDataServices(c);
                c.SimpleExport<WinRtSiren>().As<ISirenService>().AndSingleton();
                RegisterFlashlight(c, model, device);
            });
        }

        public void RegisterFlashlight(IExportRegistrationBlock c, AppModel model, CanonicalPhoneName device)
        {
            var settings = model.LocalSettings;
            var options =
                WinRtFlashlightFactory.FlashlightCreationOptions.Default;

            if (!settings.IsFirstRun)
            {
                options = settings.UseQuickFlashlightDriver
                    ? WinRtFlashlightFactory.FlashlightCreationOptions.ForceQuickFlashlight
                    : WinRtFlashlightFactory.FlashlightCreationOptions.ForceFallback;
            }

            var service = WinRtFlashlightFactory.CreateForDevice(device, options);

            c.ExportInstance(service).As<IFlashlightService>();
            settings.UseQuickFlashlightDriver = ((FlashlightProxyService) service).InnerService.GetType() !=
                                                typeof (WinRtFlashlightFallbackService);
        }

        public void RegisterDataServices(IExportRegistrationBlock c)
        {
            c.SimpleExport<WinRtCountryInfoStore>().As<ICountryInfoStore>();
            c.SimpleExport<SqliteCirclesStore>().As<ICirclesStore>();
        }
    }
}
