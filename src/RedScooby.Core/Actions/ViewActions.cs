// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;

namespace RedScooby.Actions
{
    public class ViewActions
    {
        public enum ActivityStatus
        {
            Success,
            InProgress,
            Failure,
        }

        public enum AssistView
        {
            TapAndHoldFocus,
            TapAndHoldRelease,
        }

        public enum Concern
        {
            TurnOn,
            TurnOff,
        }

        public enum Distress
        {
            TurnOn,
            TurnOff,
            ActivateCountdown,
            DeactivateCountdown,
            CategoryRequery,
        }

        public enum GlobalIndicators
        {
            ShowBusyIndicator,
            HideBusyIndicator,
        }

        public enum PinFeedback
        {
            Show,
            Hide,
        }

        public enum RootView
        {
            Lock,
            Unlock,
            FocusAssist,
        }
    }

    public class ViewMessage<T>
    {
        public ViewMessage(string message, MessageType type = MessageType.Info)
        {
            Message = message;
            Type = type;
        }

        public string Message { get; set; }
        public MessageType Type { get; set; }
    }

    [Flags]
    public enum MessageType
    {
        Info = 1 << 0,
        Error = 1 << 1,
        NonIntrusive = 1 << 2,
    }
}
