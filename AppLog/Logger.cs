using System;
using System.IO;
using log4net;
using log4net.Appender;
using log4net.Layout;

namespace AppLog
{
    /// <summary>
    /// Essentially a wrapper for log4net which takes care of the configuration.
    /// </summary>
    public class Logger
    {
        /// <summary>
        /// The private data store for the <see cref="ModuleName"/>ModuleName property.
        /// </summary>
        private string _moduleName;
        /// <summary>
        /// The name to use for the logger; this provides some locality for what the message is about.
        /// </summary>
        public string ModuleName
        {
            get { return _moduleName ?? (_moduleName = typeof(Program).ToString()); }
            set { this._moduleName = value; }  //TODO: should there be any limits?
        }

        private string _applicationName;
        public string ApplicationName
        {
            get { return _applicationName ?? (_applicationName = "AppLog Client");  }
            set { this._applicationName = value; } //TODO: should there be any limits?
        }

        /// <summary>
        /// The private datastore for the <see cref="logger"/>Logger propery.
        /// </summary>
        private ILog _logger;

        /// <summary>
        /// The ILog instance used to actually log the messages
        /// </summary>
        /// <remarks>
        /// Despite typically name conventions to use upper case letter, the class is named logger, so I just used a lower case logger for the property.
        /// </remarks>
        protected ILog logger
        {
            get { return _logger ?? (_logger = LogManager.GetLogger(ModuleName)); }
        }

        /// <summary>
        /// Private data store for the ConfigurationFile property.
        /// </summary>
        private FileInfo _configurationFile;
        
        /// <summary>
        /// An optional log4net xml configuration file; this allows runtime configuration of log4net's behavior.
        /// </summary>
        public string ConfigurationFile
        {
            get { return _configurationFile.ToString(); }
            set
            {
                if (value == null)
                {
                    _configurationFile = null;
                }
                else
                {
                    var fi = new FileInfo(value);
                    if (!fi.Exists)
                        throw new ArgumentException("Configuration file is expected to be a valid, readable file");
                    _configurationFile = fi;
                }
            }
        }

        /// <summary>
        /// Using to simplify the parsing of the level argument.
        /// 
        /// Basically this is used to easily parse the string to a level and then to log at the appropriate level.
        /// 
        /// The descriptions listed below for the levels are only used as guidance; you can use the levels for anything you wish.
        /// </summary>
        protected enum Level
        {
            /// <summary>
            /// For debugging messages; these should help provide context and tracing information.
            /// </summary>
            Debug,
            /// <summary>
            /// For marking significant events.
            /// </summary>
            Info,
            /// <summary>
            /// Warning; for when something isn't quite as expected, but not technically an error.
            /// </summary>
            Warn,
            /// <summary>
            /// For logging most errors.
            /// </summary>
            Error,
            /// <summary>
            /// For logging non-recoverable errors.
            /// </summary>
            Fatal
        };

        /// <summary>
        /// Configures a default configuration. This is used when <see cref="ConfigurationFile"/> is null so log4net will do something.
        /// </summary>
        /// <remarks>This default is to log everything to the eventlog.</remarks>
        protected void DefaultLocalConfiguration()
        {
            var pl = new PatternLayout { ConversionPattern = "%date [%thread] %-5level %logger [%property{NDC}] - %message%newline" };
            pl.ActivateOptions();

            var e = new EventLogAppender {Layout = pl, ApplicationName = ApplicationName};
            e.ActivateOptions();

            log4net.Config.BasicConfigurator.Configure(e);
        }

        /// <summary>
        /// Logs a <paramref name="message"/>message at a specified <paramref name="level"/>level via log4net
        /// 
        /// Since log4net's ILog interface didn't have a way of specifying the level,
        /// this class is used to try and minimize duplicated code.
        /// </summary>
        /// <remarks>If the string level cannot be parsed, INFO is the default level.</remarks>
        /// <param name="message">The text message to log</param>
        /// <param name="level">Indicates what level should be used for the logs</param>
        public void Log(string message, string level)
        {
            if (_configurationFile == null)
            {
                DefaultLocalConfiguration();
            }
            else
            {
                log4net.Config.XmlConfigurator.Configure(_configurationFile);
            }

            var lev = EnumHelper.Parse(level, true, Level.Info);

            switch (lev)
            {
                case Level.Debug:
                    logger.Debug(message);
                    break;
                case Level.Error:
                    logger.Error(message);
                    break;
                case Level.Fatal:
                    logger.Fatal(message);
                    break;
                case Level.Info:
                    logger.Info(message);
                    break;
                case Level.Warn:
                    logger.Warn(message);
                    break;
                default:
                    logger.Info(message);
                    break;
            }
        }
    }
}