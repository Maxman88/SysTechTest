using System;
using SysTechTest.dal;

namespace SysTechTest.PaySystems
{
    /// <summary>
    ///  1) базовая ставка заработной платы (согласно т.з. информация известная)
    ///  2) число рабочих дней в месяце(нужно что-то чтобы получать эту инфу за период времени, похоже на моё усмотрение)
    ///  2.1) справочник какие дни рабочие, какие нет(вероятно тоже на моё усмотрение)
    ///  2.2) сколько фактически было рабочих дней в периоде.
    ///  Пример. Системному технику установлен должностной оклад - 10000 руб. 
    ///     В ноябре 2007 года он отработал 17 рабочих дней(число рабочих дней в ноябре - 21). 
    ///     Следовательно, его заработок за ноябрь составит: 10000 / 21 * 17 = 8095,24 руб.
    ///  Источник: http://tkodeksrf.ru/ch-3/rzd-6/gl-20/st-129-tk-rf
    /// </summary>
    public class PayBaseRate : PayBase {
        /// <summary>
        /// Подсчёт суммы оплаты за фактически отработанное время
        /// </summary>
        /// <param name="baseRate"> базовая ставка </param>
        /// <param name="dateFrom"> дата начала расчёта </param>
        /// <param name="dateTo"> дата окончания расчёта </param>
        /// <returns></returns>
        public override decimal Calc(Employee candidat, DateTime dateFrom, DateTime dateTo) {
            if (dateTo < dateFrom ) {
                throw new ArgumentException("PayBaseRate.Calc: Period is invalid.");
            }
            if (candidat == null)
            {
                throw new ArgumentNullException("PayBaseRate.Calc: parametr candidat is null.");
            }
            if (candidat.BaseRate == decimal.Zero)
            {
                return 0.0M;
            }
            if(dateFrom == dateTo && false == IsItWorkingDay(dateFrom))
            {
                return 0.0M;
            }
            decimal res = 0.0M;
            while (dateFrom <= dateTo)
            {
                var month = GetMonthPeriod(dateFrom);
                var toForCalcPay = month.lastDay < dateTo ? month.lastDay : dateTo;
                res += CalcPayAtMonth(candidat.BaseRate, month, dateFrom, toForCalcPay);
                month = GetMonthPeriod(month.firstDay.AddMonths(1));
                dateFrom = month.firstDay;
            }
            return res;
        }
        /// <summary>
        /// Подсчёт количества рабочих дней (условие: рабочие дни все кроме субботы и воскресения)
        /// </summary>
        /// <param name="from"> дата начала </param>
        /// <param name="to"> дата окончания </param>
        /// <returns> количество рабочих дней </returns>
        public int CalcAmountBusinessDays(DateTime from, DateTime to) {
            if (from > to)
            {
                throw new ArgumentException("PayBaseRate.CalcAmountBusinessDays: Period is invalid.");
            }
            int totalDays = 0;
            for (var date = from; date <= to; date = date.AddDays(1))
            {
                if (IsItWorkingDay(date))
                {
                    totalDays++;
                }
            }
            return totalDays;
        }
        private decimal CalcPayAtMonth(decimal baseRate, (DateTime firstDay, DateTime lastDay) month, DateTime from, DateTime to) {
            decimal allworkdays = CalcAmountBusinessDays(month.firstDay, month.lastDay);
            decimal daysWorked = CalcAmountBusinessDays
                (
                    month.firstDay < from ? from : month.firstDay,
                    month.lastDay < to ? month.lastDay : to
                );
            return Currency.Round(baseRate / allworkdays * daysWorked);
        }
        private (DateTime firstDay, DateTime lastDay) GetMonthPeriod(DateTime date) {
            DateTime firstDay = new DateTime(date.Year, date.Month, 1);
            DateTime lastDay = firstDay.AddMonths(1).AddDays(-1);
            return (firstDay, lastDay);
        }
        private bool IsItWorkingDay(DateTime d) => d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday;
    }
}
