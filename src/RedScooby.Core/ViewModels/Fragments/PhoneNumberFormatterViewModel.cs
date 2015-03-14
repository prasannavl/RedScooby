// Author: Prasanna V. Loganathar
// Created: 9:30 PM 11-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.Generic;
using PhoneNumbers;
using RedScooby.Components;
using RedScooby.Infrastructure.Composition;
using RedScooby.Logging;

namespace RedScooby.ViewModels.Fragments
{
    public class PhoneNumberFormatterViewModel : ViewModelBase
    {
        private readonly PhoneNumberUtil phoneNumberUtil;
        private PhoneNumberFormat format;
        private AsYouTypeFormatter formatter;
        private string phoneNumber;
        private string region;

        public PhoneNumberFormatterViewModel(RegionManager regionManager)
        {
            phoneNumberUtil = regionManager.GetPhoneNumberUtil();
            format = PhoneNumberFormat.RFC3966;
            formatter = null;
        }

        public string Region
        {
            get { return region; }
            set
            {
                SetAndNotifyIfChanged(ref region, value, () =>
                {
                    formatter = phoneNumberUtil.GetAsYouTypeFormatter(region);
                    SetPhoneNumber(PhoneNumber);
                });
            }
        }

        public string PhoneNumber
        {
            get { return phoneNumber; }
            set { SetPhoneNumber(value); }
        }

        public PhoneNumberFormat Format
        {
            get { return format; }
            set { SetAndNotifyIfChanged(ref format, value, () => SetPhoneNumber(PhoneNumber)); }
        }

        private void SetPhoneNumber(string input)
        {
            if (formatter != null && input != null && input.Length > 3)
            {
                try
                {
                    var number = phoneNumberUtil.Parse(input, Region);

                    var formattedNumber = phoneNumberUtil
                        .FormatByPattern(number, Format, new List<NumberFormat>())
                        .Remove(0, 4);

                    if (input != formattedNumber)
                        Log.Info("Number reformatted [{2}]: {0} => {1}", input, formattedNumber, Region);

                    SetAndNotifyIfChanged(ref phoneNumber, formattedNumber, () => PhoneNumber);
                }
                catch (Exception ex)
                {
                    Log.Warn(f => f("Phone input: {0}, Exception: {1}", input, ex));
                    SetAndNotifyIfChanged(ref phoneNumber, input, () => PhoneNumber);
                }
            }
            else
            {
                SetAndNotifyIfChanged(ref phoneNumber, input, () => PhoneNumber);
            }
        }
    }
}
