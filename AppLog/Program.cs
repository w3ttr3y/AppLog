using System;
using System.IO;
using log4net;
using log4net.Appender;
using log4net.Layout;
using NConsoler;

namespace AppLog
{
    internal class EnumHelper
    {
        /// <summary>
        /// The TryParse method doesn't have a way of specifying a default, so I wrote this helper
        /// </summary>
        /// <typeparam name="TEnum">Type of enumeration</typeparam>
        /// <param name="value">String to try and parse</param>
        /// <param name="ignoreCase">Case sensitive when matching</param>
        /// <param name="def">Default level to use if parsing fails</param>
        /// <returns></returns>
        internal static TEnum Parse<TEnum>(string value, bool ignoreCase, TEnum def) where TEnum : struct, IConvertible
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException("Type TEnum must be an enumerated type.");
            }

            TEnum lev;
            if (!Enum.TryParse(value, ignoreCase, out lev))
            {
                lev = def;
            }
            return lev;
        }
    }

    internal class Logger
    {
        /// <summary>
        /// Using to simplify the parsing of the level argument
        /// </summary>
        internal enum Level
        {
            Debug,
            Info,
            Warn,
            Error,
            Fatal
        };

        /// <summary>
        /// Factory method used to get an ILog instance
        /// </summary>
        /// <param name="loggername">The logger name to use; typically the class name</param>
        /// <param name="configurationFile">The log4net configuration file to use instead of the default event log configuration.</param>
        /// <returns>An instane of logger to use when logging</returns>
        public static ILog GetLogger(string loggername = null, string configurationFile = null)
        {
            if (loggername == null)
                loggername = typeof(Program).ToString();

            if (configurationFile != null)
            {
                var fi = new FileInfo(configurationFile);
                if (fi.Exists)
                {
                    log4net.Config.XmlConfigurator.Configure(fi);
                }
                else
                {
                    DefaultLocalConfiguration();
                }
            }
            else
            {
                DefaultLocalConfiguration();
            }

            ILog logger = LogManager.GetLogger(loggername);
            return logger;
        }

        private static void DefaultLocalConfiguration()
        {
            var pl = new PatternLayout
                         {ConversionPattern = "%date [%thread] %-5level %logger [%property{NDC}] - %message%newline"};
            pl.ActivateOptions();

            var e = new EventLogAppender {Layout = pl};
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
        /// <param name="logger">An ILog instance to log on</param>
        /// <param name="lev">Indicates what level should be used for the logs</param>
        public static void Log(string message, ILog logger, Level lev)
        {
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

    internal class Program
    {
        /// <summary>
        /// This is the exposed method which actually does the logging
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="loggername">The logger name to use; can be used to differenetiate applications (batch scripts)</param>
        /// <param name="level">The level (Debug, Info, Warn, Error, or Fatal) of the message</param>
        /// <param name="configurationFile">A log4net configuration file to use instead of the default event log configuration.</param>
        [Action]
        public static void Log(string message, string loggername = null, string level = "Info", string configurationFile = null)
        {
            var logger = Logger.GetLogger(loggername);
            var lev = EnumHelper.Parse(level, true, Logger.Level.Info);
            Logger.Log(message, logger, lev);
        }

        private static void Main()
        {
            Consolery.Run();
        }
    }
}
