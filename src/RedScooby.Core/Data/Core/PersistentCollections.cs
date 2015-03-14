// Author: Prasanna V. Loganathar
// Created: 12:31 AM 25-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using RedScooby.Common;
using RedScooby.Infrastructure.Framework;

namespace RedScooby.Data.Core
{
    public interface IPersistentEnumerable : IPersistentObject, IEnumerable { }

    public interface IPersistentEnumerable<T> : IPersistentEnumerable, IList<T> { }

    public interface IPersistentCollection : IPersistentEnumerable, IList { }

    public interface IPersistentCollection<T> : IPersistentEnumerable<T> { }

    public interface IPersistentDictionary<TKey, TValue> : IPersistentEnumerable, IDictionary<TKey, TValue> { }

    public abstract class PersistentCollectionBase<T> : ObservableCollection<T>, IPersistentCollection<T>
    {
        protected PersistentCollectionBase() : this(new PersistenceHelper())
        {
            PersistenceHelper.Activate();
        }

        protected PersistentCollectionBase(IEnumerable<T> existing) : this(new PersistenceHelper(), existing)
        {
            PersistenceHelper.Activate();
        }

        protected PersistentCollectionBase(IPersistenceHelper persistenceHelper)
        {
            PersistenceHelper = persistenceHelper;
        }

        protected PersistentCollectionBase(IPersistenceHelper persistenceHelper, IEnumerable<T> existing)
            : base(existing)
        {
            PersistenceHelper = persistenceHelper;
        }

        public IPersistenceHelper PersistenceHelper { get; set; }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            PersistenceHelper.ResolveModified();
        }
    }


    public abstract class PersistentDispatchingCollectionBase<T> : ObservableCollection<T>, IPersistentCollection<T>
    {
        protected PersistentDispatchingCollectionBase() : this(new PersistenceHelper())
        {
            PersistenceHelper.Activate();
        }

        protected PersistentDispatchingCollectionBase(IEnumerable<T> existing) : this(new PersistenceHelper(), existing)
        {
            PersistenceHelper.Activate();
        }

        protected PersistentDispatchingCollectionBase(IPersistenceHelper persistenceHelper)
        {
            PersistenceHelper = persistenceHelper;
        }

        protected PersistentDispatchingCollectionBase(IPersistenceHelper persistenceHelper,
            IEnumerable<T> existing)
            : base(existing)
        {
            PersistenceHelper = persistenceHelper;
        }

        public IPersistenceHelper PersistenceHelper { get; set; }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            DispatchHelper.Current.Run(() => { base.OnPropertyChanged(e); });
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            DispatchHelper.Current.Run(() => { base.OnCollectionChanged(e); });
            PersistenceHelper.ResolveModified();
        }
    }

    public abstract class PersistentDictionaryBase<TKey, TValue> : ChangeNotifyingDictionary<TKey, TValue>,
        IPersistentDictionary<TKey, TValue>
    {
        protected PersistentDictionaryBase() : this(new PersistenceHelper())
        {
            PersistenceHelper.Activate();
        }

        protected PersistentDictionaryBase(IDictionary<TKey, TValue> existing) : this(new PersistenceHelper(), existing)
        {
            PersistenceHelper.Activate();
        }

        protected PersistentDictionaryBase(IPersistenceHelper persistenceHelper)
        {
            PersistenceHelper = persistenceHelper;
        }

        protected PersistentDictionaryBase(IPersistenceHelper persistenceHelper,
            IDictionary<TKey, TValue> existing) : base(existing)
        {
            PersistenceHelper = persistenceHelper;
        }

        public IPersistenceHelper PersistenceHelper { get; set; }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            PersistenceHelper.ResolveModified();
        }
    }

    public sealed class PersistentCollection<T> : PersistentCollectionBase<T>
    {
        public PersistentCollection() { }
        public PersistentCollection(IEnumerable<T> existing) : base(existing) { }
        public PersistentCollection(IPersistenceHelper persistenceHelper) : base(persistenceHelper) { }

        public PersistentCollection(IPersistenceHelper persistenceHelper, IEnumerable<T> existing)
            : base(persistenceHelper, existing) { }
    }

    public sealed class PersistentDispatchingCollection<T> : PersistentDispatchingCollectionBase<T>
    {
        public PersistentDispatchingCollection(IPersistenceHelper persistenceHelper) : base(persistenceHelper) { }

        public PersistentDispatchingCollection(IPersistenceHelper persistenceHelper, IEnumerable<T> existing)
            : base(persistenceHelper, existing) { }

        public PersistentDispatchingCollection() { }
        public PersistentDispatchingCollection(IEnumerable<T> existing) : base(existing) { }
    }

    public sealed class PersistentDictionary<TKey, TValue> : PersistentDictionaryBase<TKey, TValue>
    {
        public PersistentDictionary(IPersistenceHelper persistenceHelper) : base(persistenceHelper) { }

        public PersistentDictionary(IPersistenceHelper persistenceHelper, IDictionary<TKey, TValue> existing)
            : base(persistenceHelper, existing) { }

        public PersistentDictionary() { }
        public PersistentDictionary(IDictionary<TKey, TValue> existing) : base(existing) { }
    }
}
