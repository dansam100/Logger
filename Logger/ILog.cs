using System;
using Logger.Core;
using Logger.Container;
using System.Collections.Generic;
using System.Text;
using Logger.Notifier;

namespace Logger
{
    public interface ILog : ILoggerWrapper
    {
        void Debug(object message);

        void Debug(object message, Exception exception);

        void DebugFormat(string message, params object[] objects);

        void DebugFormat(IFormatProvider provider, string format, params object[] objects);

        void Error(object message);

        void Error(object message, Exception exception);

        void ErrorFormat(string message, params object[] objects);

        void ErrorFormat(IFormatProvider provider, string format, params object[] objects);

        void Info(object message);

        void Info(object message, Exception exception);

        void InfoFormat(string message, params object[] objects);

        void InfoFormat(IFormatProvider provider, string format, params object[] objects);

        void Warn(object message);

        void Warn(object message, Exception exception);

        void WarnFormat(string message, params object[] objects);

        void WarnFormat(IFormatProvider provider, string format, params object[] objects);

        bool IsDebugEnabled { get;}
        bool IsErrorEnabled { get;}
        bool IsFatalEnabled { get;}
        bool IsInfoEnabled { get;}
        bool IsWarnEnabled { get;}
    }
}
