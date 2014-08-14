using System;
using System.Collections.Generic;
using System.Text;
using Logger.Notifier;

namespace Logger.Notifier
{
    public class NotifierList : List<INotifier>
    {
        private const int DEFAULT_SIZE = 0x10;

        static NotifierList()
        {
        }

        public NotifierList()
        {

        }

        public NotifierList(int size) : this()
        {
            base.Capacity = size;
        }

        public new void Add(INotifier notifier)
        {
            if (this.Count == Capacity)
            {
                Capacity++;
            }
            base.Add(notifier);
        }
    }
}
