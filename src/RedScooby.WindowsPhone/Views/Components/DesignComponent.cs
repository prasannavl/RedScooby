// Author: Prasanna V. Loganathar
// Created: 9:20 PM 11-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Threading.Tasks;
using RedScooby.Infrastructure.Framework;

namespace RedScooby.Views.Components
{
    public static class DesignComponent
    {
        public static bool IsActive
        {
            get { return ModeDetector.IsInDesignMode; }
        }

        public static void InitializeApp()
        {
            if (IsActive)
            {
                if (App.Current == null)
                {
                    lock (typeof (DesignComponent))
                    {
                        var bootstrapper = ShellManager.CreateBootstrapper();
                        bootstrapper.RunAsync().Wait();
                    }
                }
            }
        }

        public static void InitializeComponent(Action action)
        {
            if (IsActive)
                action();
        }

        public static void PostXamlInitialize(Action action)
        {
            if (IsActive)
            {
                Task.Delay(100).ContinueWith(t => { DispatchHelper.Current.Run(action); });
            }
        }
    }
}
