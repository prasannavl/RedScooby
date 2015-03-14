// Author: Prasanna V. Loganathar
// Created: 12:49 AM 19-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Email;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using RedScooby.Helpers;
using RedScooby.Logging.FormatProvider;

namespace RedScooby.Infrastructure.Framework
{
    public sealed class WinRtErrorHandler : ErrorHandlerBase
    {
        public const string AppCriticalExitExceptionPrefix = "C_EXIT";
        public const string AppUnobservedTaskExceptionPrefix = "TASK_EXP";
        private readonly SemaphoreSlim limiter = new SemaphoreSlim(1, 1);

        public override void HandleError(Exception ex, string logPrefix = null, bool shouldQuit = false)
        {
            base.HandleError(ex, logPrefix, false);

            var sb = new StringBuilder();
            if (!shouldQuit)
                sb.AppendLine(
                    "This doesn't look like a serious error, but it is still recommended that you quit, and open the app again. You may still choose to continue at your own risk without exiting by pressing the \"Back\" button.\r\n");

            sb.AppendLine("Error message: " + ex.Message);
            if (ex.InnerException != null)
                sb.AppendLine("\r\nInner error: " + ex.InnerException.Message);

            if (shouldQuit)
                sb.AppendLine("\r\nThe application will now exit.");

            var dialog = new MessageDialog(sb.ToString(), shouldQuit
                ? "Something went terribly wrong."
                : "Something isn't quite right.");

            DispatchHelper.Current.Run(async () =>
            {
                var exitCommand = new UICommand("Exit", command =>
                {
                    limiter.Release();

                    if (shouldQuit)
                        App.Current.ApplicationExitDelegate.InvokeIfNotNullWith(true, false);
                });

                var traceCommand = new UICommand("Trace", async command =>
                {
                    var traceMsgBuilder = new StringBuilder();
                    traceMsgBuilder.AppendLine(ex.Message);
                    traceMsgBuilder.AppendLine("\r\n" + ex.StackTrace);
                    if (ex.InnerException != null)
                    {
                        traceMsgBuilder.AppendLine();
                        traceMsgBuilder.AppendLine("Inner error:");
                        traceMsgBuilder.AppendLine("\r\n" + ex.InnerException.Message);
                        traceMsgBuilder.AppendLine("\r\n" + ex.InnerException.StackTrace);
                    }

                    var d = new MessageDialog(traceMsgBuilder.ToString(), "Error Trace");

                    var reportCommand = new UICommand("Report", async command2 =>
                    {
                        var sw = new StringWriter();
                        DumpFormatProvider.Dump(new StringWriter(), ex);
                        var bytes = Encoding.UTF8.GetBytes(sw.ToString());
                        var stream = new InMemoryRandomAccessStream();
                        await stream.WriteAsync(WindowsRuntimeBuffer.Create(bytes, 0, bytes.Length, bytes.Length));
                        await EmailManager.ShowComposeNewEmailAsync(new EmailMessage
                        {
                            To = {new EmailRecipient("logparser@redscooby.com", "RedScooby Log Parser")},
                            Subject = "ExceptionId: " + Guid.NewGuid(),
                            Attachments =
                            {
                                new EmailAttachment(Guid.NewGuid() + ".txt",
                                    RandomAccessStreamReference.CreateFromStream(stream))
                            },
                        });
                    });

                    d.Commands.Insert(0, reportCommand);
                    d.Commands.Insert(1, exitCommand);
                    d.CancelCommandIndex = 1;

                    limiter.Release();
                    await limiter.WaitAsync();

                    await d.ShowAsync();

                    if (shouldQuit)
                    {
                        ShellManager.Current.PrepareForExitOnError();
                    }
                });

                dialog.Commands.Insert(0, exitCommand);
                dialog.Commands.Insert(1, traceCommand);
                dialog.CancelCommandIndex = 0;

                await limiter.WaitAsync();
                await dialog.ShowAsync();

                if (shouldQuit)
                {
                    ShellManager.Current.PrepareForExitOnError();
                }
            });
        }

        public void HandleUnobservedTaskException(UnobservedTaskExceptionEventArgs eventArgs)
        {
            HandleError(eventArgs.Exception, AppUnobservedTaskExceptionPrefix, false);
            eventArgs.SetObserved();
        }

        public void HandleCrashException(UnhandledExceptionEventArgs eventArgs)
        {
            HandleError(eventArgs.Exception, AppCriticalExitExceptionPrefix, true);
            eventArgs.Handled = true;
        }
    }
}
