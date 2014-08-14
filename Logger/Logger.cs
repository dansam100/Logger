using System;
using Logger.Tools;
using System.Text;
using Logger.Core;
using Logger.Notifier;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;

namespace Logger.Container.Ranks
{
    public delegate void NotifierAppenderHandler(Log logEvent);
    
    public class Logger : NotifierFastener, ILogger, INotifierFastener
    {
        private string v_name;
        private Level v_level;
        private Logger v_parent;
        private Ranks v_ranks;
        private BackgroundWorker v_logsender;
        private NotifierAppenderHandler v_notifierSender;
        private static readonly Type v_declaringType;

        static Logger()
        {
            v_declaringType = typeof(LoggerImpl);
        }
        
        public Logger(string name)
        {
            this.v_name = name;
            this.v_notifierSender = this.Notify;
            v_logsender = new BackgroundWorker();
            this.v_logsender.WorkerSupportsCancellation = true;
            v_logsender.DoWork += new DoWorkEventHandler(Logger_DoWork);
            v_logsender.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Logger_RunWorkerCompleted);
        }

        void Logger_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //done logging.
        }

        //Background functions
        void Logger_DoWork(object sender, DoWorkEventArgs e)
        {
            if (sender != null && e != null)
            {
                BackgroundWorker worker = sender as BackgroundWorker;
                Log logevent = e.Argument as Log;
                if (logevent != null)
                {
                    //this.Notify(logevent);
                    if (this.v_notifierSender != null)
                    {
                        this.v_notifierSender(logevent);
                    }
                }
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                }
            }
        }

        public virtual void Log(Log logevent)
        {
            try
            {
                if (logevent != null && this.IsEnabledFor(logevent.Level))
                {
                    if (logevent.Container != this.Rank)
                        logevent.Container = this.Rank;
                    //this.v_logsender.RunWorkerAsync(logevent);
                    this.Notify(logevent);
                }
            }
            catch (Exception ex)
            {
                //LogLog
            }
        }

        public void Log(Type callerStackBoundaryDeclaringType, LevelType level, object message, Exception exception)
        {
            try
            {
                if (this.IsEnabledFor(level))
                {
                    this.Log(new LogInfo(message, callerStackBoundaryDeclaringType, new Level(level, level.ToString()), this.Name, exception, DateTime.Now, null));
                }
            }
            catch (Exception ex)
            {
                //LogLog.Error("Log: Exception while logging", ex);
            }

        }

        public void Log(LogInfo logInfo)
        {
            Log(new Log(logInfo));
        }


        private void Notify(Log logevent)
        {
            if (logevent != null)
            {
                Logger logger = this;
                while (logger != null)
                {
                    if (logger.Notifiers != null)
                    {
                        try
                        {
                            logger.HyperNotify(logevent);
                        }
                        catch (Exception e)
                        {
                            //LogLog
                        }
                    }
                    logger = logger.Parent;
                }
            }
            else
                throw new ArgumentNullException("logevent");
        }

        public bool IsEnabledFor(Level level)
        {
            try
            {
                if (level != null)
                {
                    if (this.v_ranks.IsDisabled(level))
                    {
                        return false;
                    }
                    return (level >= this.FinalLevel);
                }
            }
            catch (Exception exception)
            {
                //LogLog.Error("Log: Exception while logging", exception);
            }
            return false;
        }

        public bool IsEnabledFor(LevelType level)
        {
            try
            {
                return ((int)level >= this.FinalLevel.Value);
            }
            catch (Exception exception)
            {
                //LogLog.Error("Log: Exception while logging", exception);
            }
            return false;
        }


        //Properies
        public string Name
        {
            get
            { return v_name; }
        }

        public ILoggerContainer Container
        {
            get { return this.v_ranks; }
        }


        public Level FinalLevel
        {
            get
            {
                Logger logger = this;
                Level level = this.v_level;
                while (logger != null)
                {
                    if (level != null)
                        return this.v_level = level;
                    logger = logger.v_parent;
                    level = logger.Level;
                }
                return null;
            }
        }

        public Level Level
        {
            get { return this.v_level; }
            set { this.v_level = value; }
        }

        public Ranks Rank
        {
            get { return this.v_ranks; }
            set { this.v_ranks = value; }
        }

        public Logger Parent
        {
            get { return v_parent; }
            set { v_parent = value; }
        }
    }
}
