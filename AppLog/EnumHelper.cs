using System;

namespace AppLog
{
    /// <summary>
    /// A helper class for utilizing enums.
    /// 
    /// At this point, it only contains <see cref="Parse&lt;Enum&gt;">Parse</see>
    /// </summary>
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
            // Can't constrain type parameters to being an enum,
            // so the struct, IConvertible constraint
            // and then checking if it was an Enum was the best I could figure out
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException("Type TEnum must be an enumerated type.");
            }

            //Try to parse the level, if we can't set it to the default (TEnum def)
            TEnum lev;
            if (!Enum.TryParse(value, ignoreCase, out lev))
            {
                lev = def;
            }
            return lev;
        }
    }
}
