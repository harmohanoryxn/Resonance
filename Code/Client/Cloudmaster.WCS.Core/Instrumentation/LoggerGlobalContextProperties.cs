namespace WCS.Core.Instrumentation
{
    /// <summary>
    /// Wraps Log4Net Global Context properties so it can be replaced for something else if needed.
    /// </summary>
    public class LoggerGlobalContextProperties : ILogContextProperties
    {
        public LoggerGlobalContextProperties()
        {            
        }

        public object this[string key]
        {
            get { return log4net.GlobalContext.Properties[key]; }
            set { log4net.GlobalContext.Properties[key] = value; }
        }

        public void Remove(string key)
        {
            log4net.GlobalContext.Properties.Remove(key);
        }

        public void Clear()
        {
            log4net.GlobalContext.Properties.Clear();
        }
    }




}
