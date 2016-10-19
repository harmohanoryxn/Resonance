namespace WCS.Core.Instrumentation
{
    /// <summary>
    /// Wraps Log4Net Thread Context properties so it can be replaced for something else if needed.
    /// </summary>
    public class LoggerThreadContextProperties : ILogContextProperties
    {
        public LoggerThreadContextProperties()
        {
        }

        public object this[string key]
        {
            get { return log4net.ThreadContext.Properties[key]; }
            set { log4net.ThreadContext.Properties[key] = value; }
        }

        public void Remove(string key)
        {
            log4net.ThreadContext.Properties.Remove(key);
        }

        public void Clear()
        {
            log4net.ThreadContext.Properties.Clear();
        }
    }
}
