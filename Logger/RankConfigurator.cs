using System;
using System.Collections.Generic;
using System.Text;
using Logger.Container.Ranks;
using System.Collections;
using System.Xml;
using Logger.Core;
using Logger.Notifier;
using Logger.Configuration;
using System.Reflection;

namespace Logger.Container.Ranks
{    
    public class RankConfigurator : IConfigurator
    {
        //Fields
        private const string ROOT_NAME = "logger.configuration";
        private const string DETAILS_STR = "details";
        private const string THRESHOLD_STR = "threshold";
        private const string WATCH_ATTR = "watchConfig";
        private const string TYPE_STR = "type";
        private const string NAME_STR = "name";
        private const string LEVEL_STR = "level";
        private const string NOTIFIER_STR = "notifier";
        private const string LOGGER_STR = "logger";
        private const string PARAMETERS_STR = "parameters";
        private const string PARAM_STR = "param";
        private const string LAYOUT_STR = "layout";
        private const string FILTER_STR = "filter";
        private const string ROOT_LOGGER_STR = "root";
        private const string LAYOUT_CONFIG_STR = "layout.config";

        private readonly Ranks v_rank;
        private Hashtable v_notifierList;
        private PropertyHash v_properties;
        private IList loggers;

        private List<XmlElement> loggernodes;
        private List<XmlElement> notifiernodes;
        private XmlElement rootnode;
        private XmlElement mainNode;

        static RankConfigurator()
        {
            //LOGGER_SELECT = string.Format("{0}/{1}", ROOT_NAME, LOGGER_STR);
        }

        public RankConfigurator(Ranks rank)
        {
            this.v_rank = rank;
            this.v_notifierList = new Hashtable();
            this.loggers = new List<ILogger>();
            this.loggernodes = new List<XmlElement>();
            this.notifiernodes = new List<XmlElement>();
        }

        public void Configure(Hashtable properties)
        {
        }

        public void Configure(XmlElement element)
        {
            if (element != null)
            {
                mainNode = element;
                foreach (XmlNode x in element.ChildNodes)
                {
                    if (x.NodeType == XmlNodeType.Element && x.ParentNode.LocalName == ROOT_NAME)
                    {
                        XmlElement e = (XmlElement)x;
                        if (e.LocalName == LOGGER_STR)
                        {
                            loggernodes.Add(e);
                        }
                        else if (e.LocalName == NOTIFIER_STR)
                        {
                            notifiernodes.Add(e);
                        }
                        else if (e.LocalName == LAYOUT_CONFIG_STR)
                        {
                            //Layouts already dealt with in Notifier configuration.
                        }
                        else if (e.LocalName == FILTER_STR)
                        {
                            //Filters not supported yet.
                        }
                        else if (e.LocalName == ROOT_LOGGER_STR)
                        {
                            rootnode = e;
                        }
                    }
                }

                string configWatch = element.Attributes[WATCH_ATTR].Value;
                if (configWatch != null)
                {
                    try
                    {
                        bool watchConfig = System.Convert.ToBoolean(configWatch.Trim());
                        if (watchConfig)
                        {
                            this.v_rank.WatchConfig = true;
                        }
                    }
                    catch (Exception e)
                    {
                        //Console.WriteLine(e);
                    }
                }
                string threshold = element.GetAttribute(THRESHOLD_STR);
                //LogLog.Debug("XmlHierarchyConfigurator: Hierarchy Threshold [" + str5 + "]");
                if ((threshold.Length > 0) && (threshold != "null"))
                {
                    Level level = new Level((LevelType)Enum.Parse(typeof(LevelType), threshold), threshold);
                    if (level != null)
                    {
                        this.v_rank.Threshold = level;
                    }
                    else
                    {
                        //LogLog.Warn("XmlHierarchyConfigurator: Unable to set hierarchy threshold using value [" + str5 + "] (with acceptable conversion types)");
                    }
                }

                ParseAllElements();

            }
        }

        /// <summary>
        /// Parse all elements of the configuration
        /// </summary>
        private void ParseAllElements()
        {
            ParseNotifiers();
            ParseRootLogger();
            ParseLoggers();
            /*foreach (ILogger logger in loggers)
            {
                this.v_rank.r[new LoggerHash(logger.Name)] = logger;
            }*/
            
            Properties = new PropertyHash();
            Properties["root"] = this.v_rank.Root;
            Properties["loggers"] = loggers;
            Properties["threshold"] = this.v_rank.Threshold;
            Properties["watchConfig"] = this.v_rank.WatchConfig;
        }


        private void ParseRootLogger()
        {
            if (rootnode != null)
            {
                Logger root = this.v_rank.Root;
                XmlElement details = rootnode[DETAILS_STR];
                string level = null;
                if (details.HasAttributes)
                {
                    level = details.Attributes[LEVEL_STR].Value;
                }
                lock (root)
                {
                    if (!string.IsNullOrEmpty(level) || level.ToLower() != "null")
                    {
                        LevelType lev = (LevelType)Enum.Parse(typeof(LevelType), level);
                        root.Level = new Level(lev, lev.ToString());
                    }

                    root.RemoveAllNotifiers();
                }
                ParseLogger(ref root, rootnode, true);
            }
        }

        private void ParseLogger(ref Logger logger, XmlElement node, bool isRoot)
        {
            try
            {
                lock (logger)
                {
                    foreach (XmlNode nElement in node)
                    {
                        switch (nElement.LocalName.ToLower())
                        {
                            case NOTIFIER_STR:
                                string nName = nElement.Attributes[NAME_STR].Value;
                                INotifier notifier;
                                if (!string.IsNullOrEmpty(nName) || nName != "null")
                                {
                                    notifier = this.v_notifierList[nName] as INotifier;
                                    if (notifier == null)
                                    {
                                        throw new ArgumentNullException("notifier");
                                    }
                                    if(logger.Parent == null || !logger.Parent.Contains(notifier))
                                        logger.AddNotifier(notifier);
                                }
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //LogLog(e);
            }
        }

        private void ParseLoggers()
        {
            foreach (XmlElement e in loggernodes)
            {
                ParseLogger(e);
            }
        }

        private void ParseLogger(XmlElement e)
        {
            XmlElement details = e["details"];
            string level = null;
            string name = null;
            if (details != null && details.HasAttributes)
            {
                level = details.Attributes["level"].Value;
                name = details.Attributes["name"].Value;
            }
            Logger log = this.v_rank.GetLogger(name) as Logger;
            log.Parent = this.v_rank.Root;
            lock (log)
            {
                if (string.IsNullOrEmpty(level) || level.ToLower() != "null")
                {
                    LevelType lev = (LevelType)Enum.Parse(typeof(LevelType), level);
                    log.Level = new Level(lev, lev.ToString());
                }
                log.RemoveAllNotifiers();
            }
            ParseLogger(ref log, e, false);
            loggers.Add(log);
        }


        private void ParseNotifiers()
        {
            foreach(XmlElement e in notifiernodes)
            {
                string name = e.Attributes[NAME_STR].Value;
                string type = e.Attributes[TYPE_STR].Value;
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(type))
                {
                    throw new ArgumentNullException("name|type");
                    //LogLog("Name and Type must be specified for Notifier configurations");
                }

                INotifier notifier = (INotifier)Activator.CreateInstance(Type.GetType(type));
                notifier.Configure(e, mainNode);
                this.v_notifierList[name] = notifier;
            }
        }

        public PropertyHash Properties
        {
            get
            {
                return v_properties;
            }
            set { v_properties = value; }
        }
    }
}
