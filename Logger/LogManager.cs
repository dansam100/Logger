using System;
using System.Collections.Generic;
using System.Text;
using Logger.Container;
using System.Reflection;
using Logger.Core;

namespace Logger
{
    public sealed class LogManager
    {
        private static readonly LogWrapperHash v_LogWrapperHash;

        /// <summary>
        /// static ctor
        /// Initializes LogWrapper hash.
        /// </summary>
        static LogManager()
        {
            v_LogWrapperHash = new LogWrapperHash(new LogWrapperCreatedHandler(LogManager.LogWrapperCreationHandler));

        }

        /// <summary>
        /// Get logger from name
        /// </summary>
        /// <param name="name">name of the logger</param>
        /// <returns>the logger object</returns>
        public static ILog GetLogger(string name)
        {
            return GetLogger(Assembly.GetCallingAssembly(), name);
        }

        /// <summary>
        /// Get logger from assembly and name.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ILog GetLogger(Assembly assembly, string name)
        {
            return WrapLogger(LoggerDirector.GetLogger(assembly, name));
        }

        /// <summary>
        /// Get logger from name and type.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ILog GetLogger(string name, Type type)
        {
            return WrapLogger(LoggerDirector.GetLogger(name, type.FullName));
        }

        /// <summary>
        /// Get logger from type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ILog GetLogger(Type type)
        {
            return WrapLogger(LoggerDirector.GetLogger(Assembly.GetCallingAssembly(), type.FullName));
        }

        /// <summary>
        /// Current loggers
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static List<ILog> GetCurrentLoggers(Assembly assembly)
        {
            return WrapLoggers(LoggerDirector.GetCurrentLoggers(assembly.GetName().Name));
        }

        public static List<ILog> GetCurrentLoggers(string containerName)
        {
            return WrapLoggers(LoggerDirector.GetCurrentLoggers(containerName));
        }

        private static List<ILog> WrapLoggers(List<ILogger> loggers)
        {
            List<ILog> loglist = new List<ILog>();
            foreach(ILogger log in loggers)
            {
                loglist.Add(WrapLogger(log));
            }
            return loglist;
        }

        /// <summary>
        /// Create a logger container based on name an type.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ILoggerContainer CreateContainer(string name, Type type)
        {
            return LoggerDirector.CreateContainer(name, type);
        }

        /// <summary>
        /// Create a logger container based on type and calling assembly
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ILoggerContainer CreateContainer(Type type)
        {
              return CreateContainer(Assembly.GetCallingAssembly().GetName().Name, type);
        }

        /// <summary>
        /// Create a logger Container based on name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ILoggerContainer CreateContainer(string name)
        {
            return LoggerDirector.CreateContainer(name);
        }

        /// <summary>
        /// Get A logger if it exists
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ILog Exists(string containerName, string name)
        {
            if (containerName == null)
            {
                throw new ArgumentNullException("containerName");
            }
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            return WrapLogger(LoggerDirector.Exists(containerName, name));
        }

        /// <summary>
        /// Wrap the logger
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        private static ILog WrapLogger(ILogger logger)
        {
            return (ILog)v_LogWrapperHash.GetWrapper(logger);
        }


        //Properties
        public static List<ILog> CurrentLoggers
        {
            get { return GetCurrentLoggers(Assembly.GetCallingAssembly()); }
        }

        public static string ConfigLocation
        {
            //get { return "..\\..\\logger.config.xml"; }
            get { return "logger.config.xml"; }
        }

        private static ILoggerWrapper LogWrapperCreationHandler(ILogger logger)
        {
            return new LoggerImpl(logger);
        }
    }
}