// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using FluentValidation;
using FluentValidation.Results;
using RedScooby.Models;

namespace RedScooby.Validation
{
    public class UserValidator : AbstractValidator<UserModel>
    {
        public enum UserValidationRuleSet
        {
            RegistrationBasic,
            SecurityPinCodes
        }

        public UserValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleSet(UserValidationRuleSet.RegistrationBasic.ToString(), () =>
            {
                RuleFor(x => x.Name)
                    .NotEmpty()
                    .WithMessage("Please enter your full name.")
                    .Length(3, 200)
                    .WithMessage("Please enter a valid full name.");

                RuleFor(x => x.Email)
                    .NotEmpty()
                    .WithMessage("Please enter your email address.")
                    .EmailAddress()
                    .WithMessage("You email address seems to be invalid. Please enter a valid one.");

                RuleFor(x => x.DateOfBirth).NotEmpty()
                    .WithMessage("Please enter your date of birth.")
                    .LessThanOrEqualTo(DateTime.Now.Subtract(TimeSpan.FromDays(7*365)))
                    .WithMessage(
                        "Hey! Sorry, you have to be 7+ to use the app. Until then, you can ask your parent/guardian to activate it for you.")
                    .GreaterThanOrEqualTo(DateTime.Now.Subtract(TimeSpan.FromDays(150*365)))
                    .WithMessage("They say maturity grows with age. But your age is beyond our understanding.");
            });

            RuleSet(UserValidationRuleSet.SecurityPinCodes.ToString(), () =>
            {
                // Valid 4 digit codes.
                int temp;

                RuleFor(x => x.Settings.PinCode)
                    .NotEmpty()
                    .WithMessage("Please enter your PIN code.")
                    .Must(x => int.TryParse(x, out temp))
                    .WithMessage("The PIN code can only have numeric digits.")
                    .WithMessage("Please enter your PIN code.")
                    .Length(4)
                    .WithMessage("The PIN code should be 4-digits");

                RuleFor(x => x.Settings.DisasterPinCode)
                    .NotEmpty()
                    .WithMessage("Please enter your Disaster PIN code.")
                    .Must(x => int.TryParse(x, out temp))
                    .WithMessage("The Disaster PIN code can only have numeric digits.")
                    .Length(4)
                    .WithMessage("The Disaster PIN should be 4-digits");

                Custom(
                    x =>
                        x.Settings.DisasterPinCode == x.Settings.PinCode
                            ? new ValidationFailure("Security",
                                "Your secret PIN code and Disaster PIN code cannot be the same.")
                            : null);
            });
        }
    }
}
