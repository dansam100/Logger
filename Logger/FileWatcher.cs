using System;
using System.IO;
using System.Text;
using System.ComponentModel;
using System.Collections.Generic;
using System.Security.Permissions;


namespace Logger.Configuration.Watcher
{
    public delegate void FileReplaceHandler(object sender, FileSystemEventArgs e);
    public delegate void FileRenameHandler(object sender, RenamedEventArgs e);
    public delegate void FileChangedHandler(object sender, FileSystemEventArgs e);
    
    [Flags()]
    public enum FileType
    {
        Text,
        Document,
        Xml,
        Code,
        Binary,
        All,
    }
    
    /// <summary>
    /// File watcher class.
    /// </summary>
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public class FileWatcher
    {
        private FileSystemWatcher watcher;
        private BackgroundWorker worker;
        private FileType v_fileType;
        public FileReplaceHandler ReplaceHandler;
        public FileRenameHandler RenameHandler;
        public FileChangedHandler FileChangedHandler;

        public FileWatcher()
        {
            this.watcher = new FileSystemWatcher();
            this.worker = new BackgroundWorker();
            this.watcher.NotifyFilter = this.All;
            RegisterEvents();
        }

        public FileWatcher(params NotifyFilters[] notifierFilters)
        {
            this.worker = new BackgroundWorker();
            this.watcher = new FileSystemWatcher();
            foreach (NotifyFilters filter in notifierFilters)
                watcher.NotifyFilter = watcher.NotifyFilter | filter;
            RegisterEvents();
        }

        public FileWatcher(string path, params NotifyFilters[] notifierFilters) : this(notifierFilters)
        {
            SetWatcherPath(path);
        }

        public NotifyFilters All
        {
            get
            {
                return (NotifyFilters.Attributes |
                    NotifyFilters.CreationTime |
                    NotifyFilters.DirectoryName |
                    NotifyFilters.FileName |
                    NotifyFilters.LastAccess |
                    NotifyFilters.LastWrite |
                    NotifyFilters.Security |
                    NotifyFilters.Size);
            }
        }

        public void SetWatcherPath(string path)
        {
            if (File.Exists(path))
            {
                FileInfo fi = new FileInfo(path);
                watcher.Path = fi.Directory.FullName;
                watcher.Filter = fi.Name;
                GetFileType(fi);
            }
            else if (Directory.Exists(path))
            {
                watcher.Path = path;
                watcher.Filter = "*";
                v_fileType = FileType.All;
            }
        }

        private void RegisterEvents()
        {
            this.worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            this.worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            this.watcher.Changed += new FileSystemEventHandler(FileWatcher_Changed);
            this.watcher.Deleted += new FileSystemEventHandler(FileWatcher_Changed);
            this.watcher.Renamed += new RenamedEventHandler(FileWatcher_Changed);
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //BackgroundWorker bw = sender as BackgroundWorker;
            if (e != null)
            {
                if (e.Cancelled)
                    this.watcher.EnableRaisingEvents = false;
            }
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;
            if (bw != null && e != null)
            {
                //this.BeginMonitor();
                this.watcher.EnableRaisingEvents = true;
            }
            if (bw.CancellationPending)
                e.Cancel = true;
        }

        /// <summary>
        /// EventHandler: Occurs when the file being watched is renamed, changed or deleted.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        void FileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            string file = e.Name;
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Deleted:
                    if (ReplaceHandler != null)
                        ReplaceHandler(sender, e);
                    break;
                case WatcherChangeTypes.Renamed:
                    if (RenameHandler != null)
                        RenameHandler(sender, (RenamedEventArgs)e);
                    else UnRename((RenamedEventArgs)e);
                    break;
                case WatcherChangeTypes.Changed:
                    if (FileChangedHandler != null)
                        FileChangedHandler(sender, e);
                    break;
                default:
                    break;
            }
        }


        private void UnRename(RenamedEventArgs e)
        {
            try
            {
                string file = e.OldName;
                string newname = e.FullPath;
                FileInfo fi = new FileInfo(newname);
                fi.CopyTo(file, true);
            }
            catch(Exception ex)
            {
                //LogLog
            }
        }

        public void BeginMonitor()
        {
            //this.watcher.BeginInit();
            this.worker.RunWorkerAsync();
        }

        public void EndMonitor()
        {
            //this.watcher.EndInit();
            if (this.worker.IsBusy)
                this.worker.CancelAsync();
        }

        void GetFileType(FileInfo info)
        {
            switch (info.Extension.ToLower())
            {
                case ".xml":
                    v_fileType = FileType.Xml;
                    break;
                case ".cs":
                case ".java":
                case ".cpp":
                case ".h":
                case ".js":
                case ".rb":
                case ".cmd":
                case ".com":
                case ".bat":
                    v_fileType = FileType.Code;
                    break;
                case ".pdf":
                case ".doc":
                case ".xls":
                case ".csv":
                case ".ppt":
                    v_fileType = FileType.Document;
                    break;
                case ".exe":
                    v_fileType = FileType.Binary;
                    break;
                default:
                    v_fileType = FileType.Text;
                    break;
            }
        }

        /// <summary>
        /// Returns the filetype
        /// </summary>
        public FileType FileType
        {
            get { return v_fileType; }
        }
    }
}
