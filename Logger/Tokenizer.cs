using System;
using Logger.Tools;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;
using Logger.Core;

namespace Logger.Tools
{
    public class Tokenizer : ITokenizer
    {
        //private List<string> v_tokens;
        private IToken v_token;
        private string v_pattern;
        //private Regex tokenmatcher;
        

        /// <summary>
        /// default ctor.
        /// </summary>
        internal Tokenizer()
        {
            //v_tokenTable = new Hashtable();
            //v_tokens = new List<string>();
            //tokenmatcher = new Regex("\\$(\\w+)\\$", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.CultureInvariant);
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="pattern">pattern to tokenize</param>
        public Tokenizer(string pattern) : this()
        {
            this.v_pattern = pattern;
            v_token = new Token(v_pattern);
            v_token.Tokenize();
        }

        /// <summary>
        /// Format the output by replacing tokens with desired values.
        /// </summary>
        /// <param name="log">the log of values to use</param>
        /// <returns>formatted string</returns>
        public virtual string Format(Log log)
        {
            string outputFormat = Pattern;
            
            foreach (string s in Tokens)
            {
                try
                {
                    TokenType tok = (TokenType)Enum.Parse(typeof(TokenType), s, true);
                    switch (tok)
                    {
                        case TokenType.content:
                            outputFormat = v_token.MatchAndReplace(tok, outputFormat, log.Message.ToString());
                            break;
                        case TokenType.date:
                            outputFormat = v_token.MatchAndReplace(tok, outputFormat, log.LogInfo.TimeStamp.Date.ToShortDateString());
                            break;
                        case TokenType.level:
                            outputFormat = v_token.MatchAndReplace(tok, outputFormat, log.Level.Name);
                            break;
                        case TokenType.time:
                            outputFormat = v_token.MatchAndReplace(tok, outputFormat, log.LogInfo.TimeStamp.ToString("hh:mm:ss"));
                            break;
                        case TokenType.thread:
                            outputFormat = v_token.MatchAndReplace(tok, outputFormat, log.ThreadName);
                            break;
                        case TokenType.exception:
                            if (log.Exception != null)
                                outputFormat = v_token.MatchAndReplace(tok, outputFormat, log.Exception);
                            else
                                outputFormat = outputFormat.Replace(Token.ToString(tok.ToString()), string.Empty);
                            break;
                        case TokenType.machine:
                            outputFormat = v_token.MatchAndReplace(tok, outputFormat, System.Environment.MachineName);
                            break;
                        case TokenType.user:
                            if (log != null)
                                outputFormat = v_token.MatchAndReplace(tok, outputFormat, log.LogInfo.UserName);
                            else
                                outputFormat = v_token.MatchAndReplace(tok, outputFormat, System.Environment.UserName);
                            break;
                        case TokenType.locationinfo:
                            if (log != null)
                                outputFormat = v_token.MatchAndReplace(tok, outputFormat, log.LogInfo.LocationInfo.FullInfo);
                            else
                                outputFormat = v_token.MatchAndReplace(tok, outputFormat, LocationInfo.NA);
                            break;
                        case TokenType.classname:
                            if (log != null)
                                outputFormat = v_token.MatchAndReplace(tok, outputFormat, log.LogInfo.LocationInfo.ClassName);
                            else
                                outputFormat = v_token.MatchAndReplace(tok, outputFormat, LocationInfo.NA);
                            break;
                        case TokenType.filename:
                            if (log != null)
                                outputFormat = v_token.MatchAndReplace(tok, outputFormat, log.LogInfo.LocationInfo.FileName);
                            else
                                outputFormat = v_token.MatchAndReplace(tok, outputFormat, LocationInfo.NA);
                            break;
                        case TokenType.line:
                            if (log != null)
                                outputFormat = v_token.MatchAndReplace(tok, outputFormat, log.LogInfo.LocationInfo.LineNumber);
                            else
                                outputFormat = v_token.MatchAndReplace(tok, outputFormat, LocationInfo.NA);
                            break;
                        case TokenType.methodname:
                            if (log != null)
                                outputFormat = v_token.MatchAndReplace(tok, outputFormat, log.LogInfo.LocationInfo.MethodName);
                            else
                                outputFormat = v_token.MatchAndReplace(tok, outputFormat, LocationInfo.NA);
                            break;
                        default:
                            break;
                    }
                }
                catch
                {
                    /*ignore null or parse exception*/
                }
            }
            return outputFormat;
        }

        public virtual string MatchAndReplace(Enum tokenType, string pattern, string value)
        {
            return v_token.MatchAndReplace(tokenType, pattern, value);
        }

        /// <summary>
        /// Tokenize a string pattern.
        /// </summary>
        /// <param name="pattern"></param>
        public virtual void Tokenize(string pattern)
        {
            this.v_pattern = pattern;
            this.v_token = new Token(pattern);
            this.v_token.Tokenize();
        }
        
        /// <summary>
        /// Create a tokenizer based on a string patttern.
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static ITokenizer CreateTokenizer(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
                return null;
            ITokenizer tokenizer = new Tokenizer(pattern);
            return tokenizer;
        }

        /*
        /// <summary>
        /// Set the value of a token to a given value.
        /// </summary>
        /// <param name="tokenName">the name of the token</param>
        /// <param name="tokenValue">the value to set token as.</param>
        /// <returns>true if token is found and changed; false otherwise.</returns>
        public bool SetValue(Object tokenName, Object tokenValue)
        {
            if (this.v_tokenTable.Contains(tokenName))
            {
                if(string.IsNullOrEmpty(tokenValue.ToString()))
                {
                    return false;
                }
                this.v_tokenTable[tokenName] = tokenValue;
                return true;
            }
            return false;
        }
         

        /// <summary>
        /// Set the value of a token using the enumerator.
        /// </summary>
        /// <param name="tokenName"></param>
        /// <param name="tokenValue"></param>
        /// <returns></returns>
        public bool SetValue(Formatter.TokenType tokenName, Object tokenValue)
        {
            string name = Enum.GetName(typeof(Formatter.TokenType), tokenName);
            return SetValue(name, tokenValue);
        }
         */

        //properties
        public string[] Tokens
        {
            get { return v_token.Tokens; }
        }

        public string Pattern
        {
            get { return v_pattern; }
        }
    }
}
