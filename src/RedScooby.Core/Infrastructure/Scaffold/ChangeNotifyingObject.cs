// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using RedScooby.Helpers;

namespace RedScooby.Infrastructure.Scaffold
{
    public interface IChangeNotifyingObject : INotifyPropertyChanged
    {
        bool ChangeNotificationsEnabled { get; }

        PropertyChangedEventHandler CreateAttachedHandler(INotifyPropertyChanged senderInstance,
            KeyValuePair<string, string> senderToReceiverPropertyMap);

        PropertyChangedEventHandler CreateAttachedHandler(INotifyPropertyChanged senderInstance,
            IEnumerable<KeyValuePair<string, string>> senderToReceiverPropertyMap);

        void EnableChangeNotifications();
        void RefreshChangeNotifications();
        void SupressChangeNotifications();
    }

    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public abstract class ChangeNotifyingObject : IChangeNotifyingObject
    {
        [IgnoreDataMember] private volatile bool changeNotificationsEnabled;

        internal ChangeNotifyingObject()
        {
            changeNotificationsEnabled = true;
        }

        public void EnableChangeNotifications()
        {
            changeNotificationsEnabled = true;
        }

        public void RefreshChangeNotifications()
        {
            var handler = PropertyChanged;

            if (handler != null) handler.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        }

        public void SupressChangeNotifications()
        {
            changeNotificationsEnabled = false;
        }

        public PropertyChangedEventHandler CreateAttachedHandler(INotifyPropertyChanged senderInstance,
            KeyValuePair<string, string> senderToReceiverPropertyMap)
        {
            var attachedHandler = new PropertyChangedEventHandler((sender, args) =>
            {
                if (args.PropertyName == senderToReceiverPropertyMap.Key)
                    NotifyPropertyChanged(senderToReceiverPropertyMap.Value);
            });

            senderInstance.PropertyChanged += attachedHandler;

            return attachedHandler;
        }

        public PropertyChangedEventHandler CreateAttachedHandler(INotifyPropertyChanged senderInstance,
            IEnumerable<KeyValuePair<string, string>> senderToReceiverPropertyMap)
        {
            var attachedHandler = new PropertyChangedEventHandler((sender, args) =>
            {
                foreach (var keyValuePair in senderToReceiverPropertyMap)
                {
                    if (args.PropertyName == keyValuePair.Key)
                        NotifyPropertyChanged(keyValuePair.Value);
                }
            });

            senderInstance.PropertyChanged += attachedHandler;

            return attachedHandler;
        }

        [IgnoreDataMember]
        public bool ChangeNotificationsEnabled
        {
            get { return changeNotificationsEnabled; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected internal void ClearNotificationHandlers()
        {
            PropertyChanged = null;
        }

        /// <summary>
        ///     Gets the internal handler event.
        ///     <para>
        ///         WARNING: Leaky abstraction. Use with great care, and make sure its scope is handled correctly to avoid memory
        ///         leak.
        ///     </para>
        /// </summary>
        /// <returns>The internal property change handler</returns>
        protected internal PropertyChangedEventHandler GetPropertyChangeHandlerDelegate()
        {
            return PropertyChanged;
        }

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (changeNotificationsEnabled)
            {
                var handler = PropertyChanged;

                if (handler != null) handler.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected virtual void NotifyPropertyChanged<T>(Expression<Func<T>> expression)
        {
            NotifyPropertyChanged(ExpressionHelpers.GetMemberName(expression));
        }

        // ReSharper disable once RedundantAssignment
        protected virtual void SetAndNotify<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            property = value;
            NotifyPropertyChanged(propertyName);
        }

        protected virtual void SetAndNotify<T>(ref T property, T value, Expression<Func<T>> expression)
        {
            SetAndNotify(ref property, value, ExpressionHelpers.GetMemberName(expression));
        }

        protected virtual void SetAndNotifyIfChanged<T>(ref T property, T value, Action changedAction,
            [CallerMemberName] string propertyName = null)
        {
            if (Equals(value, property)) return;
            SetAndNotify(ref property, value, propertyName);
            if (changedAction != null) changedAction.Invoke();
        }

        protected virtual void SetAndNotifyIfChanged<T>(ref T property, T value, Action changedAction,
            Expression<Func<T>> expression)
        {
            SetAndNotifyIfChanged(ref property, value, changedAction, ExpressionHelpers.GetMemberName(expression));
        }

        protected void SetAndNotifyIfChanged<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            SetAndNotifyIfChanged(ref property, value, null, propertyName);
        }

        protected void SetAndNotifyIfChanged<T>(ref T property, T value, Expression<Func<T>> expression)
        {
            SetAndNotifyIfChanged(ref property, value, ExpressionHelpers.GetMemberName(expression));
        }
    }
}
