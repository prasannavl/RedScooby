// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Background;
using RedScooby.Helpers;
using RedScooby.Logging;
using RedScooby.WindowsPhone.Protected;

namespace RedScooby.Utilities
{
    internal class TaskRegistrations
    {
        public void Run()
        {
            RunInternal();
        }

        private void RunInternal()
        {
            BackgroundExecutionManager.RequestAccessAsync().AsTask()
                .ContinueWith(t =>
                {
                    t.HandleError();

                    if (t.Result == BackgroundAccessStatus.Denied)
                    {
                        Log.Error("Backgrond tasks registration access denied.");
                        return;
                    }

                    var list = new List<TaskCreator>
                    {
                        new TaskCreator(typeof (LocationUpdateTask),
                            new TimeTrigger(15, false),
                            new[] {new SystemCondition(SystemConditionType.InternetAvailable)}),
                    };

                    BackgroundTaskRegistration.AllTasks.ForEach(x => x.Value.Unregister(false));

                    list.ForEach(x =>
                    {
                        var taskState = x.Register();
                        Log.Trace(f => f("Registered: {0}", taskState));
                    });
                })
                .ContinueWithErrorHandling();
        }

        private class TaskCreator
        {
            public readonly string Name;
            public readonly string EntryPoint;
            public readonly IBackgroundTrigger Trigger;
            public readonly IEnumerable<IBackgroundCondition> Conditions;
            public readonly bool CancelOnConditionLoss;

            public TaskCreator(Type taskType, IBackgroundTrigger trigger, IEnumerable<IBackgroundCondition> conditions,
                bool cancelOnConditionLoss = false)
            {
                Name = taskType.Name;
                EntryPoint = taskType.FullName;
                Trigger = trigger;
                Conditions = conditions;
                CancelOnConditionLoss = cancelOnConditionLoss;
            }

            public BackgroundTaskBuilder GetBuilder()
            {
                var builder = new BackgroundTaskBuilder {Name = Name, TaskEntryPoint = EntryPoint};
                if (Trigger != null)
                    builder.SetTrigger(Trigger);
                Conditions.ForEach(x => builder.AddCondition(x));

                if (Conditions.Any())
                    builder.CancelOnConditionLoss = CancelOnConditionLoss;

                return builder;
            }

            public IBackgroundTaskRegistration Register()
            {
                return GetBuilder().Register();
            }
        }
    }
}
