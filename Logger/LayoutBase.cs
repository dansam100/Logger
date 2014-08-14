using System;
using System.IO;
using System.Xml;
using Logger.Core;
using System.Text;
using Logger.Tools;
using Logger.Configuration;
using Logger.Layout.Formatter;
using System.Collections.Generic;

namespace Logger.Layout
{
    public abstract class LayoutBase : ILayout
    {
        private string v_name;
        private bool v_ignoresException;
        private string v_contentType = "text/log";
        protected string v_startvalue = string.Empty;
        protected string v_endvalue = string.Empty;
        protected string v_file;
        protected Stream v_stream;
        protected bool v_needsStream;
        protected ILayoutConfiguration v_configuration;
        protected IFormatter formatter;

        //Functions
        protected LayoutBase()
        {
            v_configuration = new LayoutConfiguration();
        }

        protected LayoutBase(bool init)
        {
        }

       /// <summary>
       /// Format the output based on this layout scheme.
       /// </summary>
       /// <param name="writer"></param>
       /// <param name="log"></param>
       /// <returns></returns>
        public abstract void Format(TextWriter writer, Log log);


        public virtual void InitLayout(TextWriter writer)
        {
            writer.WriteLine(StartValue);
        }

        public virtual void Finalize(TextWriter writer)
        {
            writer.WriteLine(EndValue);
        }

        /// <summary>
        /// Load layout configuration.
        /// </summary>
        public virtual void LoadConfiguration()
        {
            string textlayout = string.Format("logger/logger.configuration/layout.config[@type='{0}']", this.GetType().FullName);
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(LogManager.ConfigLocation);
                XmlNode node = doc.SelectSingleNode(textlayout);
                v_configuration.LoadConfiguration(node);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public virtual void Configure(XmlElement element)
        {
            string textlayout = string.Format("self::node()/layout.config[@type='{0}']", this.GetType().FullName);
            try
            {
                XmlNode node = element.SelectSingleNode(textlayout);
                v_configuration.LoadConfiguration(node);
            }
            catch (Exception e)
            {
                //LogLog
                throw e;
            }
        }

        //Properties
        public virtual string Name
        { get { return v_name; } set { v_name = value; } }

        public virtual string Header
        { get { return v_configuration.Header; } set { v_configuration.Header = value; } }

        public virtual string Footer
        { get { return v_configuration.Footer; } set { v_configuration.Footer = value; } }

        public virtual bool IgnoresException 
        { get { return v_ignoresException;} set { v_ignoresException = value;} }

        public virtual string ContentType
        { get { return v_contentType; } set { v_contentType = value; } }

        public virtual string StartValue
        { get { return v_startvalue; } }

        public virtual string EndValue
        { get { return v_endvalue; } }

        public virtual string LogFile
        { get { return v_file; } set { v_file = value; } }

        public virtual Stream LogStream
        { get { return v_stream; } set { v_stream = value; } }

        public virtual bool NeedsStream
        { get { return v_needsStream; } set { v_needsStream = value; } }
    }
}
