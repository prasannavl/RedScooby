// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace RedScooby.Infrastructure.Battery
{
    public class WindowsPhoneBatteryInfoService : IBatteryInfoService
    {
        private IObservable<double> batteryChangesObservable;
        private Windows.Phone.Devices.Power.Battery current;

        private Windows.Phone.Devices.Power.Battery Current
        {
            get { return current ?? (current = Windows.Phone.Devices.Power.Battery.GetDefault()); }
        }

        public double GetRemainingChargePercentage()
        {
            return Current.RemainingChargePercent;
        }

        public IObservable<double> BatteryPercentChanges()
        {
            return batteryChangesObservable ?? (batteryChangesObservable = CreateBatteryChangesObservable());
        }

        public void Dispose() { }

        public IObservable<double> CreateBatteryChangesObservable()
        {
            var bInfo = Windows.Phone.Devices.Power.Battery.GetDefault();
            return Observable.FromEventPattern<object>(
                h => bInfo.RemainingChargePercentChanged += h,
                h => bInfo.RemainingChargePercentChanged -= h, TaskPoolScheduler.Default)
                .Select(x => (double) bInfo.RemainingChargePercent);
        }
    }
}
