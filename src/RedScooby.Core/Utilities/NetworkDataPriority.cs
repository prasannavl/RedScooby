// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

namespace RedScooby.Utilities
{
    public enum NetworkDataPriority
    {
        Ignorable = -10,
        Low = -5,
        Normal = 0,
        High = 5,
        VeryHigh = 10,
        SkipQueue = 100
    }
}
