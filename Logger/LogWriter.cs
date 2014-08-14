using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Logger.Notifier;
using System.ComponentModel;

namespace Logger.Notifier
{
    internal class LogWriter : StringWriter
    {
        private IMassNotifier v_massLog;
        //private BackgroundWorker sizer;
         
        private float v_length;

        public LogWriter(IMassNotifier massLog)
        {
            this.v_massLog = massLog;
            /*
            sizer = new BackgroundWorker();
            sizer.DoWork += new DoWorkEventHandler(sizer_DoWork);
            sizer.RunWorkerCompleted += new RunWorkerCompletedEventHandler(sizer_RunWorkerCompleted);
            */
        }

        #region junk
        /*
        void sizer_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }

        void sizer_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker b = sender as BackgroundWorker;
            if (b != null && e != null)
            {
                string content = (string)e.Argument;
                lock (this)
                {
                    //string location = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "tmp");
                    //FileStream f = System.IO.File.Create(location);
                    //StreamWriter s = new StreamWriter(f);
                    //s.WriteLine(content);
                    //s.Close();
                    //System.IO.FileInfo fi = new FileInfo(location);
                    //v_length += (long)(fi.Length/1024d);
                    //fi.Delete();
                    
                    v_length += (double)Encoding.GetByteCount(content)/1024L;
                }
            }
            if (b.CancellationPending)
            {
                e.Cancel = true;
            }
        }
        */
        #endregion

        #region public functions...
        public override void Write(string value)
        {
            Logger.Notifier.LogEntry entry =
                new Logger.Notifier.LogEntry(value, v_massLog.Count);
            lock (v_massLog)
            {
                v_massLog.Append(entry);
            }
            if(v_massLog.Mode != LogMode.Immediate)
                v_length += entry.Length;
        }

        public override void WriteLine(string value)
        {
            Write(value);
        }

        public override void Flush()
        {
            this.v_massLog.Flush();
            base.Flush();
            this.v_length = 0;
        }

        internal void Clear()
        {
            base.Flush();
            this.v_length = 0;
            this.v_massLog.ClearBuffer();
        }
        #endregion

        #region Properties...
        public float Length
        {
            get { return this.v_length; }
        }

        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }
        #endregion
    }
}
