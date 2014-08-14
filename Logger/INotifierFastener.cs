using System;
using System.Collections.Generic;
using System.Text;
using Logger.Notifier;

namespace Logger.Tools
{
    public interface INotifierFastener
    {
        void AddNotifier(INotifier notifier);
        void RemoveNotifier(INotifier notifier);
        INotifier RemoveNotifier(string name);
        INotifier GetNotifier(string name);
        void RemoveAllNotifiers();
        bool Contains(INotifier notifier);
        //Properties
        NotifierList Notifiers { get;}
    }
}
