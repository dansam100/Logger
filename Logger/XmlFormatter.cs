using System;
using System.Collections.Generic;
using System.Text;
using Logger.Tools;
using Logger.Core;
using System.Xml;
using Logger.Configuration;
using System.Collections;
using System.IO;

namespace Logger.Layout.Formatter
{
    public class XmlFormatter : Formatter
    {
        private const string CLASS_ATTR = "class";
        private const string LOGGER_ATTR = "logger";
        private const string THREAD_ATTR = "thread";
        private const string MESSAGE_STR = "messsage";
        private const string DATA_STR = "data";
        private const string NAME_ATTR = "name";
        private const string VALUE_ATTR = "value";
        private const string LOCATIONINFO_STR = "locationInfo";
        private const string PROPERTIES_STR = "properties";

        private string v_rootnode;
        private string v_headernode, v_footernode;

        private IXmlLayoutConfiguration layoutconfiguration;

        public XmlFormatter(string pattern, ITokenizer tokenizer)
            : base(pattern, tokenizer)
        {
        }

        private XmlFormatter(string pattern) : base( pattern)
        {
        }

        public XmlFormatter(ILayoutConfiguration layout_config)
        {
            this.layoutconfiguration = (IXmlLayoutConfiguration)layout_config;
            CreateXml();
        }

        public XmlFormatter()
        { }

        public override void Format(Log content)
        {
            Result = base.tokenizer.Format(content);
        }

        /// <summary>
        /// Creates an xml node based on given values.
        /// </summary>
        /// <param name="name">name of the node</param>
        /// <param name="attributes">list of attributes and values.</param>
        /// <param name="hasText">true if the node has innerText; false otherwise</param>
        /// <param name="text">the innertext of the node</param>
        /// <returns>the created node string.</returns>
        private string CreateXmlNode(string name, Hashtable attributes, bool hasText, string text, bool hasNewline)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<{0}", name);
            int i = 0;
            if (attributes != null)
            {
                foreach (object attr in attributes.Keys)
                {
                    if (attributes[attr] is Hashtable)
                    {
                        string nested = CreateXmlNode(attr.ToString(), attributes[attr] as Hashtable, false, null, false);
                        text = (text == null) ? nested : string.Format("{0}%n{1}", text, nested);
                    }
                    else if(string.Compare(attr.ToString(), "innertext") != 0)
                    {
                        sb.Append(" ");
                        sb.AppendFormat("{0}=\"{1}\"", attr.ToString(), attributes[attr].ToString());
                        //if (i < attributes.Count)
                        //    sb.Append(" ");
                    }
                }
            }
            if (hasNewline)
                sb.AppendLine(">");
            else if(hasText)
                sb.Append(">");

            if (hasText || attributes.Contains("innertext"))
            {
                if (hasNewline)
                    sb.AppendLine(text);
                else
                {
                    text = text ?? attributes["innertext"].ToString();
                    sb.Append(text);
                }
                sb.AppendFormat("</{0}>", name);
            }
            else
                sb.Append("/>");
            return sb.ToString();
        }

        /// <summary>
        /// creates an xml header.
        /// </summary>
        /// <returns></returns>
        private string CreateHeader()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            return sb.ToString();
        }

        /// <summary>
        /// Creates an xml document for hashing results from hash or search.
        /// </summary>
        /// <returns>created xml document</returns>
        private void CreateXml()
        {
            //StringBuilder sb = new StringBuilder();
            if (this.layoutconfiguration != null)
            {
                StringBuilder sb = new StringBuilder();
                string rootname = this.layoutconfiguration.Rootnode;
                string lognode = this.layoutconfiguration.Lognode;
                string message = this.layoutconfiguration.Message;
                v_headernode = CreateXmlNode("header", null, true, this.layoutconfiguration.Header, false);
                v_footernode = CreateXmlNode("footer", null, true, this.layoutconfiguration.Footer, false);

                string messagenode = CreateXmlNode("message", null, true, message, false);
                string item = CreateXmlNode(lognode, (Hashtable)this.layoutconfiguration.Parameters["lognode"], true, messagenode, true);
                string itemroot = CreateXmlNode(rootname, (Hashtable)this.layoutconfiguration.Parameters["rootnode"], true, sb.ToString(), true);
            
                Pattern = item;
                sb.AppendLine(CreateHeader());
                sb.AppendLine(itemroot);
                //XmlDocument x = new XmlDocument();
                //x.LoadXml(sb.ToString());
                //v_rootnode = x;
                //v_rootnode = sb.ToSbtring();
                v_rootnode = sb.ToString();
                base.tokenizer = new Tokenizer(Pattern);
            }
        }

        public String Rootnode
        {
            get { return v_rootnode; }
        }

        public string HeaderNode
        {
            get { return v_headernode; }
        }

        public string FooterNode
        {
            get { return v_footernode; }
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
