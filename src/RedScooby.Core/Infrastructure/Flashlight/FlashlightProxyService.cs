// Author: Prasanna V. Loganathar
// Created: 6:48 PM 15-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Threading.Tasks;
using RedScooby.Helpers;

namespace RedScooby.Infrastructure.Flashlight
{
    public class FlashlightProxyService : IFlashlightService
    {
        private readonly HybridInterlockedMonitor monitor = new HybridInterlockedMonitor();
        private IFlashlightService service;

        public FlashlightProxyService(IFlashlightService service)
        {
            this.service = service;
        }

        public IFlashlightService InnerService
        {
            get
            {
                monitor.Enter();
                var s = service;
                monitor.Exit();
                return s;
            }
        }

        public void Dispose()
        {
            InnerService.Dispose();
        }

        public Task<bool> IsAvailableAsync()
        {
            return InnerService.IsAvailableAsync();
        }

        public void Release()
        {
            InnerService.Release();
        }

        public Task TurnOff()
        {
            return InnerService.TurnOff();
        }

        public Task TurnOn()
        {
            return InnerService.TurnOn();
        }

        public bool IsFastSwitchingCapable
        {
            get { return InnerService.IsFastSwitchingCapable; }
        }

        public bool IsOn
        {
            get { return InnerService.IsOn; }
        }

        public async Task SwapService(IFlashlightService newService, Func<IFlashlightService, Task> preSwapFunc,
            Func<IFlashlightService, Task> postSwapFunc)
        {
            await monitor.EnterAsync();
            try
            {
                if (preSwapFunc != null)
                    await preSwapFunc(service);
                SwapServiceUnsafe(newService);
                if (postSwapFunc != null)
                    await postSwapFunc(service);
            }
            finally
            {
                monitor.Exit();
            }
        }

        public void SwapService(IFlashlightService newService)
        {
            monitor.Enter();
            try
            {
                SwapServiceUnsafe(newService);
            }
            finally
            {
                monitor.Exit();
            }
        }

        private void SwapServiceUnsafe(IFlashlightService newService)
        {
            if (service != null)
            {
                service.Release();
                service.Dispose();
            }

            service = newService;
        }
    }
}
