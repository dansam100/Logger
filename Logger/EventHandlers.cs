using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Logger.Tools
{
    public class TimerEventArgs : EventArgs
    {
        public TimerType Type;
        public TimerEventArgs(Enum timertype)
        {
            this.Type = (TimerType)Enum.Parse(typeof(TimerType), timertype.ToString());
        }
    }

    public delegate void LogTimerEventHandler(object o, TimerEventArgs e);


    public class ManualResetEvents : System.Collections.Generic.List<ManualResetEvent>
    {
        private int max = 10, index = 0;
        
        public new void Add(ManualResetEvent item)
        {
            lock (this)
            {
                if (base.Count < max)
                    base.Add(item);
                else
                {
                    index = (index >= max) ? 0 : index;
                    base[index] = item;
                    index++;
                }
            }
        }
    }
}
