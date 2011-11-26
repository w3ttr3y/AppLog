using System;

namespace AppLog
{
    public class EnumHelper
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
}
