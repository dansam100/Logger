using System;
using System.Collections.Generic;
using System.Text;
using Logger.Core;

namespace Logger.Container.Ranks
{
    public class LogPlant : ILogPlant
    {
        public Logger GrowLogger(string name)
        {
            if (name != null)
            {
                return new Logger(name);
            }
            return new Logger(string.Intern("root"));
        }
    }

    internal sealed class LoggerImpl : Logger
    {
        internal LoggerImpl(string name)
            : base(name)
        { }
    }
}
