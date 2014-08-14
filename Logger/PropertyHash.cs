using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Logger.Configuration
{
    public class PropertyHash : Hashtable
    {
    }

    public interface ILayoutConfiguration
    {
        String Pattern { get; set;}
        String Alias { get; set; }
        String Header { get; set;}
        String Footer { get;set;}
        PropertyHash Parameters { get; set;}

        void LoadConfiguration(System.Xml.XmlNode node);
    }

    public interface IXmlLayoutConfiguration : ILayoutConfiguration
    {
        String Rootnode { get; set;}
        String Lognode { get; set;}
        String Message { get; set;}
    }
}
