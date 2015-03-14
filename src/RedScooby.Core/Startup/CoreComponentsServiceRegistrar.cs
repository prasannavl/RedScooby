// Author: Prasanna V. Loganathar
// Created: 1:11 PM 24-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using Grace.DependencyInjection;
using RedScooby.Components;
using RedScooby.Infrastructure.Location;
using RedScooby.Models;

namespace RedScooby.Startup
{
    public abstract class CoreComponentsServiceRegistrarBase : IServiceRegistrar<DependencyInjectionContainer>
    {
        public virtual void Register(DependencyInjectionContainer container, AppModel model)
        {
            container.Configure(c =>
            {
                c.SimpleExport<LocationManager>().AndSingleton();
                c.SimpleExport<FeedbackManager>().AndSingleton();
                c.SimpleExport<HotlineManager>().AndSingleton();
                c.SimpleExport<ConcernManager>().AndSingleton();
                c.SimpleExport<DistressManager>().AndSingleton();
                c.SimpleExport<GeoCoder>();
            });
        }
    }
}
