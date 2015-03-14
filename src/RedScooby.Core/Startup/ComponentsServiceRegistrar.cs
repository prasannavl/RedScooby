// Author: Prasanna V. Loganathar
// Created: 10:41 AM 24-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using Grace.DependencyInjection;
using PhoneNumbers;
using RedScooby.Components;
using RedScooby.Infrastructure.Flashlight;
using RedScooby.Infrastructure.Siren;
using RedScooby.Logging;
using RedScooby.Models;

namespace RedScooby.Startup
{
    public abstract class ComponentsServiceRegistrarBase : IServiceRegistrar<DependencyInjectionContainer>
    {
        public virtual void Register(DependencyInjectionContainer container, AppModel model)
        {
            container.Configure(c =>
            {
                c.SimpleExport<RegionManager>().AndSingleton();
                c.SimpleExport<FlashlightControl>().AndSingleton();
                c.SimpleExport<SirenControl>().AndSingleton();

                // Export it as delegate to make sure it can be lazy loaded.
                c.ExportInstance((s, ctx) =>
                {
                    Log.Trace("Instantiating PhoneNumberUtil");
                    return PhoneNumberUtil.GetInstance();
                });
            });
        }
    }
}
