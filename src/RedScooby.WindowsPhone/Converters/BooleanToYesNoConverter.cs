// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

namespace RedScooby.Converters
{
    public class BooleanToYesNoConverter : BooleanToStringConverter
    {
        public BooleanToYesNoConverter() : base("Yes", "No") { }
    }
}
