using System;
using System.Collections.Generic;
using System.Text;
using Logger.Container;

namespace Logger.Core
{
    /// <summary>
    /// ILogger Wrapper.
    /// </summary>
    public interface ILoggerWrapper
    {
        ILogger Logger { get;}
    }


    public interface ILogger
    {
        //Functions
        bool IsEnabledFor(Level level);
        void Log(Log logevent);
        void Log(Type callerStackBoundaryDeclaringType, LevelType level, object message, Exception exception);

        //properties
        string Name { get;}
        ILoggerContainer Container { get; }
    }
}
