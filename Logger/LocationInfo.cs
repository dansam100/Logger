using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.Globalization;
using System.Security;

namespace Logger.Core
{
    [Serializable]
    public class LocationInfo
    {
        // Fields
        private string v_className;
        private string v_fileName;
        private string v_fullInfo;
        private string v_lineNumber;
        private string v_methodName;

        public const string NA = "N/A";

        // Methods
        public LocationInfo(Type callerStackBoundaryDeclaringType)
        {
            this.v_className = NA;
            this.v_fileName = NA;
            this.v_lineNumber = NA;
            this.v_methodName = NA;
            this.v_fullInfo = NA;
            if (callerStackBoundaryDeclaringType != null)
            {
                try
                {
                    StackFrame frame;
                    StackTrace trace = new StackTrace(true);
                    int index = 0;
                    while (index < trace.FrameCount)
                    {
                        frame = trace.GetFrame(index);
                        if ((frame != null) && (frame.GetMethod().DeclaringType == callerStackBoundaryDeclaringType))
                        {
                            break;
                        }
                        index++;
                    }
                    while (index < trace.FrameCount)
                    {
                        frame = trace.GetFrame(index);
                        if ((frame != null) && (frame.GetMethod().DeclaringType != callerStackBoundaryDeclaringType))
                        {
                            break;
                        }
                        index++;
                    }
                    if (index < trace.FrameCount)
                    {
                        StackFrame frame2 = trace.GetFrame(index);
                        if (frame2 != null)
                        {
                            ExtractProperties(frame2);
                        }
                    }
                }
                catch (SecurityException)
                {
                    //LogLog.Debug("LocationInfo: Security exception while trying to get caller stack frame. Error Ignored. Location Information Not Available.");
                }
            }
        }


        public LocationInfo(Exception exception)
        {
            this.v_className = NA;
            this.v_fileName = NA;
            this.v_lineNumber = NA;
            this.v_methodName = NA;
            this.v_fullInfo = NA;

            if (exception != null)
            {
                try
                {
                    StackFrame frame;
                    StackTrace trace = new StackTrace(exception, true);
                    frame = trace.GetFrame(0);
                    ExtractProperties(frame);
                }
                catch (SecurityException)
                {
                    //LogLog.Debug("LocationInfo: Security exception while trying to get caller stack frame. Error Ignored. Location Information Not Available.");
                }
            }
        }

        public LocationInfo(string className, string methodName, string fileName, string lineNumber)
        {
            this.v_className = className;
            this.v_fileName = fileName;
            this.v_lineNumber = lineNumber;
            this.v_methodName = methodName;
            this.v_fullInfo = string.Concat(new object[] { this.v_className, '.',
                this.v_methodName, '(', this.v_fileName, ':', this.v_lineNumber, ')' });
        }

        private void ExtractProperties(StackFrame frame)
        {
            MethodBase method = frame.GetMethod();
            if (method != null)
            {
                this.MethodName = method.Name;
                if (method.DeclaringType != null)
                {
                    this.ClassName = method.DeclaringType.FullName;
                }
            }
            
            this.FileName = frame.GetFileName();
            this.LineNumber = frame.GetFileLineNumber().ToString(NumberFormatInfo.InvariantInfo);
            this.FullInfo = string.Concat(new object[] { this.v_className, '.', this.v_methodName, '(', this.v_fileName, ':', this.v_lineNumber, ')' });
        }

        // Properties
        public string ClassName
        {
            get
            {
                return this.v_className;
            }
            private set { this.v_className = value; }
        }

        public string FileName
        {
            get
            {
                return this.v_fileName;
            }
            private set { this.v_fileName = value; }
        }

        public string FullInfo
        {
            get
            {
                return this.v_fullInfo;
            }
            private set { this.v_fullInfo = value; }
        }

        public string LineNumber
        {
            get
            {
                return this.v_lineNumber;
            }
            private set { this.v_lineNumber = value; }
        }

        public string MethodName
        {
            get
            {
                return this.v_methodName;
            }
            private set { this.v_methodName = value; }
        }
    }
}
