using System;
using System.IO;
using log4net;
using log4net.Appender;
using log4net.Layout;

namespace AppLog
{
    public class Logger
    {
        private string _name;
        public string Name
        {
            get { return _name ?? (_name = typeof(Program).ToString()); }
            set { this._name = value; }  //TODO: should there be any limits?
        }

        private ILog _logger;
        protected ILog logger
        {
            get { return _logger ?? (_logger = LogManager.GetLogger(Name)); }
        }

        private FileInfo _configurationFile;
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
        /// Using to simplify the parsing of the level argument
        /// </summary>
        protected enum Level
        {
            Debug,
            Info,
            Warn,
            Error,
            Fatal
        };

        private static void DefaultLocalConfiguration()
        {
            var pl = new PatternLayout { ConversionPattern = "%date [%thread] %-5level %logger [%property{NDC}] - %message%newline" };
            pl.ActivateOptions();

            var e = new EventLogAppender { Layout = pl };
            e.ActivateOptions();

            log4net.Config.BasicConfigurator.Configure(e);
        }

        /// <summary>
        /// Logs a message via log4net
        /// 
        /// Since log4net's ILog interface didn't have a way of specifying the level,
        /// this class is used to try and minimize duplicated code.
        /// </summary>
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
