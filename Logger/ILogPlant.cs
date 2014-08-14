using System;
using System.Collections.Generic;
using System.Text;

namespace Logger.Container.Ranks
{
    public interface ILogPlant
    {
        Logger GrowLogger(string name);
    }
}
