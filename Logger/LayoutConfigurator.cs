using System;
using System.Collections.Generic;
using System.Text;
using Logger.Core;
using System.Collections;
using System.Xml;
using Logger.Configuration;

namespace Logger.Configuration
{
    public class LayoutConfigurator : IConfigurator
    {
        //fields
        protected PropertyHash v_properties;

        public LayoutConfigurator()
        {
            v_properties = new PropertyHash();
        }

        public virtual void Configure(XmlElement configElement)
        {
            if (configElement != null)
            {
                v_properties["name"] = configElement.Attributes["name"].Value;
                v_properties["type"] = configElement.Attributes["type"].Value;

                string paramSelect = "self::node()//param";
                XmlNodeList nParams = configElement.SelectNodes(paramSelect);
                foreach (XmlNode node in nParams)
                {
                    if (node.Attributes.Count > 1)
                    {
                        v_properties[node.Attributes["name"].Value] = node.Attributes["value"].Value;
                    }
                }
                Hashtable layoutTable = new Hashtable();
                string layout = "self::node()//layout";
                XmlNode layoutNode = configElement.SelectSingleNode(layout);
                if (layoutNode == null)
                    //use default layout
                    throw new ArgumentNullException("layout");
                else
                {
                    layoutTable["name"] = layoutNode.Attributes["name"].Value;
                    if(layoutNode.Attributes["type"] != null)
                        layoutTable["type"] = layoutNode.Attributes["type"].Value;
                    v_properties["layout"] = layoutTable;
                }
            }
        }

        public virtual void Configure(Hashtable configTable)
        {
        }

        //properties
        public PropertyHash Properties
        {
            get { return this.v_properties; }
            set { this.v_properties = value; }
        }
    }
}
