using System;
using System.Collections.Generic;
using SysTechTest.dal;

namespace SysTechTest.PaySystems
{
    public class CtrlWorkPayment
    {
        #region Singleton
        private static CtrlWorkPayment m_instance;
        public static CtrlWorkPayment Instance {
            get {
                if (m_instance == null) {
                    m_instance = new CtrlWorkPayment();
                }
                return m_instance;
            }
        }
        #endregion
        //------------
        private CtrlWorkPayment() { }
        public decimal CalcWorkPayment(Employee candidat, DateTime from, DateTime to) {
            if(candidat == null) {
                throw new ArgumentNullException("CtrlWorkPayment.CalcWorkPayment: parametr candidat is null.");
            }
            if(from > to) {
                throw new ArgumentException("CtrlWorkPayment.CalcWorkPayment: Period is invalid.");
            }
            List<PayBase> lst = BuildPaySystems(candidat.GroupId);
            decimal res = 0.0M;
            foreach( var item in lst ) {
                res += item.Calc(candidat, from, to);
            }
            return res;
        }

        public decimal CalcWorkPaymentAll(DateTime from, DateTime to) {
            decimal res = 0.0M;
            foreach( var empl in CtrlDbCtx.Instance.GetListEmployees())
            {
                res += CalcWorkPayment(empl, from, to);
            }
            return res;
        }
        private List<PayBase> BuildPaySystems(int groupId) {
            List<PayBase> res = new List<PayBase>();
            var ps = CtrlDbCtx.Instance.GroupsOfEmployee.Find(p => p.Id == groupId).GetPaySystems();
            if(ps.PayBaseRateParams.IsEnabled)
            {
                res.Add(new PayBaseRate(ps.PayBaseRateParams.BaseRate));
            }
            if (ps.PayExperienceParams.IsEnabled)
            {
                res.Add(new PayExperience(ps.PayBaseRateParams.BaseRate, ps.PayExperienceParams.PercentInAYear, ps.PayExperienceParams.MaxPercent));
            }
            if (ps.PayForSubordinatesParams.IsEnabled)
            {
                res.Add(new PayForSubordinates(
                    ps.PayForSubordinatesParams.Percent) 
                    { OnlyFirstLevelEnable = ps.PayForSubordinatesParams.OnlyFirstLevelEnabled }
                );
            }
            return res;
        }
    }
}
