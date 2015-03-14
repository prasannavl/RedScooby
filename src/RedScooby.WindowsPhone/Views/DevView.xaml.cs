// Author: Prasanna V. Loganathar
// Created: 10:46 PM 24-11-2014
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using Windows.UI.Xaml.Controls;
using RedScooby.Logging;
using RedScooby.Logging.Core;
using RedScooby.Views.Core;

namespace RedScooby.Views
{

    public partial class DevView : UserControlView
    {
        private IDisposable subscription;

        public DevView()
        {
            InitializeComponent();
        }

        protected override void Dispose(bool isSuspending)
        {
            base.Dispose(isSuspending);
            Unsubscribe();
        }

        public void AddViewToPivot(Pivot sourcePivot)
        {
            if (sourcePivot == null) throw new ArgumentNullException("sourcePivot");

            if (sourcePivot.Items != null)
                sourcePivot.Items.Add(GetPivotItem());
        }

        public PivotItem GetPivotItem()
        {
            return new PivotItem {Content = this, Header = "log"};
        }

        public void SubscribeToEvents()
        {
            if (subscription != null) subscription.Dispose();
            subscription = SubscribeToEvents(LogManager.Events);
        }

        public void Unsubscribe()
        {
            if (subscription != null) subscription.Dispose();
            subscription = null;
        }

        private IDisposable SubscribeToEvents(IObservable<LogEvent> eventStream)
        {
            throw new NotImplementedException();
        }
    }
}