// Author: Prasanna V. Loganathar
// Created: 9:23 PM 11-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using RedScooby.Helpers;
using RedScooby.Infrastructure.Framework;
using RedScooby.ViewModels.Around;
using RedScooby.Views.Components;

namespace RedScooby.Views.Around
{
    public class AroundMeViewBase : UserControlView<AroundMeViewModel> { }

    public sealed partial class AroundMeView
    {
        private readonly PropertyChangedEventHandler geoPosUpdateHandler;
        private int isViewportChangesInProgress;

        public AroundMeView()
        {
            InitializeComponent();
            GlanceMapControl.MapServiceToken = WindowsPhone.Protected.Resources.MapStoreToken;

            var propName = ExpressionHelpers.GetMemberName(() => Model.LastKnownLocation);
            geoPosUpdateHandler = (sender, args) =>
            {
                if (args.PropertyName == propName)
                {
                    var ignore = UpdateMapAsync();
                }
            };
        }

        public void EnableView()
        {
            var _ = Refresh()
                .ContinueWithErrorHandling();

            Model.PropertyChanged += geoPosUpdateHandler;
        }

        public void SuppressView()
        {
            Model.PropertyChanged -= geoPosUpdateHandler;
            Model.StopListeningForGeoUpdates();
        }

        public AppBar GetAppBar()
        {
            var appBar = new CommandBar();
            appBar.PrimaryCommands.Add(new AppBarButton {Icon = new SymbolIcon(Symbol.Add), Label = "add"});
            appBar.PrimaryCommands.Add(new AppBarButton {Icon = new SymbolIcon(Symbol.Find), Label = "search"});
            return appBar;
        }

        protected override void OnUnloaded()
        {
            SuppressView();
            base.OnUnloaded();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            Refresh(true)
                .ContinueWithErrorHandling();
        }

        private async Task Refresh(bool force = false)
        {
            if (Interlocked.CompareExchange(ref isViewportChangesInProgress, 1, 0) == 0)
            {
                try
                {
                    Model.StartListeningForGeoUpdates();
                    await Model.EnsureRecentAddress();
                    await UpdateMapAsync(true);
                }
                finally
                {
                    Interlocked.Exchange(ref isViewportChangesInProgress, 0);
                }
            }
        }

        private async Task UpdateMapAsync(bool lockTaken = false)
        {
            if (lockTaken || Interlocked.CompareExchange(ref isViewportChangesInProgress, 1, 0) == 0)
            {
                try
                {
                    await DispatchHelper.Current.RunAsync(async () =>
                    {
                        var loc = Model.LastKnownLocation;
                        await GlanceMapControl.TrySetViewAsync(new Geopoint(new BasicGeoposition
                        {
                            Latitude = loc.Latitude,
                            Longitude = loc.Longitude,
                        }), 14, null, null, MapAnimationKind.Bow);
                    });
                }
                finally
                {
                    if (!lockTaken) Interlocked.Exchange(ref isViewportChangesInProgress, 0);
                }
            }
        }
    }
}
