using System;
using System.Collections.Generic;
using System.Text;

namespace Logger.Core
{
    /// <summary>
    /// Wrapper class for Logger.
    /// </summary>
    class LoggerWrapperImpl : ILoggerWrapper
    {
        //fields
        private readonly ILogger v_logger;

        //functions
        public LoggerWrapperImpl(ILogger logger)
        {
            this.v_logger = logger;
        }

        //properties
        public virtual ILogger Logger
        {
            get { return this.v_logger; }
        }
    }
}
