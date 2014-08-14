using System;
using System.Xml;
using System.Text;
using Logger.Container;
using System.Reflection;
using System.Collections;
using Logger.Configuration;
using Logger.Container.Ranks;
using System.Collections.Generic;
using Logger.Configuration.Watcher;

namespace Logger.Core
{
    public class ContainerChooser : Hashtable, IContainerChooser
    {
        //Fields
        private const string DefaultLoggerName = "default-logger";
        private const string DefaultContainerName = "default-container";
        private readonly Type v_DefaultType;
        private XmlDocument Document;

        private readonly FileWatcher ConfigWatcher;

        //Functions
        public ContainerChooser(Type defaultType)
        {
            if (defaultType == null)
            {
                throw new ArgumentNullException("defaultType");
            }
            if (!typeof(ILoggerContainer).IsAssignableFrom(defaultType))
            {
                throw new ArgumentOutOfRangeException("defaultType", defaultType,
                    "Parameter: defaultType, Value: [" + defaultType + "] out of range." + 
                        "Argument must implement the ILoggerContainer interface");
            }
            this.v_DefaultType = defaultType;
            this.ConfigWatcher = new FileWatcher();
            bool watchConfig = false;
            Document = new XmlDocument();
            try
            {
                Document.Load(LogManager.ConfigLocation);
                bool.TryParse(Document.SelectSingleNode("logger/logger.configuration").Attributes["watchConfig"].Value, out watchConfig);
            }
            catch { }
            
            if (watchConfig)
                CreateConfigWatcher(LogManager.ConfigLocation);
            //LogLog.Debug("ContainerChooser: defaultType [" + this.m_defaultType + "]");
        }

        public ILoggerContainer CreateContainer(string name, Type type)
        {
            if (name == null)
                throw new ArgumentNullException("containerName");
            if (type == null)
                type = this.v_DefaultType;
            lock (this)
            {
                ILoggerContainer container = base[name] as ILoggerContainer;
                if (container != null)
                {
                    throw new LogException("Container [" + name + "] is already defined. No redefinition of Containers allowed!");
                }
                //Create a new one.
                container = (ILoggerContainer)Activator.CreateInstance(type);
                container.Name = name;
                this[name] = container;
                //call event for creation.
                return container;
            }
        }

        /// <summary>
        /// Create a container based on assembly name, name, and type.
        /// This is normally used to create the default container.
        /// </summary>
        /// <param name="assembly">name of the assembly in question.</param>
        /// <param name="type">type of the container</param>
        /// <param name="name">name of the container</param>
        /// <returns>created container</returns>
        public ILoggerContainer CreateContainer(Assembly assembly, Type type, String name)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");
            if (type == null)
                type = this.v_DefaultType;

            lock (this)
            {
                ILoggerContainer container = this[assembly.GetName().Name] as ILoggerContainer;
                {
                    if (container == null)
                    {
                        try
                        {
                            //LogLog("Creating repository");
                            container = this.CreateContainer(assembly.GetName().Name, type);
                            this.ConfigureContainer(name, container);
                        }
                        catch (Exception e)
                        {
                            //Failure.
                        }
                    }
                }
                return container;
            }
        }

        /// <summary>
        /// All created containers within this body.
        /// </summary>
        /// <returns>A List&lt;ILoggerContainer&gt; of all created containers</returns>
        public List<ILoggerContainer> GetContainers()
        {
            List<ILoggerContainer> containers = new List<ILoggerContainer>();
            lock (this)
            {
                foreach (object container in this.Values)
                {
                    containers.Add((ILoggerContainer)container);
                }
            }
            return containers;
        }

        /// <summary>
        /// Get a container by name
        /// </summary>
        /// <param name="name">the name of the container</param>
        /// <returns>container with desired name</returns>
        public ILoggerContainer GetContainer(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("containerName");
            }
            ILoggerContainer container = this[name] as ILoggerContainer;
            if (container == null)
            {
                LogException le =  new LogException("Container [" + name + "]: NOT DEFINED.");
                //LogLog(le);
                //LogLog("Creating new repository");
                return CreateContainer(name, this.v_DefaultType);
            }
            return container;
        }

        /// <summary>
        /// Get a container by assembly
        /// </summary>
        /// <param name="name">the assembly of the container</param>
        /// <returns>container with desired assembly attributes</returns>
        public ILoggerContainer GetContainer(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");
            return this.CreateContainer(assembly, this.v_DefaultType, DefaultContainerName);
        }

        /// <summary>
        /// Checks if a container exists within this framework.
        /// </summary>
        /// <param name="name">the name of the container to check for</param>
        /// <returns>true or false depending on whether the container is found or not</returns>
        public bool ContainerExists(string name)
        {
            return this.ContainsKey(name);
        }

        /// <summary>
        /// Configure a given container.
        /// </summary>
        /// <param name="name">name of the container</param>
        /// <param name="container">the container</param>
        private void ConfigureContainer(string name, ILoggerContainer container)
        {
            string configLocation = LogManager.ConfigLocation;
            
            try
            {
                if(Document != null)
                    Document.Load(configLocation);
                IConfigurator configurator = container as IConfigurator;
                if (configurator != null && name == DefaultContainerName)
                {
                    XmlNode mainNode = Document.SelectSingleNode("logger/logger.configuration");
                    configurator.Configure((XmlElement)mainNode);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Creates a configuration file watcher if one is desired.
        /// </summary>
        /// <param name="configLocation">the configuration file location</param>
        private void CreateConfigWatcher(string configLocation)
        {
            this.ConfigWatcher.SetWatcherPath(configLocation);
            this.ConfigWatcher.FileChangedHandler = ReloadConfigurations;
            this.ConfigWatcher.ReplaceHandler = ReCreateConfig;
            this.ConfigWatcher.BeginMonitor();
        }

        /// <summary>
        /// Recreates configuration file if removed.
        /// </summary>
        private void ReCreateConfig(object sender, EventArgs e)
        {
            if (Document != null)
                Document.Save(LogManager.ConfigLocation);
        }


        private void ReloadConfigurations(object sender, EventArgs e)
        {
            foreach (ILoggerContainer c in this)
            {
                if (c.WatchConfig)
                {
                    lock (c)
                    {
                        c.ResetConfiguration();
                        this.ConfigureContainer(c.Name, c);
                    }
                }
            }
        }


        #region overrides

        public override bool ContainsKey(object key)
        {
            String name = key as string;
            if (name != null)
            {
                if (base.ContainsKey(key))
                    return true;
                string k;
                for (int i = name.LastIndexOf('.'); i >= 0; i = name.Substring(0, i - 1).LastIndexOf('.'))
                {
                    k = name.Substring(0, i - 1);
                    if (base.ContainsKey(k))
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        public override Object this[object key]
        {
            get
            {
                String name = key as string;
                if (name != null)
                {
                    if (this.ContainsKey(key))
                        return base[key];
                    string k;
                    for (int i = name.LastIndexOf('.'); i >= 0; i = name.Substring(0, i-1).LastIndexOf('.'))
                    {
                        k = name.Substring(0, i - 1);
                        if (base.ContainsKey(k))
                        {
                            return base[k];
                        }
                    }
                }
                return null;
            }
        }

        #endregion

        /// <summary>
        /// Config watcher.
        /// </summary>
        public FileWatcher Watcher
        {
            get { return this.ConfigWatcher; }
        }
    }
}
