using System;
using System.IO;
using System.Xml;
using System.Text;
using Logger.Core;
using Logger.Tools;
using Logger.Layout;
using Logger.Filter;
using System.Collections;
using Logger.Configuration;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Threading;

namespace Logger.Notifier
{
    /// <summary>
    /// Handles the appending process
    /// </summary>
    public delegate void MassAppendEventHandler(List<Logger.Notifier.LogEntry> buffer);
    
    /// <summary>
    /// Log modes.
    /// 
    /// Immediate -> used for console logs which can be sent right away and flushed.
    /// Chunk -> waits for a chunk of data. If a certain size is reached, data is logged and flushed.
    /// WaitLong -> Uses a timer to determine when to attempt to log and flush.
    /// </summary>
    public enum LogMode
    {
        Immediate,
        Chunk,
        WaitLong,
    }
    
    [Serializable]
    public abstract class MassNotifier : LogList, INotifier, IMassNotifier
    {
        private XmlNode doc;
        private ProgTimer timer;
        private Level v_threshold;
        private TextWriter v_mainOut;
        protected IConfigurator v_configurator;
        protected MassAppendEventHandler AppendHandler;
        protected Stream v_stream;
        protected ILayout v_layout;
        protected IFilter v_filter;

        protected LogMode v_logmode;
        protected LogQueue logQueue;

        protected int chunkSize = 0;
        protected int RefChunkSize = 1000;
        protected long MaxFileSize = long.MaxValue;

        protected string v_filename;
        protected string v_name;
        protected bool _MEGA_LOCK = false;
        protected bool v_closed, v_append;

        internal LogWriter v_writer;

        private ThreadStart writeStart;
        private Thread massThread;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="mainOut"></param>
        /// <param name="mode"></param>
        public MassNotifier(TextWriter mainOut, LogMode mode) : this()
        {
            v_mainOut = mainOut;
            v_logmode = mode;
            //GetModeAction();
        }

        public MassNotifier(ILayout layout) : this()
        {
            this.v_layout = layout;
        }

        public MassNotifier(TextWriter mainOut) : this()
        {
            v_mainOut = mainOut;
        }


        public MassNotifier()
        {
            //initialize writers, timers, layouts and log queue
            v_writer = new LogWriter(this);
            logQueue = new LogQueue();
            timer = new ProgTimer();
            this.v_configurator = new LayoutConfigurator();

            //setup writer thread stuff
            this.writeStart = new ThreadStart(this.MassAppend);
            this.massThread = new Thread(writeStart);
            this.massThread.IsBackground = true;

            //set other parameters
            v_append = true;
            v_logmode = LogMode.Immediate;
            //GetModeAction();
        }

        #region ThreadStuff...
        /// <summary>
        /// Issues a Wait on all active threads. Issues a time out if specified.
        /// default timeout: 2 minutes.
        /// </summary>
        /// <param name="timeout"></param>
        internal void WaitAll(int timeout)
        {
            try
            {
                //if (Events != null)
                //{
                //    if (timeout == 0)
                //    {
                //        if (WaitHandle.WaitAll(Events.ToArray(), 1000 * 120, false))
                //        {
                //            Console.WriteLine("wait completed");
                //        }
                //        else
                //        {
                //            if (WaitHandle.WaitAll(Events.ToArray(), timeout, false))
                //            {
                //                Console.WriteLine("wait completed");
                //            }
                //        }
                //    }
                //}
            }
            catch (System.ArgumentNullException e)
            {
                Console.WriteLine(e);
            }
        }

        public void Start()
        {
            this.massThread.Start();
        }
        #endregion

        #region ModeAction...
        private void GetModeAction()
        {
            timer.TimerFired += new LogTimerEventHandler(timer_TimerFired);
            switch (v_logmode)
            {
                case LogMode.WaitLong:
                    timer.AddTimer(TimerType.WaitLong);
                    timer.BeginTimer();
                    break;
                case LogMode.Chunk:
                    timer.AddTimer(TimerType.ChunkCheck);
                    timer.BeginTimer();
                    break;
            }
        }


        void timer_TimerFired(object o, TimerEventArgs e)
        {
            if (e != null)
            {
                switch (e.Type)
                {
                    case TimerType.WaitLong:
                        if (this.Count > 0)
                        {
                            if (this.AppendHandler != null)
                                this.AppendHandler(this);
                            else { WriteLogs(); }
                        }
                        break;
                    case TimerType.ChunkCheck:
                        if (this.RefChunkSize >= this.chunkSize)
                        {
                            if (this.AppendHandler != null)
                                this.AppendHandler(this);
                            else { WriteLogs(); }
                        }
                        break;
                }
            }
        }
        #endregion

        #region Clearing and Flushing...
        /// <summary>
        /// Flush the stream
        /// </summary>
        public void Flush()
        {
            lock (this)
            {
                LogList entries = new LogList(this);
                this.Clear();
                logQueue.Enqueue(entries);
                WriteLogs();
                //foreach (LogEntry l in entries)
                //{
                //    Writer.WriteLine(l.Content);
                //}
                //Writer.Flush();
            }
        }

        public void ClearBuffer()
        {
            lock (this)
            {
                this.Clear();
            }
        }
        #endregion

        #region Configuration...
        private void Configure()
        {
            foreach (Object param in v_configurator.Properties.Keys)
            {
                switch (param.ToString())
                {
                    case "layout":
                        if (doc != null)
                            ConfigureLayout(v_configurator.Properties[param], doc);
                        else ConfigureLayout(v_configurator.Properties[param], null);
                        break;
                    case "append":
                        if (v_configurator.Properties[param].ToString().ToLower().CompareTo("false") == 0)
                        {
                            v_append = false;
                        }
                        break;
                    case "MaxFileSize":
                        long.TryParse(v_configurator.Properties[param].ToString(), out MaxFileSize);
                        break;
                    case "filename":
                        v_filename = v_configurator.Properties[param].ToString();
                        break;
                    case "name":
                        v_name = v_configurator.Properties[param].ToString();
                        break;
                    default:
                        break;
                }
            }
            try
            {
                InitWriterStream();

                IMassNotifier massnotifier = this as IMassNotifier;
                if(massnotifier != null)
                    massnotifier.Start();

                //IMassAppender massappender = Layout as IMassAppender;
                //if (massappender != null)
                //   this.AppendHandler = massappender.MassAppend;
            }
            catch(Exception e){ }
        }

        /// <summary>
        /// Configure using xmlelement.
        /// </summary>
        /// <param name="element"></param>
        public virtual void Configure(XmlElement element)
        {
            if (element != null)
            {
                v_configurator.Configure(element);
                this.Configure();
            }
        }

        public virtual void Configure(XmlElement element, XmlElement document)
        {
            if (element != null)
            {
                doc = (XmlNode)document;
                v_configurator.Configure(element);
                this.Configure();
            }
        }

        private void ConfigureLayout(object layout, XmlNode node)
        {
            Hashtable ht = (Hashtable)layout;
            if (ht.Contains("type") && ht["type"] != null)
            {
                //this.v_layout = (ILayout)Assembly.GetAssembly(
                //                    Type.GetType(ht["type"].ToString())).CreateInstance(ht["type"].ToString());
                this.v_layout = (ILayout)Activator.CreateInstance(Type.GetType(ht["type"].ToString(), true));
                this.v_layout.Name = ht["name"].ToString();
                this.v_layout.Configure((XmlElement)node);
            }
            else if (this.v_layout == null && ht.Contains("name") && ht["name"] != null)
            {
                if (node != null && node.HasChildNodes)
                {
                    XmlNode n = node.SelectSingleNode(string.Format("//layout.config[@name = '{0}']", ht["name"]));
                    ht["type"] = n.Attributes["type"].Value;
                    ConfigureLayout(layout, node);
                }
            }
        }

        /// <summary>
        /// Seeks out configuration path and configures the notifier.
        /// </summary>
        public virtual void LoadConfiguration()
        {
            string textnotifier = string.Format("logger/logger.configuration/notifier[@type='{0}']", this.GetType().FullName);
            try
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc = new XmlDocument();
                xDoc.Load(LogManager.ConfigLocation);
                doc = (XmlNode)xDoc;
                XmlNode node = doc.SelectSingleNode(textnotifier);
                v_configurator.Configure((XmlElement)node);
                this.Configure();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        #endregion

        #region On Exit...
        protected virtual void OnExit()
        {
            if (v_mainOut != null && !v_closed)
            {
                if(timer != null)
                    timer.StopTimer();

                //if (AppendHandler != null)
                //{
                //    AppendHandler(v_buffer);
                //}
                //else MassAppend();

                this.Flush();

                v_writer.Close();

                if (!Stream.CanWrite)
                {
                    ReCreateStream();
                }

                Layout.Finalize(Writer);
                v_mainOut.Close();
            }
        }


        public virtual void Exit()
        {
            this.OnExit();
            this.v_closed = true;
            //this.massThread.Abort();
        }
        #endregion

        #region Log Writing...

        public void Append(LogEntry log)
        {
            this.chunkSize++;
            this.Add(log);
        }

        public virtual void Append(Log log)
        {
            if (!v_closed)
            {
                v_layout.Format(v_writer, log);
                if (v_logmode == LogMode.Immediate)
                {
                    WriteLogs();
                }
            }
            else
            {
                new ObjectDisposedException(string.Format("{0}/({1})", this.Name, "Writer"));
            }
        }


        /// <summary>
        /// Write logs and set event completed.
        /// </summary>
        /// <param name="list">list of log entries</param>
        /// <param name="mre">event</param>
        private void WriteLogs(LogList list)
        {
            int i = 0;
            foreach (LogEntry l in list)
            {
                Writer.WriteLine(l.Content);
                Writer.Flush();
                
                //while (_MEGA_LOCK) Thread.Sleep(1000);
                //lock (Stream)
                //{
                //    if (v_closed || Stream == null || !Stream.CanWrite)
                //    {
                //        ReCreateStream();
                //    }
                //    lock (Writer)
                //    {
                //        Writer.WriteLine(l.Content);
                //        Writer.Flush();
                //    }
                //}
                //list.Length -= l.Length;
                ////if (_MEGA_LOCK)
                ////{
                ////    lock (v_buffer)
                ////    {
                ////        LogEntry[] entries = new LogEntry[list.Count - (i + 1) + v_buffer.Count];
                ////        v_buffer.CopyTo(0, entries, 0, v_buffer.Count);
                ////        list.CopyTo(i, entries, v_buffer.Count, list.Count - (i + 1));
                ////        v_buffer = new List<LogEntry>(entries);
                ////    }
                ////    break;
                ////}
                //this.CurrentChunkSize = list.Length;
                //i++;
            }
        }

        /// <summary>
        /// Write Logs callback
        /// </summary>
        private void WriteLogs()
        {
            LogList list = null;
            if(Monitor.TryEnter(this, 2000))
            {
                list = new LogList(this);
                Monitor.Exit(this);
            }
            if (list != null && list.Count > 0)
            {
                list.Length = this.v_writer.Length;
                this.v_writer.Clear();
                lock (logQueue)
                {
                    this.logQueue.Enqueue(list);
                }
                massThread.Resume();
            }
        }

        /// <summary>
        /// Convert the stored log into desired log
        /// </summary>
        public virtual void MassAppend()
        {
            while (true)
            {
                if (logQueue.Count > 0)
                {
                    WriteLogs(logQueue.Dispatch());
                }
                else massThread.Suspend();
            }
        }
        #endregion

        #region AbstractStuff...
        /// <summary>
        /// In case the stream is lost, this will recreate the stream and writer for logging purposes.
        /// </summary>
        public abstract void ReCreateStream();

        /// <summary>
        /// initialize writer stream
        /// </summary>
        internal abstract void InitWriterStream();

        /// <summary>
        /// Get a the writer stream
        /// </summary>
        public abstract void GetWriterStream(params bool[] force);

        /// <summary>
        /// hash code
        /// </summary>
        /// <returns></returns>
        public abstract int OtherHashCode();
        #endregion

        #region Properties...
        /// <summary>
        /// Get the size of the chunk.
        /// </summary>
        /// <returns></returns>
        protected virtual double CurrentChunkSize
        {
            get;
            set;
        }

        protected List<LogEntry> LogList
        {
            get { return new List<LogEntry>(this); }
        }

        public Stream Stream
        {
            get { return v_stream; }
            set { this.v_stream = value; }
        }

        public bool IsClosed
        {
            get { return v_closed; }
        }

        public virtual bool AppendContent
        {
            get { return v_append; }
            set { v_append = value; }
        }

        public virtual String Name
        {
            get { return v_name; }
            set { v_name = value; }
        }

        public virtual ILayout Layout
        {
            get { return v_layout; }
            set { v_layout = value; }
        }

        public virtual TextWriter Writer
        {
            get
            {
                if(v_mainOut != null)
                    return v_mainOut;
                else return (v_mainOut = new StreamWriter(v_stream));
            }
            set { v_mainOut = value; }
        }

        public LogMode Mode
        {
            get { return v_logmode; }
            set { v_logmode = value; GetModeAction(); }
        }

        public virtual Level Threshold
        {
            get { return v_threshold; }
            set { v_threshold = value; }
        }

        public virtual IFilter Filter
        { get { return v_filter; } }
        #endregion
    }

    #region LogEntry class...
    /// <summary>
    /// Log entry struct declaration.
    /// </summary>
    public struct LogEntry
    {
        private string log;
        private int start;
        private float length;

        internal LogEntry(string entry, int start)
        {
            log = entry;
            this.start = start;
            length = Encoding.ASCII.GetByteCount(log);
        }

        public string Content
        {
            get { return log; }
        }

        public float Length
        {
            get { return length; }
        }
    }
    #endregion
}
