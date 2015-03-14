// Author: Prasanna V. Loganathar
// Created: 12:29 AM 25-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using RedScooby.Infrastructure.Scaffold;

namespace RedScooby.Data.Core
{
    public interface IPersistentObject
    {
        IPersistenceHelper PersistenceHelper { get; set; }
    }

    public abstract class PersistentDispatchingObject : ChangeDispatchingObject, IPersistentObject
    {
        protected PersistentDispatchingObject() : this(new PersistenceHelper())
        {
            PersistenceHelper.Activate();
        }

        protected PersistentDispatchingObject(IPersistenceHelper persistenceHelper)
        {
            PersistenceHelper = persistenceHelper;
        }

        public IPersistenceHelper PersistenceHelper { get; set; }

        protected override void NotifyPropertyChanged(string propertyName = null)
        {
            base.NotifyPropertyChanged(propertyName);
            PersistenceHelper.ResolveModified();
        }
    }

    public abstract class PersistentObject : ChangeNotifyingObject, IPersistentObject
    {
        protected PersistentObject() : this(new PersistenceHelper())
        {
            PersistenceHelper.Activate();
        }

        protected PersistentObject(IPersistenceHelper persistenceHelper)
        {
            PersistenceHelper = persistenceHelper;
        }

        public IPersistenceHelper PersistenceHelper { get; set; }

        protected override void NotifyPropertyChanged(string propertyName = null)
        {
            base.NotifyPropertyChanged(propertyName);
            PersistenceHelper.ResolveModified();
        }
    }
}
