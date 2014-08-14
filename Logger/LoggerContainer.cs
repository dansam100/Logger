using System;
using Logger.Core;
using Logger.Notifier;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Logger.Configuration;

namespace Logger.Container
{
    public delegate void ConfigurationChangedHandler(object sender, EventArgs e);
    public delegate void ShutDownHandler(object sender, EventArgs e);
    
    public abstract class LoggerContainerImpl : ILoggerContainer
    {
        private bool v_configured;
        private string v_name;
        private Level v_threshold;
        private List<Level> v_levelHash;
        public bool watchConfig;
        IConfigurator v_Configurator;

        //events
        public event ConfigurationChangedHandler ConfigurationChanged;
        public event ConfigurationChangedHandler ConfigurationReset;
        //public event LoggerCreatedEventHandler LoggerCreated;

        //delegates
        protected ConfigurationChangedHandler v_configurationChanged;
        protected ConfigurationChangedHandler v_configurationReset;
        protected ShutDownHandler v_containerShutdown;

        //private LoggerCreatedEventHandler v_loggerCreatedEvent;

        protected LoggerContainerImpl(string name)
        {
            v_name = name;
            v_configured = false;
            v_name = string.Empty;
            v_threshold = Level.All;
            v_levelHash = new List<Level>();
            this.PopulateLevels();
        }

        protected LoggerContainerImpl() : this("ContainerBase")
        {
        }

        private void PopulateLevels()
        {
            this.v_levelHash.Add(Level.Off);
            this.v_levelHash.Add(Level.Fatal);
            this.v_levelHash.Add(Level.Alert);
            this.v_levelHash.Add(Level.Error);
            this.v_levelHash.Add(Level.Warn);
            this.v_levelHash.Add(Level.Info);
            this.v_levelHash.Add(Level.Debug);
            this.v_levelHash.Add(Level.Trace);
            this.v_levelHash.Add(Level.Verbose);
            this.v_levelHash.Add(Level.All);

        }

        public abstract ILogger Exists(string name);
        public abstract ILogger GetLogger(string name);

        public abstract void Log(Log log);

        public virtual void Close()
        {
            //do something to shut down.
        }

        protected virtual void OnConfigurationChanged(EventArgs e)
        {
            if (e == null)
            {
                e = EventArgs.Empty;
            }
            ConfigurationChangedHandler configurationChangedEvent = this.v_configurationChanged;
            if (configurationChangedEvent != null)
            {
                configurationChangedEvent(this, EventArgs.Empty);
            }
        }

        protected virtual void OnConfigurationReset(EventArgs e)
        {
            if (e == null)
            {
                e = EventArgs.Empty;
            }
            ConfigurationChangedHandler configurationResetEvent = this.v_configurationReset;
            if (configurationResetEvent != null)
            {
                configurationResetEvent(this, e);
            }
        }

        protected virtual void OnShutdown(EventArgs e)
        {
            if (e == null)
            {
                e = EventArgs.Empty;
            }
            ShutDownHandler shutdownEvent = this.v_containerShutdown;
            if (shutdownEvent != null)
            {
                shutdownEvent(this, e);
            }
        }

        public virtual void ResetConfiguration()
        {
            this.Configured = false;
            this.v_levelHash.Clear();
            this.PopulateLevels();
        }

        public virtual IConfigurator Configurator
        {
            get { return this.v_Configurator; }
            set { this.v_Configurator = value; }
        }


        //Properties
        public virtual bool Configured
        {
            get { return v_configured; }
            set { v_configured = value; }
        }

        public virtual string Name
        {
            get { return v_name; }
            set { v_name = value; }
        }

        public virtual Level Threshold
        {
            get { return v_threshold; }
            set
            {
                if (value != null)
                {
                    this.v_threshold = value;
                }
                else
                {
                    //LogLog.Warn("LoggerRepositorySkeleton: Threshold cannot be set to null. Setting to ALL");
                    this.v_threshold = Level.All;
                }
            }
        }

        public virtual bool WatchConfig
        {
            get { return watchConfig; }
            set { watchConfig = value; }
        }

        public abstract List<ILogger> CurrentLoggers { get;}
        public abstract List<INotifier> Notifiers { get;}
    }
}
