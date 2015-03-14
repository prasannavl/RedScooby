// Author: Prasanna V. Loganathar
// Created: 6:06 AM 17-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System.Threading.Tasks;

namespace RedScooby.Infrastructure.Framework.Commands
{
    public class AsyncCommandResultDispatcher
    {
        public static void DispatchResult(Task task, bool handleError = true)
        {
            if (handleError && task.IsFaulted)
            {
                ErrorHandler.Current.HandleError(task.Exception);
            }
        }
    }
}
