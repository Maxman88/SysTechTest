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
        private List<PayBase> BuildPaySystems(int groupId) {
            List<PayBase> res = new List<PayBase>();
            switch ((DbHelpers.Group)groupId)
            {
                case DbHelpers.Group.Employee :
                    res.Add(new PayBaseRate());
                    res.Add(new PayExperience(3M, 30M));
                    break;
                case DbHelpers.Group.Manager:
                    res.Add(new PayBaseRate());
                    res.Add(new PayExperience(5M, 40M));
                    res.Add(new PayForSubordinates(0.5M, true));
                    break;
                case DbHelpers.Group.Salesman:
                    res.Add(new PayBaseRate());
                    res.Add(new PayExperience(1M, 35M));
                    res.Add(new PayForSubordinates(3M, false));
                    break;
                default:
                    string msg = "BuildPaySystems for group id = " + groupId.ToString() + " is not implemented.";
                    throw new NotImplementedException(msg);
            }
            return res;
        }
    }
}
