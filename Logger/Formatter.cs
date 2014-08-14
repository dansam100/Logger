using System;
using Logger.Core;
using System.Text;
using Logger.Tools;
using System.Collections.Generic;

namespace Logger.Layout.Formatter
{
    public abstract class Formatter : IFormatter
    {
        private string v_pattern;
        private string v_result;
        protected ITokenizer tokenizer;

        public Formatter(string pattern, ITokenizer tokenizer)
        {
            this.tokenizer = tokenizer;
            this.v_pattern = pattern;
        }

        public Formatter(string pattern)
        {
            this.v_pattern = pattern;
            tokenizer = new Tokenizer(pattern);
        }

        internal Formatter() { }


        public abstract void Format(Log content);

        //Properties
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

        protected string Result
        {
            get { return v_result; }
            set { v_result = value; }
        }

        public override string ToString()
        {
            return StringFormat.FormatOutput(v_result);
        }
    }
}
