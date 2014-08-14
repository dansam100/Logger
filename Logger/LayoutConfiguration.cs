using System;
using Logger.Core;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Logger.Configuration
{
    public class LayoutConfiguration : ILayoutConfiguration
    {
        private string v_pattern;
        private string v_alias;
        private string v_header;
        private string v_footer;
        protected PropertyHash v_parameters;

        public LayoutConfiguration()
        {
            this.v_parameters = new PropertyHash();
        }

        public string Pattern
        {
            get
            {
                return v_pattern;
            }
            set
            {
                v_pattern = value;
            }
        }

        public string Alias
        {
            get
            {
                return v_alias;
            }
            set
            {
                v_alias = value;
            }
        }

        public string Header
        {
            get
            {
                return v_header;
            }
            set
            {
                v_header = value;
            }
        }

        public string Footer
        {
            get
            {
                return v_footer;
            }
            set
            {
                v_footer = value;
            }
        }

        public PropertyHash Parameters
        {
            get
            {
                return v_parameters;
            }
            set
            {
                v_parameters = value;
            }
        }

        public virtual void LoadConfiguration(XmlNode node)
        {
            string paramSelect = "self::node()//param";
            Alias = node.Attributes["name"].Value;
            XmlNodeList paramnodes = node.SelectNodes(paramSelect);
            foreach (XmlNode param in paramnodes)
            {
                XmlElement e = (XmlElement)param;
                if (e.Attributes != null && e.Attributes.Count > 1)
                {
                    switch (e.Attributes[0].Value)
                    {
                        case "header": Header = e.Attributes[1].Value;
                            this.v_parameters["header"] = Header;
                            break;
                        case "footer": Footer = e.Attributes[1].Value;
                            this.v_parameters["footer"] = Footer;
                            break;
                        case "pattern": Pattern = e.Attributes[1].Value;
                            this.v_parameters["pattern"] = Pattern;
                            break;
                        default:
                            this.v_parameters[e.Attributes[0].Value] = e.Attributes[1].Value;
                            break;
                    }
                }
            }
        }
    }
}
