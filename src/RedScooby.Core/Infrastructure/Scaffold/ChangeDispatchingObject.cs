// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System.ComponentModel;
using System.Runtime.CompilerServices;
using RedScooby.Infrastructure.Framework;

namespace RedScooby.Infrastructure.Scaffold
{
    public abstract class ChangeDispatchingObject : ChangeNotifyingObject
    {
        protected override void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (ChangeNotificationsEnabled)
            {
                var handler = GetPropertyChangeHandlerDelegate();

                if (handler != null)
                {
                    DispatchHelper.Current.Run(() => handler(this, new PropertyChangedEventArgs(propertyName)));
                }
            }
        }
    }
}
