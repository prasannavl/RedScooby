// Author: Prasanna V. Loganathar
// Created: 7:31 PM 15-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;

namespace RedScooby.Logging.Core
{
    public class PrefixedLogger : LoggerFacade
    {
        private readonly string prefix;

        internal PrefixedLogger(string prefix, LoggerBase logger) : base(logger)
        {
            if (string.IsNullOrEmpty(prefix))
                throw new ArgumentException("prefix");

            this.prefix = prefix;
        }

        public override void Execute(LogLevel level, string message)
        {
            Logger.Execute(level, String.Concat(prefix, message));
        }
    }

    public sealed class ContexualPrefixLogger : PrefixedLogger
    {
        public ContexualPrefixLogger(string prefix, LoggerBase logger) : base(prefix + ": ", logger) { }
    }
}
