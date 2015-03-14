// Author: Prasanna V. Loganathar
// Created: 9:48 PM 15-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.ApplicationModel.Email;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using RedScooby.Helpers;
using RedScooby.Infrastructure.Framework.Commands;
using RedScooby.Logging;

namespace RedScooby.Views.Settings
{
    public sealed partial class LogView
    {
        public LogView()
        {
            InitializeComponent();
            BottomAppBar = GetAppBar();
        }

        public AppBar GetAppBar()
        {
            var emailButton = new AppBarButton
            {
                Label = "report",
                Icon = new SymbolIcon(Symbol.MailForward),
                Command = CommandFactory.CreateAsync(async () =>
                {
                    var data = await FileSystemLogger.Current.GetSavedLogDataAsync(TitleTextBlock.Text, -1);
                    var bytes = Encoding.UTF8.GetBytes(data);
                    var stream = new InMemoryRandomAccessStream();
                    await stream.WriteAsync(WindowsRuntimeBuffer.Create(bytes, 0, bytes.Length, bytes.Length));

                    await EmailManager.ShowComposeNewEmailAsync(new EmailMessage
                    {
                        To = {new EmailRecipient("logparser@redscooby.com", "RedScooby Log Parser")},
                        Subject = "LogId: " + Guid.NewGuid(),
                        Attachments =
                        {
                            new EmailAttachment(Guid.NewGuid() + ".txt",
                                RandomAccessStreamReference.CreateFromStream(stream))
                        },
                    });
                })
            };

            var deleteButton = new AppBarButton
            {
                Label = "delete",
                Icon = new SymbolIcon(Symbol.Delete),
                Command =
                    CommandFactory.CreateAsync(
                        async () =>
                        {
                            await FileSystemLogger.Current.DeleteLogAsync(TitleTextBlock.Text);
                            WindowHelpers.GoBack();
                        })
            };


            var settingsBar = new CommandBar();
            settingsBar.PrimaryCommands.Add(emailButton);
            settingsBar.PrimaryCommands.Add(deleteButton);
            return settingsBar;
        }
    }
}
