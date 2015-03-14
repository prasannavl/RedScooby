// Author: Prasanna V. Loganathar
// Created: 6:48 PM 15-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using Ailon.WP.Utils;

namespace RedScooby.Infrastructure.Flashlight
{
    public class WinRtFlashlightFactory
    {
        public enum FlashlightCreationOptions
        {
            Default,
            ForceQuickFlashlight,
            ForceFallback,
        }

        public static IFlashlightService CreateForDevice(CanonicalPhoneName device,
            FlashlightCreationOptions creationOptions = FlashlightCreationOptions.Default)
        {
            if (creationOptions == FlashlightCreationOptions.ForceQuickFlashlight)
                return new FlashlightProxyService(new WinRtFlashlightService());
            if (creationOptions == FlashlightCreationOptions.ForceFallback)
                return new FlashlightProxyService(new WinRtFlashlightFallbackService());

            IFlashlightService realService = null;
            if (device.CanonicalManufacturer == "NOKIA")
            {
                if (device.CanonicalModel == "Lumia 925")
                    realService = new WinRtFlashlightService();
            }
            if (realService != null) return new FlashlightProxyService(realService);
            return new FlashlightProxyService(new WinRtFlashlightFallbackService());
        }
    }
}
