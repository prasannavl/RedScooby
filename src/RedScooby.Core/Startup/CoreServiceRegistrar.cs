// Author: Prasanna V. Loganathar
// Created: 9:19 PM 23-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System.Globalization;
using System.Net.Http.Formatting;
using Grace.DependencyInjection;
using Newtonsoft.Json;
using RedScooby.Logging;
using RedScooby.Models;

namespace RedScooby.Startup
{
    public abstract class CoreServiceRegistrarBase : ICoreServiceRegistrar<DependencyInjectionContainer>
    {
        public virtual void Register(DependencyInjectionContainer container)
        {
            Log.Info("Registering services");

            RegisterCoreDataEssentials(container);
        }

        public DependencyInjectionContainer CreateContainer()
        {
            return new DependencyInjectionContainer {AutoRegisterUnknown = true};
        }

        public void RegisterCoreDataEssentials(DependencyInjectionContainer container)
        {
            var jsonSettings = new JsonSerializerSettings
            {
                ObjectCreationHandling = ObjectCreationHandling.Reuse,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                Formatting = Formatting.None,
                Culture = CultureInfo.InvariantCulture,
                TypeNameHandling = TypeNameHandling.None,
            };

            var jsonFormatter = new JsonMediaTypeFormatter()
            {
                SerializerSettings = jsonSettings,
            };

            container.Configure(c =>
            {
                c.SimpleExport<AppModel>().AndSingleton();
                c.SimpleExport<CirclesModel>().AndSingleton();

                c.ExportInstance(jsonSettings).As<JsonSerializerSettings>();
                c.ExportInstance(jsonFormatter).As<MediaTypeFormatter>();
            });
        }
    }
}
