using System;
using System.Collections.Generic;
using System.Text;
using Logger.Notifier;
using System.Collections;
using Logger.Core;

namespace Logger.Tools
{
    public class NotifierFastener : INotifierFastener
    {
        private INotifier[] v_notifiers;
        private NotifierList v_notifierList;

        //Methods
        public NotifierFastener()
        {
            this.v_notifierList = new NotifierList(1);
            this.v_notifiers = null;
        }

        public void AddNotifier(INotifier notifier)
        {
            if (notifier == null)
            {
                throw new ArgumentNullException("notifier");
            }
            if (!this.v_notifierList.Contains(notifier))
            {
                this.v_notifierList.Add(notifier);
            }
        }

        public void RemoveNotifier(INotifier notifier)
        {
            if ((notifier != null) && (this.v_notifierList != null))
            {
                this.v_notifierList.Remove(notifier);
                if (this.v_notifierList.Count == 0)
                {
                    this.v_notifierList = new NotifierList(1);
                }
                this.v_notifiers = null;
            }
        }


        public INotifier RemoveNotifier(string name)
        {
            INotifier notifier = this.GetNotifier(name);
            if (notifier != null && this.v_notifierList != null && name != null)
            {
                this.v_notifierList.Remove(notifier);
                if (this.v_notifierList.Count == 0)
                {
                    this.v_notifierList = new NotifierList(1);
                }
                this.v_notifiers = null;
            }
            return notifier;
        }

        public INotifier GetNotifier(string name)
        {
            if (this.v_notifierList != null && name != null)
            {
                foreach (INotifier notifier in this.v_notifierList)
                {
                    if (notifier.Name == name)
                        return notifier;
                }
            }
            return null;
        }

        public void RemoveAllNotifiers()
        {
            this.CloseNestedNofitiers();
            this.Clear();
        }

        public void HyperNotify(Log logevent)
        {
            if (logevent != null)
            {
                if (this.Notifiers != null)
                {
                    this.v_notifiers = Notifiers.ToArray();
                }
                foreach (INotifier notifier in this.v_notifiers)
                {
                    try
                    {
                        notifier.Append(logevent);
                    }
                    catch (Exception e)
                    {
                        //LogLog
                    }
                }
            }
        }


        public void CloseNestedNofitiers()
        {
            foreach (INotifier n in v_notifierList)
            {
                n.Exit();
            }
        }


        private void Clear()
        {
            this.v_notifierList = new NotifierList(1);
            this.v_notifiers = null;
        }


        public NotifierList Notifiers
        {
            get
            {
                if (this.v_notifierList == null)
                {
                    return new NotifierList();
                }
                return (this.v_notifierList);
            }
        }


        public bool Contains(INotifier notifier)
        {
            foreach (INotifier n in this.v_notifierList)
                if (notifier.OtherHashCode() == n.OtherHashCode())
                    return true;
            return false;
        }
    }
}
