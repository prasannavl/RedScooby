// Author: Prasanna V. Loganathar
// Created: 9:23 PM 11-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using Windows.ApplicationModel.Chat;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Email;
using Windows.ApplicationModel.Store;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using RedScooby.Common.Resources;
using RedScooby.Helpers;
using RedScooby.Infrastructure.Framework.Commands;
using RedScooby.ViewModels;
using RedScooby.Views.Components;

namespace RedScooby.Views.Settings
{
    public class SettingsViewBase : UserControlView<WinRtSettingsViewModel> { }

    public sealed partial class SettingsView
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        public AppBar GetAppBar()
        {
            var shareButton = new AppBarButton()
            {
                Label = MenuStrings.ShareLink,
                Icon = new SymbolIcon(Symbol.ReShare),
                Command = CommandFactory.Create(() =>
                {
                    var shareUi = DataTransferManager.GetForCurrentView();

                    shareUi.DataRequested += (sender, args) =>
                    {
                        args.Request.Data.Properties.Title = CommonStrings.AppTitle;
                        args.Request.Data.Properties.Description = CommonStrings.AppShortDescription;
                        args.Request.Data.SetApplicationLink(new Uri(NeutralStrings.RedScoobyUri));
                        args.Request.Data.SetText(string.Format(CommonStrings.ShareShortMessage, CurrentApp.LinkUri));
                    };

                    DataTransferManager.ShowShareUI();
                })
            };

            var emailButton = new AppBarButton
            {
                Label = MenuStrings.EmailLink,
                Icon = new SymbolIcon(Symbol.Mail),
                Command = CommandFactory.CreateAsync(async () =>
                {
                    await EmailManager.ShowComposeNewEmailAsync(new EmailMessage
                    {
                        Subject = "RedScooby: " + CommonStrings.AppShortDescription,
                        Body =
                            string.Format(CommonStrings.ShareEmailMessage, CurrentApp.LinkUri,
                                Model.CoreViewModel.User.Name)
                    });
                })
            };


            var smsButton = new AppBarButton
            {
                Label = MenuStrings.SmsLink,
                Icon = new SymbolIcon(Symbol.Message),
                Command = CommandFactory.CreateAsync(async () =>
                {
                    await ChatMessageManager.ShowComposeSmsMessageAsync(new ChatMessage
                    {
                        Body = string.Format(CommonStrings.ShareShortMessage, CurrentApp.LinkUri),
                    });
                })
            };

            var rateButton = new AppBarButton
            {
                Label = MenuStrings.RateApp,
                Icon = new SymbolIcon(Symbol.Favorite),
                Command =
                    CommandFactory.CreateAsync(
                        async () =>
                        {
                            await
                                Launcher.LaunchUriAsync(
                                    new Uri("ms-windows-store:reviewapp?appid=" + CurrentApp.AppId));
                        })
            };

            var settingsBar = new CommandBar();
            settingsBar.PrimaryCommands.Add(emailButton);
            settingsBar.PrimaryCommands.Add(smsButton);
            settingsBar.PrimaryCommands.Add(shareButton);
            settingsBar.PrimaryCommands.Add(rateButton);

            return settingsBar;
        }

        private void WiFi_Click(object sender, RoutedEventArgs e)
        {
            Launcher.LaunchUriAsync(new Uri("ms-settings-wifi:"))
                .AsTask().ContinueWithErrorHandling();
        }

        private void Gps_Click(object sender, RoutedEventArgs e)
        {
            Launcher.LaunchUriAsync(new Uri("ms-settings-location:"))
                .AsTask().ContinueWithErrorHandling();
        }

        private void CellData_Click(object sender, RoutedEventArgs e)
        {
            Launcher.LaunchUriAsync(new Uri("ms-settings-cellular:"))
                .AsTask().ContinueWithErrorHandling();
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Guide screen
        }

        private void ShowBetaDialog(object sender, RoutedEventArgs e)
        {
            WindowHelpers.ShowOperationNotAvailableOnBetaDialogAsync()
                .ContinueWithErrorHandling();
        }
    }
}
