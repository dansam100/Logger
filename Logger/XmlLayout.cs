using System;
using System.IO;
using Logger.Core;
using System.Text;
using System.Collections.Generic;
using System.Xml;
using Logger.Configuration;
using System.Collections;
using Logger.Tools;
using System.Reflection;
using Logger.Notifier;
using Logger.Layout.Formatter;

namespace Logger.Layout
{
    public class XmlLayout : LayoutBase, IMassAppender
    {
        /*
         * Read the configuration somehow and configure the XmlLayout.
         */
        public XmlDocument Maindocument;

        public XmlLayout(bool init)
        {
            Maindocument = new XmlDocument();
            NeedsStream = true;
            base.v_configuration = new XmlLayoutConfiguration();

            LoadConfiguration();
            if (v_configuration != null)
                base.formatter = new XmlFormatter(v_configuration);
            else
                base.formatter = new XmlFormatter();
            CreateStartXml();
        }

        public XmlLayout()
            : base()
        {
            NeedsStream = true;
            base.v_configuration = new XmlLayoutConfiguration();
        }

        protected XmlLayout(ILayoutConfiguration configuration)
        {
            v_configuration = (IXmlLayoutConfiguration)configuration;
            base.formatter = new XmlFormatter(v_configuration);
            CreateStartXml();
        }


        public override void Format(TextWriter writer, Log log)
        {
            base.formatter.Format(log);
            writer.WriteLine(base.formatter.ToString());
        }
        

        public override void Configure(XmlElement element)
        {
            base.Configure(element);
            if (v_configuration != null)
                base.formatter = new XmlFormatter(v_configuration);
            else
                base.formatter = new XmlFormatter();
            CreateStartXml();
        }

        /// <summary>
        /// Creates an xml document for hashing results from hash or search.
        /// </summary>
        /// <returns>created xml node</returns>
        private void CreateStartXml()
        {
            if (this.v_configuration != null)
            {
                StringBuilder sb = new StringBuilder();
                StringBuilder structure = new StringBuilder();

                string rootname = ((IXmlLayoutConfiguration)this.v_configuration).Rootnode;
                string lognode = ((IXmlLayoutConfiguration)this.v_configuration).Lognode;

                string headernode = CreateXmlNode("header", null, true, this.v_configuration.Header, false);
                Header = headernode;
                string footernode = CreateXmlNode("footer", null, true, this.v_configuration.Footer, false);
                Footer = footernode;

                sb.AppendLine(headernode);

                string itemroot = CreateXmlNode(rootname, (Hashtable)this.v_configuration.Parameters["rootnode"], true, sb.ToString(), true);
                itemroot = itemroot.Replace(string.Format("</{0}>", rootname), string.Empty);

                ITokenizer t = Tokenizer.CreateTokenizer(itemroot);
                itemroot = t.Format(null);

                structure.AppendLine(CreateHeader());
                structure.AppendLine(itemroot);

                v_startvalue = structure.ToString().TrimEnd('\n', '\r');
                v_endvalue = string.Format("{0}{1}</{2}>", footernode, System.Environment.NewLine, rootname);
            }
        }


        /// <summary>
        /// creates an xml header.
        /// </summary>
        /// <returns>the header</returns>
        private string CreateHeader()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            return sb.ToString();
        }

        /// <summary>
        /// Creates an xml node based on given values.
        /// </summary>
        /// <param name="name">name of the node</param>
        /// <param name="attributes">list of attributes and values</param>
        /// <param name="hasText">true if the node has inner text; false otherwise</param>
        /// <param name="text">the innertext of the node</param>
        /// <param name="hasNewline">indicates if the node should separate its children with newlines</param>
        /// <returns>the created node string</returns>
        private string CreateXmlNode(string name, Hashtable attributes, bool hasText, string text, bool hasNewline)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<{0}", name);
            int i = 0;
            if (attributes != null)
            {
                sb.Append(" ");
                foreach (object attr in attributes.Keys)
                {
                    i++;
                    sb.AppendFormat("{0}=\"{1}\"", attr.ToString(), attributes[attr].ToString());
                    if (i < attributes.Count)
                        sb.Append(" ");
                }
            }
            if (hasNewline)
                sb.AppendLine(">");
            else
                sb.Append(">");
            
            if (hasText)
            {
                if (hasNewline)
                    sb.AppendLine(text.TrimEnd('\n', '\r').TrimStart('\n', '\r'));
                else
                    sb.Append(text);

                sb.AppendFormat("</{0}>", name);
            }
            return sb.ToString();
        }


        public string MassAppend(List<Logger.Notifier.LogEntry> buffer)
        {
            XmlDocument maindoc = new XmlDocument();
            maindoc.Load(this.v_stream);
            maindoc[((IXmlLayoutConfiguration)this.v_configuration).Rootnode].InnerXml =
                        maindoc[((IXmlLayoutConfiguration)this.v_configuration).Rootnode].InnerXml.Replace(Footer, string.Empty);

            foreach (Logger.Notifier.LogEntry value in buffer)
            {
                maindoc[((IXmlLayoutConfiguration)this.v_configuration).Rootnode].InnerXml =
                    maindoc[((IXmlLayoutConfiguration)this.v_configuration).Rootnode].InnerXml + value.Content;
            }

            maindoc[((IXmlLayoutConfiguration)this.v_configuration).Rootnode].InnerXml =
                    maindoc[((IXmlLayoutConfiguration)this.v_configuration).Rootnode].InnerXml + Footer;
            string output = maindoc.OuterXml;
            return output;            
        }

        /// <summary>
        /// Content type.
        /// </summary>
        public override string ContentType
        {
            get
            {
                return base.ContentType = "text/xml";
            }
        }

        public override bool NeedsStream
        {
            get
            {
                return true;
            }
        }

        public override string StartValue
        {
            get
            {
                return v_startvalue;
            }
        }

        public override string EndValue
        {
            get
            {
                return v_endvalue;
            }
        }

    }
}
