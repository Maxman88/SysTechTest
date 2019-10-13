using System;
using System.Collections.Generic;
using SysTechTest.dal;

namespace SysTechTest.PaySystems
{
    /// <summary>
    /// Подсчёт надбавки к оплате труда за стаж.
    /// </summary>
    public class PayExperience : PayBaseRate
    {
        private readonly decimal m_percentInAYear;
        private readonly decimal m_maxPercent;

        public PayExperience(decimal percentInAYear, decimal maxPercent) {
            m_percentInAYear = percentInAYear;
            m_maxPercent = maxPercent;
        }

        /// <summary>
        /// Функция расчёта надбавок за стаж работы
        /// </summary>
        /// <param name="baseRate"> базовая ставка </param>
        /// <param name="dateFrom"> дата начала расчёта </param>
        /// <param name="dateTo"> дата окончания расчёта </param>
        /// <param name="dateOfEmployment"> дата устройства на работу </param>
        /// <param name="percentInAYear"> процент надбавки за каждый год стажа</param>
        /// <param name="maxPercent"> максимальная надбавка в % </param>
        /// <returns></returns>
        public override decimal Calc(Employee candidat, DateTime dateFrom, DateTime dateTo) {
            if (dateTo < dateFrom)
            {
                throw new ArgumentException("PayExperience.Calc: Period is invalid.");
            }
            if (candidat == null)
            {
                throw new ArgumentNullException("PayExperience.Calc: parametr candidat is null.");
            }
            var dateOfEmployment = DateTimeUtils.DateFromStr(candidat.DateOfEmployment);
            if (dateTo <= dateOfEmployment || candidat.BaseRate == decimal.Zero)
            {
                return 0.0M;
            }

            var intervals = GetIntervals(dateOfEmployment, dateFrom, dateTo);

            decimal res = 0.0M;
            foreach (var (stag, date1, date2) in intervals.FindAll(item => decimal.Zero < item.stag))
            {
                decimal payBase = base.Calc(candidat, date1, date2);
                decimal percent = (decimal)stag * m_percentInAYear;
                percent = percent < m_maxPercent ? percent : m_maxPercent;
                res += payBase * 0.01M * percent;
            }
            return Currency.Round(res);
        }
        private List<(int stag, DateTime dateFrom, DateTime dateTo)> GetIntervals
                    (DateTime dateOfEmployment, DateTime dateFrom, DateTime dateTo) 
        {
            if (dateFrom < dateOfEmployment)
            {
                dateFrom = dateOfEmployment;
            }
            List<(int, DateTime, DateTime)> res = new List<(int, DateTime, DateTime)>();
            if(dateTo <= dateOfEmployment )
            {
                return res;
            }
            int staj = 0;
            while (dateOfEmployment.AddYears(staj) < dateTo)
            {
                DateTime currYear = dateOfEmployment.AddYears(staj);
                DateTime nextYear = dateOfEmployment.AddYears(staj + 1);
                if (currYear <= dateFrom && dateFrom < nextYear)
                {
                    if(dateTo <= nextYear)
                    {
                        res.Add((staj, dateFrom.AddDays(dateFrom != currYear ? 0 : 1), dateTo));
                    }else 
                    {
                        res.Add((staj, dateFrom.AddDays(dateFrom != currYear ? 0 : 1), nextYear));
                        dateFrom = nextYear;
                    }
                }
                staj++;
            }
            return res;
        }
    }
}
