using System;
using System.Collections.Generic;
using System.Text;
using Logger.Configuration;
using Logger.Core;
using System.Collections;
using Logger.Notifier;
using System.Xml;
using Logger.Tools;

namespace Logger.Container.Ranks
{
    public sealed class Ranks : LoggerContainerImpl, IConfigurator
    {
        private ILogPlant v_defaultplant;
        private IDictionary v_rankTable;
        private Logger v_mainLogger;
        private PropertyHash v_properties;
        public LevelCollection LevelCollection;
        
        /// <summary>
        /// Ctor
        /// </summary>
        public Ranks() : this(new LogPlant())
        {
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="logplant"></param>
        public Ranks(ILogPlant logplant)
        {
            this.v_defaultplant = logplant;
            this.v_rankTable = new Dictionary<LoggerHash,ILogger>();
            this.LevelCollection = new LevelCollection();

            this.v_configurationChanged = this.LoadConfiguration;
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="logplant"></param>
        /// <param name="properties"></param>
        public Ranks(ILogPlant logplant, PropertyHash properties) : this(logplant)
        {
            this.v_properties = properties;
        }

        /// <summary>
        /// Log an event
        /// </summary>
        /// <param name="logevent">event to log</param>
        public override void Log(Log logevent)
        {
            if (logevent == null)
            {
                throw new ArgumentNullException("logevent");
            }
            this.GetLogger(logevent.LoggerName, this.v_defaultplant).Log(logevent);

        }

        /// <summary>
        /// Add a level to the level collection
        /// </summary>
        /// <param name="levelEntry">the level to add</param>
        internal void AddLevel(LevelEntry levelEntry)
        {
            if (levelEntry == null)
            {
                throw new ArgumentNullException("levelEntry");
            }
            if (levelEntry.Name == null)
            {
                throw new ArgumentNullException("levelEntry.Name");
            }
            if (levelEntry.Value == -1)
            {
                Level level = this.LevelCollection[levelEntry.Name];
                if (level == null)
                {
                    throw new InvalidOperationException("Cannot redefine level [" +
                        levelEntry.Name + "] because it is not defined in the LevelMap. To define the level supply the level value.");
                }
                levelEntry.Value = level.Value;
            }
            this.LevelCollection.Add(levelEntry.Name, levelEntry.Value);
        }

        /// <summary>
        /// Clear the logger
        /// </summary>
        public void Clear()
        {
            this.v_rankTable.Clear();
        }

        /// <summary>
        /// Collect notifiers
        /// </summary>
        /// <param name="notifierList">the list to populate</param>
        /// <param name="container">the container to grab notifiers from.</param>
        private static void CollectNotifiers(ref List<INotifier> notifierList, INotifierFastener container)
        {
            foreach (INotifier notifier in container.Notifiers)
            {
                CollectNotifiers(ref notifierList, notifier);
            }
        }

        /// <summary>
        /// Get a logger by its hash.
        /// </summary>
        /// <param name="hash">logger hash</param>
        /// <returns></returns>
        internal ILogger this[LoggerHash hash]
        {
            get
            {
                if (hash == null)
                    throw new ArgumentNullException("hash");
                return (ILogger)this.v_rankTable[hash];
            }
        }

        /// <summary>
        /// Collect notifiers from a notifier.
        /// </summary>
        /// <param name="notifierList"></param>
        /// <param name="notifier"></param>
        private static void CollectNotifiers(ref List<INotifier> notifierList, INotifier notifier)
        {
            if (!notifierList.Contains(notifier))
            {
                notifierList.Add(notifier);
                INotifierFastener container = notifier as INotifierFastener;
                if (container != null)
                    CollectNotifiers(ref notifierList, container);
            }
        }

        /// <summary>
        /// Ensure that a logger exists.
        /// </summary>
        /// <param name="name">the name of the logger</param>
        /// <returns>the logger if found</returns>
        public override ILogger Exists(string name)
        {
            if (name != null)
            {
                return (this.v_rankTable[new LoggerHash(name)] as Logger);
            }
            throw new ArgumentNullException("name");
        }

        /// <summary>
        /// Get a logger by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override ILogger GetLogger(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            return GetLogger(name, this.v_defaultplant);
        }

        /// <summary>
        /// Get a logger based on its name.
        /// </summary>
        /// <param name="name">name of the logger</param>
        /// <param name="plant">the log plant responsible for creating the said logger.</param>
        /// <returns>logger if found, or newly created logger if not found</returns>
        public ILogger GetLogger(string name, ILogPlant plant)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (plant == null)
                throw new ArgumentNullException("plant");
            LoggerHash hash = new LoggerHash(name);
            lock (this.v_rankTable)
            {
                Logger logger = this.v_rankTable[hash] as Logger;
                if (logger == null)
                {
                    logger = plant.GrowLogger(name);
                    logger.Rank = this;
                    this.InformParents(logger);
                    lock (this)
                    {
                        this.v_rankTable[hash] = logger;
                    }
                    //this.OnLoggerCreatedEvent(logger);
                    return logger;
                }
                return logger;
            }
        }

        /// <summary>
        /// Traverse upwards and notify parents of the newly created logger.
        /// </summary>
        /// <param name="log"></param>
        private void InformParents(Logger log)
        {
            string name = log.Name;
            for (int i = name.LastIndexOf('.'); i >= 0; i = name.Substring(0, i - 1).LastIndexOf('.'))
            {
                LoggerHash hash = new LoggerHash(name.Substring(0, i));
                Logger parent = this.v_rankTable[hash] as Logger;
                if (parent != null)
                {
                    log.Parent = parent;
                    return;
                }
            }
            log.Parent = this.v_mainLogger;
        }

        /// <summary>
        /// Check if a level is disabled.
        /// </summary>
        /// <param name="level">the leevl to check for</param>
        /// <returns>true or false depending on whether the level is enabled or disabled</returns>
        public bool IsDisabled(Level level)
        {
            if (level == null)
                throw new ArgumentNullException("level");
            if (this.Configured)
                return (this.Threshold > level);
            return true;
        }

        /// <summary>
        /// Configure the logger
        /// </summary>
        /// <param name="element">the xml node to use for configuration</param>
        public void Configure(XmlElement element)
        {

            if (!Configured)
            {
                base.Configurator = new RankConfigurator(this);
                Configurator.Configure(element);
                this.Configured = true;
            }
            else
            {
                ((RankConfigurator)Configurator).Configure(element);
                this.Configured = true;
            }
            this.OnConfigurationChanged(null);
        }

        /// <summary>
        /// configure using a hashtable.
        /// </summary>
        /// <param name="properties"></param>
        public void Configure(Hashtable properties)
        {
            this.Configurator.Properties = (PropertyHash)properties;
            this.OnConfigurationChanged(null);
        }

        /// <summary>
        /// Load configuration into container
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void LoadConfiguration(object sender, EventArgs e)
        {
            LoggerContainerImpl container = sender as LoggerContainerImpl;
            if (container != null)
            {
                PropertyHash p = (PropertyHash)container.Configurator.Properties;
                foreach (object key in p.Keys)
                {
                    switch (key.ToString())
                    {
                        case "threshold":
                            this.Threshold = (Level)p[key];
                            break;
                        case "loggers":
                            IList l = (IList)p[key];
                            foreach (Object i in l)
                            {
                                ILogger ilogger = i as Logger;
                                if (ilogger != null)
                                {
                                    LoggerHash hash = new LoggerHash(ilogger.Name);
                                    ((Logger)ilogger).Parent = this.Root; 
                                    this.v_rankTable[hash] = ilogger;
                                }
                            }
                            break;
                        case "root":
                            ILogger logger = p[key] as ILogger;
                            if (logger != null)
                                this.Root = (Logger)logger;
                            break;
                        case "watchConfig":
                            bool value = false;
                            System.Boolean.TryParse(p[key].ToString(), out value);
                            WatchConfig = value;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Close the container
        /// </summary>
        public override void Close()
        {
            //LogLog("Shutdown called on ranks");
            this.Root.CloseNestedNofitiers();
            lock (this.v_rankTable)
            {
                List<ILogger> loggers = this.CurrentLoggers;
                foreach (Logger logger in loggers)
                {
                    logger.CloseNestedNofitiers();
                }
                this.Root.RemoveAllNotifiers();
                foreach (Logger logger in loggers)
                {
                    logger.RemoveAllNotifiers();
                }
            }
            base.Close();
        }

        /// <summary>
        /// Reset all configurations
        /// </summary>
        public override void ResetConfiguration()
        {
            this.Root.Level = Level.Debug;
            this.Threshold = Level.All;
            lock (this.v_rankTable)
            {
                this.Close();
                foreach (Logger logger in this.CurrentLoggers)
                {
                    logger.Level = null;
                }
            }
            base.ResetConfiguration();
            this.OnConfigurationChanged(null);
        }

        //Properties
        /// <summary>
        /// Root logger.
        /// </summary>
        public Logger Root
        {
            get
            {
                if (this.v_mainLogger == null)
                {
                    lock (this)
                    {
                        if (this.v_mainLogger == null)
                        {
                            Logger logger = this.v_defaultplant.GrowLogger(null);
                            logger.Rank = this;
                            this.v_mainLogger = logger;
                        }
                    }
                }
                return this.v_mainLogger;
            }
            private set { this.v_mainLogger = value; }
        }

        /// <summary>
        /// Notifiers
        /// </summary>
        public override List<INotifier> Notifiers
        {
            get
            {
                List<INotifier> notifiers = new List<INotifier>();
                CollectNotifiers(ref notifiers, this.v_mainLogger);
                foreach (Logger logger in this.CurrentLoggers)
                {
                    CollectNotifiers(ref notifiers, logger);
                }
                return notifiers;
            }
        }

        /// <summary>
        /// Properties
        /// </summary>
        public PropertyHash Properties
        {
            get { return this.v_properties; }
            set { this.v_properties = value; }
        }

        /// <summary>
        /// Current loggers
        /// </summary>
        public override List<ILogger> CurrentLoggers
        {
            get
            {
                List<ILogger> loggers = new List<ILogger>((IEnumerable<ILogger>)this.v_rankTable.Values);
                return loggers;
            }
        }
    }
}
