// Author: Prasanna V. Loganathar
// Created: 11:57 PM 18-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;

namespace RedScooby.Infrastructure.Framework
{
    public interface IErrorHandler
    {
        void HandleError(Exception ex, string logPrefix = null, bool shouldQuit = false);
        void HandleSilentError(Exception ex, string logPrefix = null);
    }
}
