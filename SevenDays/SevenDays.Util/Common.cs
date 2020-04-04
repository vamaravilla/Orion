using System;
using System.Collections.Generic;
using System.Text;

namespace SevenDays.Util
{
    public static class Common
    {
        /// <summary>
        /// Build the error detail
        /// </summary>
        /// <param name="ex">Exfeption</param>
        /// <returns>Error detail string</returns>
        public static string GetMessageError(Exception ex)
        {
            if (ex == null) return String.Empty;
            if (ex.InnerException != null)
            {
                return $"Data Access error: {ex.Message}, Inner: {ex.InnerException.Message}";
            }
            else
            {
                return $"Data Access error: {ex.Message}";
            }
        }
    }
}
