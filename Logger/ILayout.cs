using System;
using System.IO;
using System.Text;
using Logger.Core;
using System.Collections.Generic;
using System.Xml;

namespace Logger.Layout
{
    public interface ILayout
    {
        //Methods
        void Format(TextWriter writer, Log log);
        void LoadConfiguration();
        void Configure(XmlElement element);
        void InitLayout(TextWriter writer);
        void Finalize(TextWriter writer);
        //Properties
        string Name { get; set;}
        string ContentType { get; }
        string Footer { get;}
        string Header { get; }
        bool IgnoresException { get;}
        string StartValue { get;}
        string EndValue { get;}
        Stream LogStream { get; set; }
        string LogFile { get; set;}
        bool NeedsStream { get; set; }
    }
}
