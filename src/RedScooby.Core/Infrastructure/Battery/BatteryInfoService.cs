// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;

namespace RedScooby.Infrastructure.Battery
{
    public interface IBatteryInfoService : IDisposable
    {
        IObservable<double> BatteryPercentChanges();
        double GetRemainingChargePercentage();
    }
}
