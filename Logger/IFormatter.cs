using System;
using System.Collections.Generic;
using System.Text;
using Logger.Core;

namespace Logger.Layout.Formatter
{
    public interface IFormatter
    {
        void Format(Log content);
        string Pattern { get; set; }
        string ToString();
    }
}
