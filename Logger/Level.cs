using System;
using System.Collections.Generic;
using System.Text;

namespace Logger.Core
{
    public sealed class Level : IComparable
    {
        public static readonly Level Alert = new Level(LevelType.Alert, "ALERT");
        public static readonly Level All = new Level(LevelType.All, "ALL");
        public static readonly Level Debug = new Level(LevelType.Debug, "DEBUG");
        public static readonly Level Error = new Level(LevelType.Error, "ERROR");
        public static readonly Level Fatal = new Level(LevelType.Fatal, "FATAL");
        public static readonly Level Info = new Level(LevelType.Info, "INFO");
        public static readonly Level Warn = new Level(LevelType.Warn, "WARN");
        public static readonly Level Verbose = new Level(LevelType.Verbose, "VERBOSE");
        public static readonly Level Off = new Level(LevelType.Off, "OFF");
        public static readonly Level Trace = new Level(LevelType.Trace, "TRACE");

        private readonly string v_levelName;
        private readonly int v_levelValue;
        
        
        /// <summary>
        /// ctor: Defines a level.
        /// </summary>
        /// <param name="level">the value of the level</param>
        /// <param name="levelName">the name of the level</param>
        public Level(LevelType level, string levelName)
        {
            if (levelName == null)
            {
                throw new ArgumentNullException("levelName");
            }
            this.v_levelValue = (int)level;
            this.v_levelName = string.Intern(levelName);
        }

        public Level(Level level, string levelName)
        {
            if (levelName == null)
            {
                throw new ArgumentNullException("levelName");
            }
            this.v_levelValue = level.v_levelValue;
            this.v_levelName = string.Intern(levelName);
        }

        /// <summary>
        /// Compare two levels
        /// </summary>
        /// <param name="lhs">the left hand side</param>
        /// <param name="rhs">the right hand side</param>
        /// <returns></returns>
        public static int Compare(Level lhs, Level rhs)
        {
            if ((lhs == rhs) || (lhs == null && rhs == null))
            {
                return 0;
            }
            else if (lhs == null)
            {
                return -1;
            }
            else if (rhs == null)
            {
                return 1;
            }
            else
                return (lhs.v_levelValue - rhs.v_levelValue);
        }

        /// <summary>
        /// Check if the left level is the same as the right.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            Level level = obj as Level;
            if (level != null)
            {
                return (this.v_levelValue == level.v_levelValue);
            }
            return base.Equals(obj);
        }

        /// <summary>
        /// Return the value of this level
        /// </summary>
        /// <returns>level value</returns>
        public override int GetHashCode()
        {
            return this.v_levelValue;
        }

        /*
        public static bool operator ==(Level lhs, Level rhs)
        {
            if (null != lhs && rhs != null)
            {
                return (lhs.v_levelValue == rhs.v_levelValue);
            }
            return (lhs == rhs);
        }
        */

        public static bool operator ==(Level lhs, LevelType rhs)
        {
            return (lhs.v_levelValue == (int)rhs);
        }

        public static bool operator >(Level lhs, Level rhs)
        {
            if (lhs.CompareTo(rhs) > 0)
                return true;
            return false;
        }

        public static bool operator <(Level lhs, Level rhs)
        {
            if (lhs.CompareTo(rhs) < 0)
                return true;
            return false;
        }

        public static bool operator >=(Level lhs, Level rhs)
        {
            if (lhs.CompareTo(rhs) >= 0)
                return true;
            return false;
        }

        public static bool operator <=(Level lhs, Level rhs)
        {
            if (lhs.CompareTo(rhs) <= 0)
                return true;
            return false;
        }

        /*
        public static bool operator !=(Level lhs, Level rhs)
        {
            return !(rhs == lhs);
        }
        */

        public static bool operator !=(Level lhs, LevelType rhs)
        {
            return !(lhs.v_levelValue == (int)rhs);
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            Level level = obj as Level;
            if (level == null)
            {
                throw new ArgumentException("Parameter: r, Value: [" + obj + "] is not an instance of Level");
            }
            return Compare(this, level);
        }

        #endregion


        //Properties.
        public string DisplayName
        { get { return this.v_levelName; } }

        public string Name
        { get { return this.v_levelName; } }

        public int Value 
        { 
            get { return this.v_levelValue; }
        }       
    }

    public enum LevelType : int
    {
        All = int.MinValue,
        Alert = 0x188ff,
        Debug = 0x7530,
        Error = 0x11111,
        Fatal = 0x1abcd,
        Info = 0x9999,
        Warn = 0xeeee,
        Trace = 0x4e4e,
        Off = 0x7ffffff7,
        Verbose = 0x2770,
    }
}
