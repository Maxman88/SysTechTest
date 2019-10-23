using System;
using System.Globalization;
using System.Security.Cryptography;

namespace SysTechTest
{
    public static class Crypto
    {
        public static string GetSHA256Hash(string s) {
            if (string.IsNullOrEmpty(s))
            {
                throw new ArgumentException("An empty string value cannot be hashed.");
            }
            Byte[] data = System.Text.Encoding.UTF8.GetBytes(s);
            Byte[] hash;
            using (var sha = SHA256CryptoServiceProvider.Create())
            {
                hash = sha.ComputeHash(data);
            }
            return Convert.ToBase64String(hash);
        }
    }
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
        /// <summary>
        /// Вычисление процента от числа
        /// </summary>
        /// <param name="basis"> число </param>
        /// <param name="percent"> процент </param>
        /// <returns></returns>
        public static decimal CalcPercent(decimal basis, decimal percent) {
            return basis * 0.01M * percent;
        }
    }
    public static class DateTimeUtils
    {
        private const string DateTimeFormat = "dd.MM.yyyy h:mm:ss";
        public static DateTime DateFromStr(string s) => DateTime.ParseExact(s, DateTimeFormat, CultureInfo.InvariantCulture);
        public static string DateToStr(DateTime d) => d.ToString(DateTimeFormat, CultureInfo.InvariantCulture);
    }
}
