using System;
using SysTechTest.dal;

namespace SysTechTest.PaySystems
{
    abstract public class PayBase
    {
        public abstract decimal Calc(Employee candidat, DateTime dateFrom, DateTime dateTo);
    }
}
