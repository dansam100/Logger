using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Logger.Container;
using Logger.Configuration.Watcher;

namespace Logger.Container
{
    public interface IContainerChooser
    {
        //Methods
        ILoggerContainer CreateContainer(String name, Type type);
        List<ILoggerContainer> GetContainers();
        ILoggerContainer GetContainer(string name);
        ILoggerContainer GetContainer(Assembly assembly);
        bool ContainerExists(string name);

        //Properties
        FileWatcher Watcher { get;}
    }
}
