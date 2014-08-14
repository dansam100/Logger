using System;
using System.Collections.Generic;
using System.Text;

namespace Logger.Tools
{
    public class LogQueue
    {
        protected IList<LogList> queue;
        public LogQueue()
        {
            queue = new List<LogList>();
        }

        public void Enqueue(LogList loglist)
        {
            queue.Add(loglist);
        }

        public LogList Dispatch()
        {
            LogList list = queue[0];
            queue.RemoveAt(0);
            return list;
        }

        public int Count
        {
            get { return queue.Count; }
        }
    }

    /// <summary>
    /// Log list
    /// </summary>
    public class LogList : List<Logger.Notifier.LogEntry>
    {
        /// <summary>
        /// copy ctor
        /// </summary>
        /// <param name="list">list to copy</param>
        public LogList(LogList list) : base( list ){}

        /// <summary>
        /// default ctor
        /// </summary>
        public LogList(){}

        public float GetLength()
        {
            float length = 0;
            foreach (Logger.Notifier.LogEntry entry in this)
            {
                length += entry.Length;
            }
            return Length = length;
        }

        public float Length
        {
            get; set;
        }
    }
}
