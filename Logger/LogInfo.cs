using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Logger.Configuration;
using System.Reflection;

namespace Logger.Core
{
    [StructLayout(LayoutKind.Sequential)]
    public struct LogInfo
    {
        public LogInfo(object message, Type callerStackBoundaryDeclaringType, Level level, string loggerName, Exception exception, DateTime date, LocationInfo info)
        {
            this.LoggerName = loggerName;
            this.Level = level;
            this.Message = message.ToString();
            this.Exception = exception;
            this.TimeStamp = date;
            this.Properties = new PropertyHash();
            this.UserName = System.Environment.UserName;
            this.ThreadName = System.Threading.Thread.CurrentThread.Name;
            if (info == null)
            {
                if (exception != null)
                    this.LocationInfo = new LocationInfo(exception);
                else
                    this.LocationInfo = new LocationInfo(callerStackBoundaryDeclaringType);
            }
            else
                this.LocationInfo = info;
        }

        public string LoggerName;
        public Level Level;
        public string Message;
        public DateTime TimeStamp;
        public Exception Exception;
        
        public PropertyHash Properties;
        public LocationInfo LocationInfo;
        public string UserName;
        public string ThreadName;
        //public string Domain;
        //public string Identity;
    }
}
