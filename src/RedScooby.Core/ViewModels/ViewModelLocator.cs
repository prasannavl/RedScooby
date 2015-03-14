// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Reactive.Disposables;
using Grace.DependencyInjection;
using RedScooby.ViewModels.Around;
using RedScooby.ViewModels.Assistance;
using RedScooby.ViewModels.Fragments;
using RedScooby.ViewModels.Settings;
using RedScooby.ViewModels.Setup;

namespace RedScooby.ViewModels
{
    public class ViewModelLocator
    {
        public static IInjectionScope Scope { get; protected internal set; }

        public UserSetupViewModel UserSetupViewModel
        {
            get { return Scope.Locate<UserSetupViewModel>(); }
        }

        public ConcernActiveViewModel ConcernActiveViewModel
        {
            get { return Scope.Locate<ConcernActiveViewModel>(); }
        }

        public RegistrationViewModel RegistrationViewModel
        {
            get { return Scope.Locate<RegistrationViewModel>(); }
        }

        public LoginViewModel LoginViewModel
        {
            get { return Scope.Locate<LoginViewModel>(); }
        }

        public PhoneNumberFormatterViewModel PhoneNumberFormatterViewModel
        {
            get { return Scope.Locate<PhoneNumberFormatterViewModel>(); }
        }

        public ConcernControlViewModel ConcernControlViewModel
        {
            get { return Scope.Locate<ConcernControlViewModel>(); }
        }

        public DistressControlViewModel DistressControlViewModel
        {
            get { return Scope.Locate<DistressControlViewModel>(); }
        }

        public DistressCountdownViewModel DistressCountdownViewModel
        {
            get { return Scope.Locate<DistressCountdownViewModel>(); }
        }

        public DistressActiveViewModel DistressActiveViewModel
        {
            get { return Scope.Locate<DistressActiveViewModel>(); }
        }

        public AssistanceViewModel AssistanceViewModel
        {
            get { return Scope.Locate<AssistanceViewModel>(); }
        }

        public PinFeedbackViewModel PinFeedbackViewModel
        {
            get { return Scope.Locate<PinFeedbackViewModel>(); }
        }

        public FlashlightViewModel FlashlightViewModel
        {
            get { return Scope.Locate<FlashlightViewModel>(); }
        }

        public SirenViewModel SirenViewModel
        {
            get { return Scope.Locate<SirenViewModel>(); }
        }

        public AroundMeViewModel AroundMeViewModel
        {
            get { return Scope.Locate<AroundMeViewModel>(); }
        }

        public SettingsViewModel SettingsViewModel
        {
            get { return Scope.Locate<SettingsViewModel>(); }
        }

        public static IDisposable BeginScope(ExportRegistrationDelegate @delegate = null, string scopeName = null,
            IDisposalScopeProvider disposalScopeProvider = null)
        {
            var scope = Scope.CreateChildScope(@delegate, scopeName, disposalScopeProvider);
            Scope = scope;
            return GetReversionDisposableForScope(scope);
        }

        public static IDisposable GetReversionDisposableForScope(IInjectionScope scope)
        {
            return Disposable.Create(() =>
            {
                if (Scope == scope)
                    RevertScope();
                else
                {
                    throw new InvalidOperationException("Scope has been changed and/or disposed out of order");
                }
            });
        }

        public static void RevertScope()
        {
            var parentScope = Scope.ParentScope;
            if (parentScope != null)
            {
                var current = Scope;
                Scope = parentScope;
                current.Dispose();
            }
        }
    }
}
