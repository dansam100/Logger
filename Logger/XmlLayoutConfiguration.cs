using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Collections;

namespace Logger.Configuration
{
    public class XmlLayoutConfiguration : LayoutConfiguration, IXmlLayoutConfiguration
    {
        private string v_rootnode = "Logger",
                       v_lognode = "Log",
                       v_message = "$message$\n$exception$";

        /// <summary>
        /// TODO: Make XML Configuration Read as they should be.
        /// </summary>
        /// <param name="node"></param>
        public override void LoadConfiguration(System.Xml.XmlNode node)
        {
            string paramSelect = "self::node()/parameters/param";
            //string attrSelect = "self::node()//attribute";
            //string nodeSelect = "self::node()//node";
            Alias = node.Attributes["name"].Value;
            XmlNodeList paramnodes = node.SelectNodes(paramSelect);

            foreach (XmlNode param in paramnodes)
            {
                XmlElement e = (XmlElement)param;
                XmlNodeList children = param.ChildNodes;
                if (e.Attributes != null && e.Attributes.Count > 0)
                {
                    switch (e.Attributes["name"].Value)
                    {
                        case "header": Header = e.Attributes["value"].Value;
                            this.v_parameters["header"] = Header;
                            break;
                        case "footer": Footer = e.Attributes["value"].Value;
                            this.v_parameters["footer"] = Footer;
                            break;
                        case "pattern": Pattern = e.Attributes["value"].Value;
                            this.v_parameters["pattern"] = Pattern;
                            break;
                        case "rootnode": Rootnode = e.Attributes["value"].Value;
                            this.v_parameters["rootnode"] = Rootnode;
                            break;
                        case "lognode": Lognode = e.Attributes["value"].Value;
                            this.v_parameters["lognode"] = Lognode;
                            break;
                        default:
                            if (e.Attributes["name"].Value != null && e.Attributes["value"].Value != null)
                                this.v_parameters[e.Attributes["name"].Value] = e.Attributes["value"].Value;
                            break;
                    }
                }
                if (children != null && children.Count > 0)
                {
                    v_parameters[e.Attributes["name"].Value] = GetChildParams(children);
                }
            }
        }

        private Hashtable GetChildParams(XmlNodeList children)
        {
            Hashtable pTable = new Hashtable();
            foreach (XmlNode child in children)
            {
                if (child.Name.CompareTo("attribute") == 0 && child.Attributes.Count > 1)
                {
                    pTable[child.Attributes["name"].Value] = child.Attributes["value"].Value;
                }
                else if (child.Name.CompareTo("message") == 0 && child.ParentNode.Attributes["name"].Value == "lognode")
                {
                    Message = child.Attributes["value"].Value;
                }
                else
                {
                    if (child.ChildNodes != null && child.ChildNodes.Count > 0)
                        pTable[child.Name] = GetChildParams(child.ChildNodes);

                    if (child.Attributes != null && child.Attributes["innertext"] != null)
                    {
                        if (pTable[child.Name] != null)
                        {
                            ((Hashtable)pTable[child.Name]).Add("innertext", child.Attributes["innertext"].Value);
                        }
                        else
                        {
                            Hashtable table = new Hashtable();
                            table.Add("innertext", child.Attributes["innertext"].Value);
                            pTable[child.Name] = table;
                        }
                    }
                }
            }
            return pTable;
        }

        public string Rootnode
        {
            get { return v_rootnode; }
            set { v_rootnode = value; }
        }

        public string Lognode
        {
            get { return v_lognode; }
            set { v_lognode = value; }
        }

        public string Message
        {
            get { return v_message; }
            set { v_message = value; }
        }
    }
}
