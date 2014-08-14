using System;
using Logger.Core;
using Logger.Notifier;
using System.Collections.Generic;
using System.Text;

namespace Logger.Container
{
    /// <summary>
    /// Logger container.
    /// </summary>
    public interface ILoggerContainer
    {
        //functions
        ILogger Exists(string name);
        ILogger GetLogger(string name);
        void ResetConfiguration();
        void Log(Log log);
        void Close();

        //properties
        List<ILogger> CurrentLoggers { get;}
        List<INotifier> Notifiers { get;}
        string Name { get; set;}
        bool WatchConfig { get; set;}
    }
}
