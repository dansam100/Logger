using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Logger.Core
{
    public delegate ILoggerWrapper LogWrapperCreatedHandler(ILogger logger);
    
    public class LogWrapperHash
    {
        //fields
        private Hashtable v_wrapperHash;
        ////Add eventhandlers.
        private readonly LogWrapperCreatedHandler v_wrapperCreatedHandler;

        //Methods
        public LogWrapperHash(LogWrapperCreatedHandler handler)
        {
            this.v_wrapperCreatedHandler = handler;
            v_wrapperHash = new Hashtable();
        }

        protected virtual ILoggerWrapper CreateWrapperObject(ILogger logger)
        {
            if (this.v_wrapperCreatedHandler != null)
            {
                return this.v_wrapperCreatedHandler(logger);
            }
            //return null;
            return new LoggerImpl(logger);
        }

        public virtual ILoggerWrapper GetWrapper(ILogger logger)
        {
            if (logger == null)
            {
                return null;
            }
            lock (this)
            {
                Hashtable hashtable = (Hashtable)this.v_wrapperHash[logger.Container];
                if (hashtable == null)
                {
                    hashtable = new Hashtable();
                    this.v_wrapperHash[logger.Container] = hashtable;
                    //logger.Container.ShutdownEvent += this.m_shutdownHandler;
                }
                ILoggerWrapper wrapper = hashtable[logger] as ILoggerWrapper;
                if (wrapper == null)
                {
                    wrapper = this.CreateWrapperObject(logger);
                    hashtable[logger] = wrapper;
                }
                return wrapper;
            }

        }

        public Hashtable Containers
        {
            get { return v_wrapperHash; }
        }
    }
}
