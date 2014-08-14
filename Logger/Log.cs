using System;
using System.Text;
using Logger.Core;
using Logger.Container;
using System.Security.Permissions;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Logger.Configuration;
using System.Threading;
using System.Security;
using System.Globalization;

namespace Logger.Core
{
    [Serializable]
    public class Log : ISerializable
    {
        private LogInfo v_info;
        private readonly object v_message;
        private ILoggerContainer v_container;
        private readonly Exception v_exception;

        private readonly Type v_callerStackBoundaryDeclaringType; 

        public Log(LogInfo info) : this(null, null, info){ }

        protected Log(SerializationInfo info, StreamingContext context)
        {
            
            this.v_info.LoggerName = info.GetString("LoggerName");
            this.v_info.Level = (Level)info.GetValue("Level", typeof(Level));
            this.v_info.Message = info.GetString("Message");
            this.v_info.ThreadName = info.GetString("ThreadName");
            this.v_info.TimeStamp = info.GetDateTime("TimeStamp");
            this.v_info.LocationInfo = (LocationInfo)info.GetValue("LocationInfo", typeof(LocationInfo));
            this.v_info.UserName = info.GetString("UserName");
            this.v_info.Exception = (Exception)info.GetValue("Exception", typeof(Exception));
            this.v_info.Properties = (PropertyHash)info.GetValue("Properties", typeof(PropertyHash));          
        }

        public Log(Type callerStackBoundaryDeclaringType, ILoggerContainer container, LogInfo data)
        {
            this.v_container = container;
            this.v_callerStackBoundaryDeclaringType = callerStackBoundaryDeclaringType;
            this.v_info = data;
            this.v_message = v_info.Message;
            this.v_exception = v_info.Exception;
        }

        public Log(Type callerStackBoundaryDeclaringType, ILoggerContainer container,
                    string loggerName, Level level, object message, Exception exception, LocationInfo info)
            : this(callerStackBoundaryDeclaringType, container, new LogInfo(message, callerStackBoundaryDeclaringType,
                level, loggerName, exception, DateTime.Now, info)) { }

        
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("LoggerName", this.v_info.LoggerName);
            info.AddValue("Level", this.v_info.Level);
            info.AddValue("Message", this.v_info.Message);
            info.AddValue("ThreadName", this.v_info.ThreadName);
            info.AddValue("TimeStamp", this.v_info.TimeStamp);
            info.AddValue("LocationInfo", this.v_info.LocationInfo);
            info.AddValue("UserName", this.v_info.UserName);
            info.AddValue("Exception", this.v_info.Exception);
            info.AddValue("Properties", this.v_info.Properties);
        }

        public string ThreadName
        {
            get
            {
                if(this.v_info.ThreadName == null)
                {
                    this.v_info.ThreadName = Thread.CurrentThread.Name;
                    if ((this.v_info.ThreadName == null) || (this.v_info.ThreadName.Length == 0))
                    {
                        try
                        {
                            this.v_info.ThreadName = Thread.CurrentThread.ManagedThreadId.ToString(NumberFormatInfo.InvariantInfo);
                        }
                        catch (SecurityException)
                        {
                            this.v_info.ThreadName = Thread.CurrentThread.GetHashCode().ToString(CultureInfo.InvariantCulture);
                        }
                    }
                }
                return this.v_info.ThreadName;
            }

        }

        public Level Level
        { get { return v_info.Level; } }

        public string LoggerName
        { get { return this.ToString(); } }

        public Exception Exception
        { get { return this.v_exception; } }

        public ILoggerContainer Container
        { 
            get { return this.v_container; }
            set { this.v_container = value; }
        }

        public object Message
        { get { return this.v_message; } }

        public LogInfo LogInfo
        {
            get
            {
                return v_info;
            }
        }
    }
}
