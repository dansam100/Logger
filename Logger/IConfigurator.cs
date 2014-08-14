using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Collections;
using Logger.Configuration;

namespace Logger.Configuration
{
    public interface IConfigurator
    {
        void Configure(XmlElement configElement);
        void Configure(Hashtable configTable);
        PropertyHash Properties { get; set; }
    }
}
