using System;
using System.Collections.Generic;
using System.Text;

namespace Logger.Notifier
{
    public interface IMassNotifier
    {
        void MassAppend();
        void Append(Logger.Notifier.LogEntry value);
        void Start();
        void Flush();
        void ClearBuffer();

        int Count { get; }
        LogMode Mode{get; set;}
    }

    public interface IMassAppender
    {
        string MassAppend(List<Logger.Notifier.LogEntry> buffer);
    }
}
