using System;
using System.Collections.Generic;
using System.Text;

namespace Logger.Tools
{
    /// <summary>
    /// Token interface
    /// </summary>
    public interface IToken
    {
        void Tokenize();
        string[] Tokens { get;}
        bool Match(Enum tokenType);
        string MatchAndReplace(Enum tokenType, string pattern, string value);
        string MatchAndReplace(Enum tokenType, string pattern, object value);
    }

    public enum TokenType
    {
        date,
        content,
        level,
        thread,
        time,
        exception,
        machine,
        user,
        assembly,
        extension,
        locationinfo,
        filename,
        classname,
        methodname,
        line,
    }
}
