using System;
using System.Collections.Generic;
using System.Text;
using Logger.Core;
using Logger.Tools;

namespace Logger.Layout.Formatter
{
    public class TextFormatter : Formatter
    {
        private const string defaultFormat = "$date$@$time$: [$level$]%t$content$";

        public TextFormatter(string pattern, ITokenizer tokenizer) : base (pattern, tokenizer)
        {
        }

        public TextFormatter(string pattern) : base(pattern)
        {
        }

        public TextFormatter() : base(defaultFormat)
        { }
        
        public override void Format(Log content)
        {
            Result = base.tokenizer.Format(content);
        }
    }
}
