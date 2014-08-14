using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Logger.Tools;
using System.ComponentModel;

namespace Logger.Core
{
    class LoggerImpl : ILog
    {
        //Fields
        private static readonly Type DeclaringType;
        private readonly ILogger v_logger = null;

        /// <summary>
        /// Static ctor
        /// </summary>
        static LoggerImpl()
        {
            DeclaringType = typeof(LoggerImpl);
        }

        public LoggerImpl(ILogger logger)
        {
            this.v_logger = logger;
        }

        //Functions
        public void Debug(object message)
        {
            if (this.IsDebugEnabled)
            {
                this.Logger.Log(DeclaringType, LevelType.Debug, message, null);
            }
        }

        public void Debug(object message, Exception exception)
        {
            if (this.IsDebugEnabled)
            {
                this.Logger.Log(DeclaringType, LevelType.Debug, message, exception);
            }
        }

        public void DebugFormat(string message, params object[] objects)
        {
            if (this.IsDebugEnabled)
            {
                this.Logger.Log(DeclaringType, LevelType.Debug, new StringFormat(null, message, objects), null);
            }
        }

        public void DebugFormat(IFormatProvider provider, string format, params object[] objects)
        {
            if (this.IsDebugEnabled)
            {
                this.Logger.Log(DeclaringType, LevelType.Debug, new StringFormat(provider, format, objects), null);
            }

        }

        public void Error(object message)
        {
            if (this.IsErrorEnabled)
            {
                this.Logger.Log(DeclaringType, LevelType.Error, message, null);
            }
        }

        public void Error(object message, Exception exception)
        {
            if (this.IsErrorEnabled)
            {
                this.Logger.Log(DeclaringType, LevelType.Error, message, exception);
            }
        }

        public void ErrorFormat(string message, params object[] objects)
        {
            if (this.IsErrorEnabled)
            {
                this.Logger.Log(DeclaringType, LevelType.Error, new StringFormat(CultureInfo.InvariantCulture, message, objects), null);
            }
        }

        public void ErrorFormat(IFormatProvider provider, string format, params object[] objects)
        {
            if (this.IsErrorEnabled)
            {
                this.Logger.Log(DeclaringType, LevelType.Error, new StringFormat(provider, format, objects), null);
            }

        }

        public void Info(object message)
        {
            if (this.IsInfoEnabled)
            {
                this.Logger.Log(DeclaringType, LevelType.Info, message, null);
            }

        }

        public void Info(object message, Exception exception)
        {
            if (this.IsInfoEnabled)
            {
                this.Logger.Log(DeclaringType, LevelType.Info, message, exception);
            }

        }

        public void InfoFormat(string message, params object[] objects)
        {
            if (this.IsInfoEnabled)
            {
                this.Logger.Log(DeclaringType, LevelType.Info, new StringFormat(CultureInfo.InvariantCulture, message, objects), null);
            }

        }

        public void InfoFormat(IFormatProvider provider, string format, params object[] objects)
        {
            if (this.IsInfoEnabled)
            {
                this.Logger.Log(DeclaringType, LevelType.Info, new StringFormat(provider, format, objects), null);
            }
        }

        public void Warn(object message)
        {
            if (this.IsWarnEnabled)
            {
                this.Logger.Log(DeclaringType, LevelType.Info, message, null);
            }
        }

        public void Warn(object message, Exception exception)
        {
            if (this.IsWarnEnabled)
            {
                this.Logger.Log(DeclaringType, LevelType.Info, message, exception);
            }
        }

        public void WarnFormat(string message, params object[] objects)
        {
            if (this.IsWarnEnabled)
            {
                this.Logger.Log(DeclaringType, LevelType.Warn, new StringFormat(CultureInfo.InvariantCulture, message, objects), null);
            }
        }

        public void WarnFormat(IFormatProvider provider, string format, params object[] objects)
        {
            if (this.IsWarnEnabled)
            {
                this.Logger.Log(DeclaringType, LevelType.Warn, new StringFormat(provider, format, objects), null);
            }
        }


        //Properties
        public bool IsDebugEnabled
        { get { return this.Logger.IsEnabledFor(Level.Debug); } }

        public bool IsErrorEnabled
        { get { return this.Logger.IsEnabledFor(Level.Error); } }

        public bool IsFatalEnabled
        { get { return this.Logger.IsEnabledFor(Level.Fatal); } }

        public bool IsInfoEnabled
        { get { return this.Logger.IsEnabledFor(Level.Info); } }

        public bool IsWarnEnabled
        { get { return this.Logger.IsEnabledFor(Level.Warn); } }

        public ILogger Logger
        {
            get { return this.v_logger; }
        }
    }
}
