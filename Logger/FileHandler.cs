using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Logger.Tools
{
    public sealed class FileHandler
    {
        public const string NameExtenderString = "_";
        public static string DuplicateLogName = string.Empty;

        public static Stream GetFileWriteHandle(string filename)
        {
            try
            {

                try
                {
                    FileInfo fi = new FileInfo(filename);
                    DirectoryInfo di = fi.Directory;
                    if (fi.Exists)
                    {
                        int count;
                        string file_wildcard = fi.Name.Replace(filename, fi.Name.Replace(fi.Extension, "_*" + fi.Extension));
                        //string file_wildcard = string.Format("{0}*", filename);
                        FileInfo[] files = di.GetFiles(file_wildcard);
                        count = files.Length;
                        string newname = Path.Combine(di.FullName, fi.Name.Replace(fi.Extension,
                            NameExtenderString + count.ToString() + fi.Extension));

                        fi.CopyTo(newname, true);
                        fi.Delete();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                return new FileStream(filename,
                    FileMode.Create | FileMode.Append, FileAccess.Write,
                        FileShare.ReadWrite | FileShare.Delete);
            }
            catch (DirectoryNotFoundException dnf)
            {
                Console.WriteLine("Could not find directory. Creating one!");
                CreatePath(filename);
                return GetFileWriteHandle(filename);
            }
        }

        public static Stream GetFileWriteHandle(string filename, bool force)
        {
            try
            {
                if (force)
                {
                    return GetFileWriteHandle(filename);
                }
                return new FileStream(filename, FileMode.Create | FileMode.Append, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete);
            }
            catch (DirectoryNotFoundException)
            {
                CreatePath(filename);
                return GetFileWriteHandle(filename, force);
            }
        }


        private static void CreatePath(string path)
        {
            FileInfo fi = new FileInfo(path);
            DirectoryInfo di = fi.Directory;
            if(!di.Exists)
                di.Create();
        }

        /// <summary>
        /// Get a fresh file handle.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static Stream GetFreshWriteHandle(string filename)
        {
            return new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete);
        }

        public static Stream GetFileReadHandle(string filename)
        {
            try
            {
                if(filename != null)
                    return new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete);
            }
            catch(Exception e)
            {
            }
            return null;
        }

        /*
        public static Stream GetAppendStream(string v_filename, string endtokens)
        {
            StreamReader reader = null;
            Stream s = null;
            string content = string.Empty;
            try
            {
                s = new FileStream(v_filename, FileMode.OpenOrCreate, FileAccess.Read);
                reader =  new StreamReader(s);
                content = reader.ReadToEnd();
                content.Replace(endtokens, string.Empty);
                reader.Close();
               
            }
            catch (Exception e){ }
            s = GetFileHandle(v_filename, false);
            StreamWriter sw = new StreamWriter(s);
            sw.Write(content);
            return s;
        }
        */

        public static long GetFileSize(string filename)
        {
            try
            {
                FileInfo fi = new FileInfo(filename);
                return fi.Length;
            }
            catch { throw; }
            
        }

        /// <summary>
        /// truncate a log file with a different name.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static Stream Truncate(string filename)
        {
            string newname, nextname = string.Empty;
            try
            {
                FileInfo fi = new FileInfo(filename);
                if (fi.Exists)
                {
                    string file_wildcard = fi.Name.Replace(fi.Extension, "-log*" + fi.Extension);
                    DirectoryInfo di = fi.Directory;
                    FileInfo[] files = di.GetFiles(file_wildcard);
                    int count = files.Length;

                    if (count <= 0)
                    {
                        newname = Path.Combine(di.FullName,
                            fi.Name.Replace(fi.Extension, "-log" + (++count).ToString() + fi.Extension));
                    }
                    else
                    {
                        newname = Path.Combine(di.FullName,
                            fi.Name.Replace(fi.Extension, "-log" + count.ToString() + fi.Extension));
                    }

                    if (fi.Name.CompareTo(newname) != 0)
                    {
                        fi.CopyTo(newname, true);
                        fi.Delete();
                    }
                    
                    nextname = Path.Combine(di.FullName,
                        fi.Name.Replace(fi.Extension, "-log" + (++count).ToString() + fi.Extension));
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
            DuplicateLogName = nextname;
            return GetFileWriteHandle(nextname, false);
        }
    }
}
