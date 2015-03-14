// Author: Prasanna V. Loganathar
// Created: 1:44 PM 27-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using RedScooby.Infrastructure.Framework;
using RedScooby.Views.Components;

namespace RedScooby.Helpers
{
    public static class WindowHelpers
    {
        public static object GetCurrentView()
        {
            var rootPage = Window.Current.Content;
            var frame = rootPage as Frame;
            if (frame != null)
            {
                var page = frame.Content;
                if (page != null)
                {
                    return page;
                }
            }
            return rootPage;
        }

        public static bool GoBack()
        {
            var rootPage = Window.Current.Content;
            var frame = rootPage as Frame;
            if (frame == null) return false;
            if (!frame.CanGoBack) return false;

            DispatchHelper.Current.Run(() => frame.GoBack());
            return true;
        }

        public static async Task<bool> NavigateAsync(Type type)
        {
            if (DispatchHelper.Current.CheckAccess())
            {
                var rootPage = Window.Current.Content;
                var frame = rootPage as Frame;
                return frame != null && frame.Navigate(type);
            }
            else
            {
                var tcs = new TaskCompletionSource<bool>();
                DispatchHelper.Current.Dispatch(() =>
                {
                    var rootPage = Window.Current.Content;
                    var frame = rootPage as Frame;
                    tcs.TrySetResult(frame != null && frame.Navigate(type));
                });
                return await tcs.Task.ConfigureAwait(false);
            }
        }

        public static async Task<T> NavigateToAsync<T>() where T : PageBase
        {
            if (DispatchHelper.Current.CheckAccess())
            {
                var rootPage = Window.Current.Content;
                var frame = rootPage as Frame;
                if (frame != null && frame.Navigate(typeof (T)))
                {
                    return frame.Content as T;
                }
                return null;
            }

            var tcs = new TaskCompletionSource<T>();
            DispatchHelper.Current.Dispatch(() =>
            {
                var rootPage = Window.Current.Content;
                var frame = rootPage as Frame;
                if (frame != null && frame.Navigate(typeof (T)))
                {
                    tcs.TrySetResult(frame.Content as T);
                    return;
                }
                tcs.TrySetResult(null);
            });
            return await tcs.Task.ConfigureAwait(false);
        }

        public static async Task<bool> NavigateToWithModelContext<T>(object context) where T : PageBase
        {
            var page = await NavigateToAsync<T>();
            if (page == null) return false;
            page.SetModelContext(context);
            return true;
        }

        public static async Task<bool> NavigateWithDataContextAsync(Type type, object dataContext)
        {
            var rootPage = Window.Current.Content;
            var frame = rootPage as Frame;
            if (frame == null) return false;

            if (DispatchHelper.Current.CheckAccess())
            {
                if (!frame.Navigate(type)) return false;
                var page = frame.Content as PageBase;
                if (page == null) return false;
                page.DataContext = dataContext;
                return true;
            }

            var tcs = new TaskCompletionSource<bool>();
            DispatchHelper.Current.Dispatch(() =>
            {
                if (frame.Navigate(type))
                {
                    var page = frame.Content as PageBase;
                    if (page != null)
                    {
                        page.DataContext = dataContext;
                        tcs.TrySetResult(true);
                        return;
                    }
                }

                tcs.TrySetResult(false);
            });

            return await tcs.Task.ConfigureAwait(false);
        }

        public static async Task ShowMessageDialog(string message, string title = null, string dismissText = "close")
        {
            var dialog = title == null ? new MessageDialog(message) : new MessageDialog(message, title);
            if (dismissText != null)
            {
                dialog.Commands.Add(new UICommand(dismissText));
            }
            if (DispatchHelper.Current.CheckAccess())
                await dialog.ShowAsync().AsTask().ConfigureAwait(false);
            else
                await DispatchHelper.Current.DispatchAsync(dialog.ShowAsync().AsTask).ConfigureAwait(false);
        }

        public static async Task ShowOperationNotAvailableOnBetaDialogAsync()
        {
            var message =
                "This feature is not operational during the current beta phase. We're working on getting this feature to you as soon as possible.";
            await ShowMessageDialog(message, "Feature disabled").ConfigureAwait(false);
        }
    }
}
