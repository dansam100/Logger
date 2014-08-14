 using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace Logger.Tools
{
    public class Token : IToken
    {
        private static string DEFAULT_TOKEN = "$";
        private string v_pattern;
        private Dictionary<string, Regex> v_tokens;
        private Hashtable v_tokenTable;
        private List<Regex> v_matchers;

        public static List<string> SpecialTokens = new List<string>();
        static Token()
        {
            string[] specialtokens = new string[] { "$", "^", "(", "[", "]", ")", "*", ".", "+" };
            SpecialTokens.AddRange(specialtokens);
        }

        public Token(string pattern, params string[] tokens)
            : this()
        {
            this.v_pattern = pattern;
            foreach (string token in tokens)
            {
                string format = string.Format("{0}(\\w+){0}", Escape(token));
                v_matchers.Add(new Regex(format, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.CultureInvariant));
            }
        }

        public Token(string pattern)
            : this()
        {
            this.v_pattern = pattern;
        }

        private Token()
        {
            this.v_tokens = new Dictionary<string, Regex>();
            v_tokenTable = new Hashtable();
            this.v_matchers = new List<Regex>();
            string format = string.Format("{0}(\\w+){0}", Escape(DEFAULT_TOKEN));
            this.v_matchers.Add(new Regex(format, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.CultureInvariant));
        }

        /// <summary>
        /// List of tokens within the current pattern.
        /// </summary>
        public string[] Tokens
        {
            get { return new List<string>(this.v_tokens.Keys).ToArray(); }
        }

        /// <summary>
        /// Checks if there is a match for a certain token within the pattern.
        /// </summary>
        /// <param name="tokenType">the token to check existence of.</param>
        /// <returns>true if token is found; false otherwise</returns>
        public virtual bool Match(Enum tokenType)
        {
            if (this.v_tokens.ContainsKey(tokenType.ToString()))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Match and replace a token of string equal to tokentype.tostring() with specified value
        /// within a pattern.
        /// </summary>
        /// <param name="tokenType">Enumerator of tokens</param>
        /// <param name="pattern">pattern to perform replacement on</param>
        /// <param name="value">value to replace with</param>
        /// <returns>pattern with token replaced with value.</returns>
        public virtual string MatchAndReplace(Enum tokenType, string pattern, object value)
        {
            return MatchAndReplace(tokenType, pattern, value.ToString());
        }

        /// <summary>
        /// Match and replace a token of string equal to tokentype.tostring() with specified value
        /// within a pattern.
        /// </summary>
        /// <param name="tokenType">Enumerator of tokens</param>
        /// <param name="pattern">pattern to perform replacement on</param>
        /// <param name="value">value to replace with</param>
        /// <returns>pattern with token replaced with value.</returns>
        public virtual string MatchAndReplace(Enum tokenType, string pattern, string value)
        {
            if (this.v_tokens.ContainsKey(tokenType.ToString()) && value != null)
            {
                pattern = v_tokens[tokenType.ToString()].Replace(pattern, value, int.MaxValue);
            }
            else
                pattern = v_tokens[tokenType.ToString()].Replace(pattern, "N/A", int.MaxValue);
            return pattern;
        }

        /// <summary>
        /// Start tokenizing a given pattern
        /// </summary>
        public virtual void Tokenize()
        {
            foreach (Regex r in this.v_matchers)
            {
                MatchCollection m = r.Matches(v_pattern);
                IEnumerator matchEnum = null;
                IEnumerator matchColEnum = null;
                matchColEnum = m.GetEnumerator();
                while (matchColEnum.MoveNext())
                {
                    matchEnum = ((Match)matchColEnum.Current).Captures.GetEnumerator();
                    while (matchEnum.MoveNext() && ((Match)matchColEnum.Current).Success)
                    {
                        string format = r.ToString().Replace("\\w+", ((Match)matchColEnum.Current).Groups[1].Value);
                        this.v_tokens[((Match)matchColEnum.Current).Groups[1].Value] = new Regex(format, r.Options);
                        v_tokenTable[((Match)matchColEnum.Current).Groups[1].Value] = string.Empty;
                    }
                }
            }
        }

        private static string Escape(string token)
        {
            if (SpecialTokens.Contains(token.Trim()))
            {
                return "\\" + token.Trim();
            }
            return token;
        }


        public static string ToString(string tokenvalue)
        {
            return string.Format("{0}{1}{0}", DEFAULT_TOKEN, tokenvalue);
        }
    }    
}
