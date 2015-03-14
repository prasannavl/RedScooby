// Author: Prasanna V. Loganathar
// Created: 6:48 PM 15-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using RedScooby.Helpers;
using RedScooby.Infrastructure.Framework;

namespace RedScooby.Infrastructure.Flashlight
{
    public class WinRtFlashlightService : IFlashlightService
    {
        private MediaCapture mediaCapture;
        private bool isReady;
        private TaskCompletionSource<bool> initTask = new TaskCompletionSource<bool>();
        private InterlockedMonitor monitor = new InterlockedMonitor();

        public void Dispose()
        {
            Release();
        }

        public async Task<bool> IsAvailableAsync()
        {
            await EnsureMediaCaptureInitializedAsync();
            return isReady;
        }

        public async Task TurnOff()
        {
            if (await IsAvailableAsync())
            {
                if (IsOn)
                {
                    IsOn = false;
                    try
                    {
                        var torch = mediaCapture.VideoDeviceController.TorchControl;
                        torch.Enabled = false;
                    }
                    catch
                    {
                        Release();
                    }
                }
            }
        }

        public async Task TurnOn()
        {
            if (await IsAvailableAsync())
            {
                if (!IsOn)
                {
                    IsOn = true;
                    try
                    {
                        var torch = mediaCapture.VideoDeviceController.TorchControl;
                        if (torch.PowerSupported) torch.PowerPercent = 100;
                        torch.Enabled = true;
                    }
                    catch
                    {
                        Release();
                        IsOn = false;
                    }
                }
            }
        }

        public void Release()
        {
            if (mediaCapture == null) return;
            if (!monitor.TryEnter())
            {
                initTask.Task.Wait();
            }
            try
            {
                var m = mediaCapture;
                if (initTask == null) initTask = new TaskCompletionSource<bool>();
                mediaCapture = null;
                m.Dispose();
            }
            finally
            {
                monitor.Exit();
            }
        }

        public bool IsFastSwitchingCapable
        {
            get { return true; }
        }

        public bool IsOn { get; private set; }

        ~WinRtFlashlightService()
        {
            Release();
        }

        public static async Task<DeviceInformationCollection> GetCameraDevices()
        {
            var devices = (await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture));
            return devices;
        }

        private async Task EnsureMediaCaptureInitializedAsync()
        {
            if (mediaCapture != null) return;

            if (!monitor.TryEnter())
            {
                var tcs = initTask;
                if (tcs != null)
                    await tcs.Task;
                return;
            }
            try
            {
                await DispatchHelper.Current.RunAsync(async () =>
                {
                    var devices = await GetCameraDevices();
                    foreach (var device in devices)
                    {
                        var mc = new MediaCapture();

                        var settings = new MediaCaptureInitializationSettings
                        {
                            StreamingCaptureMode = StreamingCaptureMode.Video,
                            PhotoCaptureSource = PhotoCaptureSource.Photo,
                            VideoDeviceId = device.Id,
                            AudioDeviceId = string.Empty,
                            AudioSource = null,
                        };

                        try
                        {
                            await mc.InitializeAsync(settings).AsTask();
                            if (mc.VideoDeviceController.TorchControl.Supported)
                            {
                                isReady = true;
                                mc.Failed += (sender, args) =>
                                {
                                    sender.Dispose();
                                    IsOn = false;
                                };

                                mediaCapture = mc;
                                return;
                            }
                            mc.Dispose();
                        }
                        catch (Exception ex)
                        {
                            ErrorHandler.Current.HandleError(ex);
                        }
                    }

                    var mcDisposed = new MediaCapture();
                    mcDisposed.Dispose();

                    isReady = false;
                    mediaCapture = mcDisposed;
                });
            }
            finally
            {
                var tcs = initTask;
                tcs.TrySetResult(true);
                initTask = null;
                monitor.Exit();
            }
        }
    }
}
