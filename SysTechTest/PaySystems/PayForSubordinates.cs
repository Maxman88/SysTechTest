using System;
using SysTechTest.dal;

namespace SysTechTest.PaySystems
{
    public class PayForSubordinates : PayBase
    {
        private readonly decimal m_percent;
        private readonly bool m_onlyFirstLevel;
        public PayForSubordinates(decimal percent, bool onlyFirstLevel) {
            m_percent = percent;
            m_onlyFirstLevel = onlyFirstLevel;
        }
        public override decimal Calc(Employee candidat, DateTime dateFrom, DateTime dateTo) {
            if (dateTo < dateFrom)
            {
                throw new ArgumentException("PayForSubordinates.Calc: Period is invalid.");
            }
            if (candidat == null)
            {
                throw new ArgumentNullException("PayForSubordinates.Calc: parametr candidat is null.");
            }
            if (m_percent == decimal.Zero)
            {
                return 0.0M;
            }
            decimal res = m_onlyFirstLevel ? CalcFirstLevel(candidat, dateFrom, dateTo)
                                           : CalcAllLevels(candidat, dateFrom, dateTo);
            return Currency.Round(res * 0.01M * m_percent);
        }
        private decimal CalcFirstLevel(Employee candidat, DateTime dateFrom, DateTime dateTo) {
            decimal res = 0.0M;
            foreach (var item in candidat.GetSubordinates()) {
                res += CtrlWorkPayment.Instance.CalcWorkPayment(item, dateFrom, dateTo);
            }
            return res;
        }
        private decimal CalcAllLevels(Employee candidat, DateTime dateFrom, DateTime dateTo, decimal acc=0.0M) {
            foreach (var item in candidat.GetSubordinates()) {
                _= CalcAllLevels(item, dateFrom, dateTo, acc);
                acc += CtrlWorkPayment.Instance.CalcWorkPayment(item, dateFrom, dateTo);
            }
            return acc;
        }
    }
}