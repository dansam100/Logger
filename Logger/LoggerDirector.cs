using System;
using System.Collections.Generic;
using System.Text;
using Logger.Container;
using System.Reflection;
using Logger.Core;

namespace Logger.Core
{
    public sealed class LoggerDirector
    {
        //fields
        private static IContainerChooser v_containerChooser;

        //functions
        static LoggerDirector()
        {
            if (v_containerChooser == null)
            {
                v_containerChooser = new ContainerChooser(typeof(Logger.Container.Ranks.Ranks));
            }
            try
            {
                RegisterAppDomainEvents();
            }
            catch (System.Security.SecurityException se)
            {
                //LogLog("LoggerDirector: Security Exception" + se);
            }
        }
        
        private LoggerDirector()
        {
        }

        public static ILoggerContainer CreateContainer(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("container");
            }
            return ContainerChooser.CreateContainer(name, null);
        }


        public static ILoggerContainer CreateContainer(string name, Type ContainerType)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (ContainerType == null)
            {
                throw new ArgumentNullException("type");
            }
            return ContainerChooser.CreateContainer(name, ContainerType);
        }

        public static ILogger Exists(string containerName, string name)
        {
            if (containerName == null)
            {
                throw new ArgumentNullException("containerName");
            }
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            return ContainerChooser.GetContainer(containerName).Exists(name);
        }

        /// <summary>
        /// Get logger based on type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ILogger GetLogger(Type type)
        {
            return GetLogger(Assembly.GetCallingAssembly(), type.FullName);
        }

        /// <summary>
        /// Get logger based on container name and type
        /// </summary>
        /// <param name="containerAssembly"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ILogger GetLogger(Assembly containerAssembly, string name)
        {
            if (containerAssembly == null)
            {
                throw new ArgumentNullException("containerAssembly");
            }
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            return ContainerChooser.GetContainer(containerAssembly).GetLogger(name);
        }

        public static ILogger GetLogger(string containerName, string name)
        {
            if (containerName == null)
            {
                throw new ArgumentNullException("containerAssembly");
            }
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            return ContainerChooser.GetContainer(containerName).GetLogger(name);
        }

        public static List<ILogger> GetCurrentLoggers(string containerName)
        {
            if (containerName == null)
            {
                throw new ArgumentNullException("containerName");
            }
            return ContainerChooser.GetContainer(containerName).CurrentLoggers;
        }

        public static List<ILoggerContainer> GetAllContainers()
        {
            return ContainerChooser.GetContainers();
        }
        
        //Properties
        public static IContainerChooser ContainerChooser
        {
            get
            {
                return v_containerChooser;
            }
            set
            {
                v_containerChooser = value;
            }
        }

        private static void RegisterAppDomainEvents()
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
            AppDomain.CurrentDomain.DomainUnload += new EventHandler(CurrentDomain_DomainUnload);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (GetAllContainers() != null)
                CurrentDomain_ProcessExit(sender, e);
        }

        static void CurrentDomain_DomainUnload(object sender, EventArgs e)
        {
            CurrentDomain_ProcessExit(sender, e);
        }

        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            foreach (ILoggerContainer container in GetAllContainers())
            {
                container.Close();
            }
            ContainerChooser.Watcher.EndMonitor();
        }
    }
}
