using System;
using System.Text;
using System.Collections.Generic;
using Logger.Tools;
using Logger.Notifier;
using Logger.Core;

namespace Logger
{
    class Program
    {
        static void Main(string[] args)
        {
            //TODO: Write LogLog class for internal logging of unhandled exceptions and failures.
            string pattern = Console.ReadLine();
            //ITokenizer t = new Tokenizer(pattern);
            //t.SetValue(Formatter.TokenType.content, "hahaha");
            //ConsoleNotifier cn = new ConsoleNotifier();
            FileNotifier fn = new FileNotifier(true);
            //cn.Threshold = Level.All;

            //ILog logger = LogManager.GetLogger(new Logger.Program().GetType().ToString());

            LogInfo info = new LogInfo("this is the message", new Logger.Program().GetType(), Level.All, "thisLogger", new ArgumentException(), DateTime.Now, null);
            Log log = new Log(info);

            //logger.Debug("this is an exception message", new ArgumentException());

            //cn.Append(log);
            //cn.Append(log);
            fn.Append(log);
            fn.Append(log);
            fn.Exit();
        }
    }
}
