

using System;
using System.Globalization;

namespace SysTechTest
{
    public static class Currency
    {
        /// <summary>
        /// Округление для денег
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static decimal Round(decimal val) {
            return Math.Round(val, 2, MidpointRounding.AwayFromZero);
        }
    }
    public static class DateTimeUtils
    {
        private const string DateTimeFormat = "dd.MM.yyyy";
        public static DateTime DateFromStr(string s) => DateTime.ParseExact(s, DateTimeFormat, CultureInfo.InvariantCulture);
        public static string DateToStr(DateTime d) => d.ToString(DateTimeFormat, CultureInfo.InvariantCulture);
    }
}
