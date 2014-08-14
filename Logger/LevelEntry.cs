using System;
using System.Collections.Generic;
using System.Text;

namespace Logger.Container.Ranks
{
    // Nested Types
    /// <summary>
    /// Level Entry class
    /// </summary>
    internal class LevelEntry
    {
        // Fields
        private string v_DisplayName;
        private string v_levelName;
        private int v_levelValue;

        // Methods
        public LevelEntry()
        {
            this.v_DisplayName = null;
            this.v_levelName = null;
            this.v_levelValue = -1;
        }

        public override string ToString()
        {
            return string.Concat(
                new object[] { "LevelEntry(Value=", this.v_levelValue,
                        ", Name=", this.v_levelName, ", DisplayName=", this.v_DisplayName, ")" });
        }

        // Properties
        public string DisplayName
        {
            get
            {
                return this.v_DisplayName;
            }
            set
            {
                this.v_DisplayName = value;
            }
        }

        public string Name
        {
            get
            {
                return this.v_levelName;
            }
            set
            {
                this.v_levelName = value;
            }
        }

        public int Value
        {
            get
            {
                return this.v_levelValue;
            }
            set
            {
                this.v_levelValue = value;
            }
        }
    }
}
