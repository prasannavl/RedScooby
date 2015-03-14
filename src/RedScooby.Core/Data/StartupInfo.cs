// Author: Prasanna V. Loganathar
// Created: 6:43 AM 05-03-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

namespace RedScooby.Data
{
    public class StartupInfo
    {
        public int UserId { get; set; }
        public bool IsFirstRun { get; set; }
        public bool IsUserAuthenticated { get; set; }
        public bool HasActiveAssistState { get; set; }
    }
}
