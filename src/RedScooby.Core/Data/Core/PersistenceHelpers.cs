// Author: Prasanna V. Loganathar
// Created: 12:29 AM 25-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;

namespace RedScooby.Data.Core
{
    public interface IPersistenceHelper
    {
        bool IsActive { get; }
        DateTimeOffset LastModifiedDate { get; }
        DateTimeOffset LastSavedDate { get; }
        bool ShouldSave { get; }
        void Activate();
        void Deactivate();
        void SetModified();
        void SetSaved();
    }


    public sealed class PersistenceHelper : PersistenceHelperBase
    {
        public PersistenceHelper() { }

        public PersistenceHelper(DateTimeOffset lastSaved, DateTimeOffset? lastModified = null)
            : base(lastSaved, lastModified) { }
    }


    public abstract class PersistenceHelperBase : IPersistenceHelper
    {
        protected PersistenceHelperBase() { }

        protected PersistenceHelperBase(DateTimeOffset lastSaved, DateTimeOffset? lastModified = null)
        {
            LastSavedDate = lastSaved;
            if (lastModified.HasValue) LastModifiedDate = lastModified.Value;
        }

        public virtual void SetSaved()
        {
            LastSavedDate = DateTimeOffset.Now;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Activate()
        {
            IsActive = true;
        }

        public virtual void SetModified()
        {
            LastModifiedDate = DateTimeOffset.UtcNow;
        }

        public bool IsActive { get; private set; }
        public virtual DateTimeOffset LastModifiedDate { get; private set; }
        public virtual DateTimeOffset LastSavedDate { get; private set; }

        public virtual bool ShouldSave
        {
            get { return LastSavedDate < LastModifiedDate || LastSavedDate == default (DateTimeOffset); }
        }
    }


    public static class PersistanceHelperExtensions
    {
        public static void ResolveModified(this IPersistenceHelper persistenceHelper)
        {
            var helper = persistenceHelper;
            if (helper != null && helper.IsActive)
            {
                helper.SetModified();
            }
        }
    }
}
