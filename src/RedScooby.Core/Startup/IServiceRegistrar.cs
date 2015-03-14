// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using RedScooby.Models;

namespace RedScooby.Startup
{
    public interface IServiceRegistrar<in T>
    {
        void Register(T container, AppModel model);
    }

    public interface ICoreServiceRegistrar<in T>
    {
        void Register(T container);
    }
}
