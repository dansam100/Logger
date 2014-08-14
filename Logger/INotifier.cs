using System;
using Logger.Core;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace Logger.Notifier
{
    /// <summary>
    /// Log notifier interface.
    /// </summary>
    public interface INotifier
    {
        //functions
        void Exit();
        void Append(Log log);
        void LoadConfiguration();
        void Configure(XmlElement element);
        void Configure(XmlElement e, XmlElement mainNode);
        int OtherHashCode();
        //properties
        string Name { get;}
        Stream Stream { get; set;}
        bool AppendContent { get; set;}
    }
}
