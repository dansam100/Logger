using System;
using System.Collections.Generic;
using System.Text;
using Logger.Core;
using Logger.Layout;

namespace Logger.Notifier
{
    public class ConsoleNotifier : Notifier, INotifier
    {
        public bool WriteToStandardErrorOutput = false;

        public ConsoleNotifier()
            : base()
        {
            base.Stream = Console.OpenStandardOutput();
        }

        public override void Append(Log log)
        {
            base.Append(log);
        }

        public override void GetWriterStream(params bool[] force)
        {
            if (!this.WriteToStandardErrorOutput)
                base.Writer = Console.Out;
            else
                base.Writer = Console.Error;
        }

        internal override void InitWriterStream()
        {
            GetWriterStream();
            Mode = LogMode.Immediate;
            Layout.InitLayout(Writer);
        }

        protected override void Append(List<Log> logs)
        {
            foreach (Log log in logs)
            {
                this.Append(log);
            }
        }

        public override void ReCreateStream()
        {
            Stream = Console.OpenStandardOutput();
            GetWriterStream();
        }

        public override bool AppendContent
        {
            get
            {
                return true;
            }
        }
    }
}
