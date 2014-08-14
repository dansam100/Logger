using System;
using System.Collections.Generic;
using System.Text;
using Logger.Core;
using System.IO;
using System.Xml;
using System.Collections;
using System.Reflection;
using Logger.Tools;
using Logger.Configuration;
using Logger.Layout;
using Logger.Filter;

namespace Logger.Notifier
{
    //TODO: use a stringwriter to write the output onto a string till certain time limit and then write all within string buffer to correct.
    //Derived classes will specify if they want to use this functionality by specifying a time limit.
    //this way, if using FileNotifier, file size can be checked and xml document can be used instead of writer.
    //this style also makes DB logging and other logging much faster.
    //the log files can be watched or locked via a stream if necessary to prevent errors.
    //Derived classes will specify what writer they want to use otherwise (eg: Console.Out for ConsoleNotifier).
    //Use ProgTimer principle from hasher program.
    
    public abstract class Notifier : MassNotifier, INotifier
    {
        //methods
        public Notifier(ILayout layout) : base(layout)
        { 
        }

        public Notifier()
        {
        }

        public Notifier(bool init)
        {
            LoadConfiguration();
        }

        protected virtual void Append(List<Log> logs)
        {
            foreach (Log log in logs)
            {
                this.Append(log);
            }
        }

        /// <summary>
        /// Other hashcode for checking notifier resemblance.
        /// </summary>
        /// <returns></returns>
        public override int OtherHashCode()
        {
            int hashcode = this.Name.GetHashCode();
            hashcode += this.GetType().GetHashCode();
            return hashcode;
        }
    }
}
