// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using RedScooby.Actions;
using RedScooby.Helpers;
using RedScooby.ViewModels;
using RedScooby.Views.Around;
using RedScooby.Views.Circles;
using RedScooby.Views.Components;
using RedScooby.Views.Settings;

namespace RedScooby.Views
{
    public class MainViewBase : PageView<MainViewModel> { }

    public sealed partial class MainView : MainViewBase
    {
        private PivotItem lastSelectedPivotItem;

        public MainView()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
            lastSelectedPivotItem = (PivotItem) RootPivot.SelectedItem;
            RootPivot.SelectionChanged += RootPivotOnSelectionChanged;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Dispatcher.RunIdleAsync(e => { AroundMeViewHolder.Activate(); });
            Dispatcher.RunIdleAsync(e => { CirclesViewHolder.Activate(); });
            Dispatcher.RunIdleAsync(e => { SettingsViewHolder.Activate(); });
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();

            SubscribeTo(ViewActions.RootView.FocusAssist,
                () => { RootPivot.SelectedItem = AssistPivotItem; });

            SubscribeTo(ViewActions.RootView.Lock, () => { RootPivot.IsLocked = true; });
            SubscribeTo(ViewActions.RootView.Unlock, () => { RootPivot.IsLocked = false; });

            var width = ActualWidth;
            StatusPopOut.HorizontalOffset = width;
        }

        private void RootPivotOnSelectionChanged(object sender,
            SelectionChangedEventArgs selectionChangedEventArgs)
        {
            var pivot = (PivotItem) RootPivot.SelectedItem;
            if (pivot == AroundMePivotItem)
            {
                AroundMeViewHolder.Activate();
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    var content = AroundMeViewHolder.ContentTemplateRoot;
                    if (content != null)
                    {
                        var view = (AroundMeView) content;
                        ShowAppBar(view.GetAppBar());
                        view.EnableView();
                    }
                });
            }
            else if (pivot == SettingsPivotItem)
            {
                SettingsViewHolder.Activate();
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    var content = SettingsViewHolder.ContentTemplateRoot;
                    if (content != null)
                    {
                        var view = (SettingsView) content;
                        ShowAppBar(view.GetAppBar());
                    }
                });
            }
            else if (pivot == CirclesPivotItem)
            {
                CirclesViewHolder.Activate();
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    var content = CirclesViewHolder.ContentTemplateRoot;
                    if (content != null)
                    {
                        var view = (CirclesView) content;
                        ShowAppBar(view.GetAppBar());
                    }
                });
            }
            else
            {
                HideAppBar();
            }


            if (lastSelectedPivotItem == AroundMePivotItem)
            {
                var content = AroundMeViewHolder.ContentTemplateRoot;
                if (content != null)
                {
                    var aroundView = (AroundMeView) content;
                    aroundView.SuppressView();
                }
            }

            lastSelectedPivotItem = pivot;
        }

        #region TestMethods

        public bool IsStatusActive { get; set; }

        public async Task HideStatus()
        {
            await ShowStatusStoryboard.RunReverseAsync();
            StatusPopOut.IsOpen = false;
        }

        public async Task ShowStatus()
        {
            StatusPopOut.IsOpen = true;
            await ShowStatusStoryboard.RunAsync();
        }

        private void ScheduleShowStatus(TimeSpan after)
        {
            Observable.Timer(after)
                .ObserveOnDispatcher()
                .Subscribe(async x =>
                {
                    await ShowStatus();
                    ScheduleHideStatus(TimeSpan.FromSeconds(4));
                });
        }

        private void ScheduleHideStatus(TimeSpan after)
        {
            Observable.Timer(after)
                .ObserveOnDispatcher()
                .Subscribe(async x2 =>
                {
                    await HideStatus();
                    ScheduleShowStatus(TimeSpan.FromSeconds(3));
                });
        }

        #endregion
    }
}
