// Author: Prasanna V. Loganathar
// Created: 9:16 PM 11-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using RedScooby.Actions;
using RedScooby.Common;
using RedScooby.Components;
using RedScooby.Infrastructure.Composition;
using RedScooby.Infrastructure.Framework.Commands;

namespace RedScooby.ViewModels.Assistance
{
    public class DistressActiveViewModel : ViewModelBase
    {
        private readonly DistressManager distressManager;
        private IDisposable autoCategorySubscription;
        private ObservableCollection<DistressNotificationOptionDetail> notificationOptions;
        private DistressNotificationOptionDetail notificationPreference;

        public DistressActiveViewModel(DistressManager distressManager)
        {
            this.distressManager = distressManager;
            EndDistressAsyncCommand = CommandFactory.CreateAsync(distressManager.RequestEndDistress);

            SubscribeTo(ViewModelActions.Distress.CategoryRequery,
                () => ViewMessenger.Send(ViewActions.Distress.CategoryRequery));

            SetupNotificationPreferences();

            NotificationPreference =
                NotificationOptions.First(x => x.OptionTitle == distressManager.CurrentNotificationPreference);

            var startTime = DateTimeOffset.Now;

            autoCategorySubscription =
                Observable.Timer(startTime.Add(TimeSpan.FromSeconds(2)))
                    .Subscribe(t =>
                    {
                        if (distressManager.DesiredCategory == DistressCategory.General)
                            ToggleCategory(DistressCategory.Defence);
                    });

            AddDisposable(autoCategorySubscription);
        }

        public AsyncRelayCommand EndDistressAsyncCommand { get; private set; }

        public ObservableCollection<DistressNotificationOptionDetail> NotificationOptions
        {
            get { return notificationOptions; }
            set { SetAndNotifyIfChanged(ref notificationOptions, value); }
        }

        public DistressNotificationOptionDetail NotificationPreference
        {
            get { return notificationPreference; }
            set
            {
                SetAndNotifyIfChanged(ref notificationPreference, value,
                    () => { distressManager.SetNotificationPreference(value.OptionTitle); });
            }
        }

        public ActivityState GetCategoryState(DistressCategory category)
        {
            return distressManager.GetCategoryState(category).State;
        }

        public void DisposeAutoCategorySelection()
        {
            if (autoCategorySubscription != null)
            {
                autoCategorySubscription.Dispose();
                autoCategorySubscription = null;
            }
        }

        public void ToggleCategory(DistressCategory category)
        {
            DisposeAutoCategorySelection();
            var _ = distressManager.ToggleCategory(category);
        }

        public void SetupNotificationPreferences()
        {
            // TODO: Move strings to resources.

            notificationOptions = new ObservableCollection<DistressNotificationOptionDetail>
            {
                new DistressNotificationOptionDetail
                {
                    OptionDescription = "Smart notifications (Recommended)",
                    OptionTitle = DistressNotificationPreference.Auto
                },
                new DistressNotificationOptionDetail
                {
                    OptionDescription = "My inner circles only",
                    OptionTitle = DistressNotificationPreference.Circles
                },
                new DistressNotificationOptionDetail
                {
                    OptionDescription = "Nearest professionals only",
                    OptionTitle = DistressNotificationPreference.Professionals
                },
                new DistressNotificationOptionDetail
                {
                    OptionDescription = "My inner circles and nearest professionals",
                    OptionTitle = DistressNotificationPreference.CirclesAndProfessionals
                },
            };
        }
    }

    public class DistressNotificationOptionDetail
    {
        public DistressNotificationPreference OptionTitle { get; set; }
        public string OptionDescription { get; set; }
    }
}
