// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using FluentValidation;
using FluentValidation.Results;
using RedScooby.Components;

namespace RedScooby.Validation
{
    internal class PhoneNumberValidator : AbstractValidator<string>
    {
        public PhoneNumberValidator(RegionManager regionManager)
        {
            var phoneNumberUtil = regionManager.GetPhoneNumberUtil();
            Custom(x =>
            {
                try
                {
                    var ph = phoneNumberUtil.Parse(x, string.Empty);
                    if (!phoneNumberUtil.IsValidNumberForRegion(ph, phoneNumberUtil.GetRegionCodeForNumber(ph)))
                        return new ValidationFailure("PhoneNumber", "Phone number is not valid for given country.");
                }
                catch
                {
                    return new ValidationFailure("PhoneNumber", "Phone number is not valid.");
                }

                return null;
            });
        }
    }
}
