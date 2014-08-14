using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Logger.Core;

namespace Logger.Tools
{
    public class LevelCollection : Hashtable
    {
        public LevelCollection() : base(StringComparer.CurrentCultureIgnoreCase)
        {
        }

        public void Add(Level level)
        {
            if (level != null)
            {
                Add(level.Name, level.Value);
            }
        }

        public void Add(string name, int value)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            this.Add(name, new Level((LevelType)Enum.Parse(typeof(LevelType), value.ToString()), name));
        }

        public Level this[string name]
        {
            get
            {
                if (name == null)
                {
                    throw new ArgumentNullException("name");
                }
                lock (this)
                {
                    return (Level)base[name];
                }
            }
        }
 

    }
}
