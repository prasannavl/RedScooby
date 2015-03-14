// Author: Prasanna V. Loganathar
// Created: 11:49 AM 24-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System.Collections.Generic;
using Grace.DependencyInjection;

namespace RedScooby.Startup
{
    public class ServiceRegistrationInfo
    {
        public ICoreServiceRegistrar<DependencyInjectionContainer> Core { get; set; }
        public IServiceRegistrar<DependencyInjectionContainer> CoreComponents { get; set; }
        public IServiceRegistrar<DependencyInjectionContainer> Components { get; set; }
        public IEnumerable<IServiceRegistrar<DependencyInjectionContainer>> Extensions { get; set; }

        public DependencyInjectionContainer CreateContainer()
        {
            return new DependencyInjectionContainer {AutoRegisterUnknown = true};
        }
    }
}
