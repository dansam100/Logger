using System;
using System.Collections.Generic;
using System.Text;
using Logger.Core;
using Logger.Layout;
using Logger.Tools;
using System.Reflection;
using System.IO;
using Logger.Configuration.Watcher;

namespace Logger.Notifier
{
    public class FileNotifier : Notifier
    {
        FileWatcher fileWatcher;
        private int SIZE_THRESHOLD;

        public FileNotifier(bool init)
            : base(init)
        {
            //GetWriterStream();
            //this.Layout.LogStream = base.Stream;
            //this.Layout.LogFile = base.v_filename;
        }

        public FileNotifier()
            : base(){}

        public FileNotifier(ILayout layout)
            : base(layout) { }


        #region Streamhandler...
        /// <summary>
        /// initialize the writer stream
        /// </summary>
        internal override void InitWriterStream()
        {
            //TOD0: make this concatenate to the current file somehow since logging is done daily.
            //put an attribute in config (append should work) to specify if new file should be generated for each interrupt of service or not.
            if (v_filename != null && this.Layout != null)
            {
                ITokenizer t = Tokenizer.CreateTokenizer(v_filename);
                if (t.Tokens.Length > 0)
                {
                    v_filename = t.MatchAndReplace(TokenType.date, v_filename, DateTime.Now.ToString("dd-MM-yyyy"));
                    v_filename = t.MatchAndReplace(TokenType.assembly, v_filename, Assembly.GetEntryAssembly().GetName().Name);
                    v_filename = t.MatchAndReplace(TokenType.extension, v_filename,
                        (this.Layout.ContentType.Split('/').Length > 1) ? this.Layout.ContentType.Split('/')[1] : this.Layout.ContentType);
                }
                base.Mode = LogMode.Immediate;
                Layout.LogFile = v_filename;
                SIZE_THRESHOLD = (int)(0.15 * MaxFileSize) + base.v_writer.Encoding.GetByteCount(Layout.StartValue)/1024; 
            }
            GetWriterStream(true);
        }

        public override void GetWriterStream(params bool[] force)
        {
            if (!string.IsNullOrEmpty(v_filename))
            {
                //v_stream = FileHandler.GetFileWriteHandle(v_filename, Layout.NeedsStream);
                if(force.Length > 0)
                    v_stream = FileHandler.GetFileWriteHandle(v_filename, force[0]);
                else v_stream = FileHandler.GetFileWriteHandle(v_filename, false);
                Layout.LogStream = v_stream;
                Layout.LogFile = v_filename;
                TextWriter tw = new StreamWriter(v_stream);

                Layout.InitLayout(tw);
                Writer = tw;

                Writer.Flush();

                //Create a file watcher.
                fileWatcher = new FileWatcher(v_filename, NotifyFilters.Size);
                fileWatcher.FileChangedHandler = this.OnFileLimitReached;
                fileWatcher.BeginMonitor();
            }
        }

        public override void ReCreateStream()
        {
            v_stream = FileHandler.GetFileWriteHandle(v_filename, false);
            TextWriter tw = new StreamWriter(v_stream);
            Writer = tw;
        }
        #endregion

        #region SizeChecking....
        /// <summary>
        /// Event Handler: When the file size changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFileLimitReached(object sender, FileSystemEventArgs e)
        {
            if (e != null)
            {
                if (e.ChangeType == WatcherChangeTypes.Changed)
                {
                    lock (Stream)
                    {
                        try
                        {
                            double filelength = 0;
                            try
                            {
                                filelength = (double)FileHandler.GetFileSize(v_filename)/1024;
                            }
                            catch
                            {
                                Stream.Close();
                                filelength = (double)FileHandler.GetFileSize(v_filename)/1024;
                            }
                            finally
                            {
                                //if (!Stream.CanWrite)
                                //{
                                //    ReCreateStream();
                                //}
                            }
                            
                            if ((filelength + this.ApproximateSize) >= MaxFileSize)
                            {
                                if (!_MEGA_LOCK)
                                {
                                    _MEGA_LOCK = true;
                                    Writer.Flush();
                                    Layout.Finalize(Writer);
                                   
                                    Stream = FileHandler.Truncate(v_filename);
                                    this.v_filename = FileHandler.DuplicateLogName;

                                    GetWriterStream();

                                    _MEGA_LOCK = false;
                                    Console.WriteLine("file size exceeded here");
                                }
                            }
                        }
                        catch
                        {
                            if (!Stream.CanWrite)
                            {
                                //Stream = FileHandler.GetFileWriteHandle(v_filename, Layout.NeedsStream);
                                ReCreateStream();
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Configuration...
        public override void Configure(System.Xml.XmlElement element)
        {
            base.Configure(element);
            SIZE_THRESHOLD = (int)(0.15 * MaxFileSize);
        }

        public override void Configure(System.Xml.XmlElement element, System.Xml.XmlElement document)
        {
            base.Configure(element, document);
            SIZE_THRESHOLD = (int)(0.15 * MaxFileSize);
        }
        #endregion

        #region junk...
        /*
        public override void MassAppend()
        {
            IMassAppender massappender = Layout as IMassAppender;
            if (massappender != null)
            {
                if (!v_closed)
                {
                    if (v_buffer.Count > 0)
                    {
                        List<LogEntry> list = null;
                        lock (v_buffer)
                        {
                            list = new List<LogEntry>(v_buffer);
                        }
                        string result = massappender.MassAppend(list);
                        Writer.Close();
                        Writer = new StreamWriter(FileHandler.GetFreshWriteHandle(v_filename));
                        Writer.WriteLine(result);
                        Flush();
                        Writer.Close();
                    }
                }
            }
            else
            {
                base.MassAppend();
            }
        }
        */
        #endregion

        #region Properties...

        public override void Append(Log log)
        {
            base.Append(log);
        }

        protected override void Append(List<Log> logs)
        {
            foreach (Log log in logs)
            {
                this.Append(log);
            }
        }

        public override void Exit()
        {
            if(fileWatcher != null)
                fileWatcher.EndMonitor();
            base.Exit();
        }
        #endregion

        #region Properties...

        //Properties
        public override System.IO.TextWriter Writer
        {
            get
            {
                return base.Writer;
            }
        }

        /// <summary>
        /// approx. size
        /// </summary>
        private double ApproximateSize
        {
            get { return SIZE_THRESHOLD + this.CurrentChunkSize; }
        }
        #endregion
    }
}
