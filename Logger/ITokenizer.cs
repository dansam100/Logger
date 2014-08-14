using System;
using System.Collections.Generic;
using System.Text;
using Logger.Tools;
using Logger.Core;

namespace Logger.Tools
{
    public interface ITokenizer
    {
        //methods
        void Tokenize(string pattern);
        string MatchAndReplace(Enum token, string pattern, string value);
        string Format(Log log);
        //properties
        string[] Tokens { get;}
        string Pattern { get;}
    }
}
