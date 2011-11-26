using NConsoler;

namespace AppLog
{
    internal class Program
    {
        /// <summary>
        /// This is the exposed method which actually does the logging
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="loggername">The logger name to use; can be used to differenetiate applications (batch scripts)</param>
        /// <param name="level">The level (Debug, Info, Warn, Error, or Fatal) of the message</param>
        /// <param name="configFile">A log4net configuration file to use instead of the default event log configuration.</param>
        [Action]
        public static void Log(string message, string loggername = null, string level = "Info", string configFile = null)
        {
            (new Logger {ConfigurationFile = configFile, Name = loggername}).Log(message, level);
        }

        private static void Main()
        {
            Consolery.Run();
        }
    }
}
