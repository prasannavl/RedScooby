// Author: Prasanna V. Loganathar
// Created: 12:01 AM 19-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using RedScooby.Logging;

namespace RedScooby.Infrastructure.Framework
{
    public class ErrorHandler
    {
        public static IErrorHandler Current { get; private set; }

        public static void Initialize(IErrorHandler handler)
        {
            Current = handler;
        }
    }

    public abstract class ErrorHandlerBase : IErrorHandler
    {
        public virtual void HandleSilentError(Exception ex, string logPrefix = null)
        {
            var message = ResolvePrefix(logPrefix);
            Log.Error(f => f(message, ex));
        }

        public virtual void HandleError(Exception ex, string logPrefix = null, bool shouldQuit = false)
        {
            var message = ResolvePrefix(logPrefix);
            Log.Critical(f => f(message, ex));
        }

        private static string ResolvePrefix(string logPrefix)
        {
            string message;
            if (logPrefix == null)
                message = "{0}";
            else
            {
                message = logPrefix + " => {0}";
            }
            return message;
        }
    }
}
