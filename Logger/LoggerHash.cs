using System;
using System.Collections.Generic;
using System.Text;

namespace Logger.Container.Ranks
{
    internal class LoggerHash
    {
        private string v_name;
        private int v_hashcode;

        public LoggerHash(string name)
        {
            this.v_name = name;
            this.v_hashcode = name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }
            LoggerHash key = obj as LoggerHash;
            return ((key != null) && (this.v_name == key.v_name));
        }

        public override int GetHashCode()
        {
            return this.v_hashcode;
        }

    }
}
