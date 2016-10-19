using System;
using System.Reflection;
using log4net.Config;
using log4net.Core;

namespace WCS.Core.Instrumentation
{
    /// <summary>
    /// Wraps Log4Net so it can be replaced for something else if needed.
    /// </summary>
    public sealed class Logger : BaseLogger
    {
        #region Fields

        private readonly log4net.Core.ILogger _log4netLogger;
        private readonly ILogContextProperties _threadContextProperties = new LoggerThreadContextProperties();
        private readonly ILogContextProperties _globalContextProperties = new LoggerGlobalContextProperties();
        private static object _logLock = new object();
        #endregion

        #region Constructors

//        static Logger()
//        {
//            // Configure log4net.
//            //
//#if DEBUG
//            log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("LoggingConfig.debug.xml"));
//#else // Assume release for other configurations.
//            log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("LoggingConfig.release.xml"));
//#endif
//        }

        /// <summary>
        /// Creates a new logger.
        /// </summary>
        /// <param name="name">A name which can be included in the output.</param>
        public Logger(string name, bool isExternalConfigFile)
            : base()
		{

			if (isExternalConfigFile) log4net.Config.DOMConfigurator.Configure(); 
            this.Name = name;
            _log4netLogger = log4net.Core.LoggerManager.GetLogger(Assembly.GetCallingAssembly(), name);
            SetLoggingLevel();
        }

        /// <summary>
        /// Creates a new logger.
        /// </summary>
        /// <param name="ownerType">The Type of the class that declared or owns this logger.</param>
        public Logger(Type ownerType)
        {
        	XmlConfigurator.Configure();
            _log4netLogger = log4net.Core.LoggerManager.GetLogger(ownerType.Assembly, ownerType);

            this.Name = _log4netLogger.Name;
            SetLoggingLevel();
        }

        private void SetLoggingLevel()
        {
            this.IsDebugEnabled = _log4netLogger.IsEnabledFor(this.ConvertLogLevel(LoggingLevel.Debug));
            this.IsInfoEnabled = _log4netLogger.IsEnabledFor(this.ConvertLogLevel(LoggingLevel.Info));
            this.IsWarnEnabled = _log4netLogger.IsEnabledFor(this.ConvertLogLevel(LoggingLevel.Warn));
            this.IsErrorEnabled = _log4netLogger.IsEnabledFor(this.ConvertLogLevel(LoggingLevel.Error));
            this.IsFatalEnabled = _log4netLogger.IsEnabledFor(this.ConvertLogLevel(LoggingLevel.Fatal));
        }

        #endregion

        #region Methods

        /// <summary>
        /// When overridden in a derived class, prefixes a string with some logging boilerplate.
        /// </summary>
        /// <param name="levelNoun">The level at which the logging is happening.</param>
        /// <param name="line">The text string being logged.</param>
        /// <returns>A formatted string which will be passed on to the logger.</returns>
        protected override string Prefix(string levelNoun, string line)
        {
            var utcNow = DateTime.UtcNow;
            int msDelta = (int)utcNow.Subtract(this.InitializedAtUtc).TotalMilliseconds;

            return String.Concat(msDelta, " ", line);
        }

        /// <summary>
        /// Writes a line of text out to the underlying logging implementation.
        /// </summary>
        /// <remarks>
        /// This method will not be called if debugging is not enabled at the level it was
        /// logged at via the public logging methods.
        /// </remarks>
        /// <param name="line">The text string to write to the logger.</param>
        /// <param name="level">The level at which the log should be written.</param>
        protected override void WriteLog(LoggingLevel level, string line)
        {
			//if (line.Length>200)
			//    line = line.Substring(0,200);

            lock (_logLock)
            {
                _log4netLogger.Log(typeof(Logger), ConvertLogLevel(level), line, null);
            }
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Provides access to setting global context property name-value pairs to a logger instance.  
        /// This allows use of filtering log messages using these property values
        /// </summary>
        public ILogContextProperties GlobalContextProperties
        {
            get { return _globalContextProperties; }
        }

        /// <summary>
        /// Provides access to setting global context property name-value pairs to a logger instance.  
        /// This allows use of filtering log messages using these property values       
        /// </summary>
        public ILogContextProperties ThreadContextProperties
        {
            get { return _threadContextProperties; }
        }

        private log4net.Core.Level ConvertLogLevel(LoggingLevel loggingLevel)
        {
            switch (loggingLevel)
            {
                case LoggingLevel.Debug: return log4net.Core.Level.Debug;
                case LoggingLevel.Info: return log4net.Core.Level.Info;
                case LoggingLevel.Warn: return log4net.Core.Level.Warn;
                case LoggingLevel.Error: return log4net.Core.Level.Error;
                case LoggingLevel.Fatal: return log4net.Core.Level.Fatal;
            }

            return log4net.Core.Level.Off;
        }

        #endregion
    }
}
