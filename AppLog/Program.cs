using NConsoler;

namespace AppLog
{
    internal class Program
    {
        /// <summary>
        /// This is the exposed method whict actually does the logging
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="app">The name of the application to use in the event log. This should be the name of the process and is used to differentiate applications. (batch scripts)</param>
        /// <param name="module">The logger name to use; can be used to differenetiate different modules of an applications (batch scripts)</param>
        /// <param name="level">The level (Debug, Info, Warn, Error, or Fatal) of the message</param>
        /// <param name="configFile">A log4net configuration file to use instead of the default event log configuration.</param>
        [Action]
        public static void Log(string message, string app="AppLog Client", string module = null, string level = "Info", string configFile = null)
        {
            (new Logger {ConfigurationFile = configFile, ModuleName = module, ApplicationName = app}).Log(message, level);
        }

        private static void Main()
        {
            Consolery.Run();
        }
    }
}
