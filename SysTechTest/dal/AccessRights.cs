using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysTechTest.dal
{
    class AccessRights
    {
        public Int64 Id;
        // common
        public string Name;
        public string DateOfEmployment;
        public int GroupId;
        public Int64 ParentId;
        
        
        // enter params
        public string Login;
        public string Password;
        
        // начисление заработной платы
        public decimal BaseRate;

        // от группы зависят методы расчёта зп.
        // 1) базовая ставка

        // 2) надбавка за стаж
        // 3) надбавка за подчинённых 1-го уровня
        // 4) надбавка за подчинённых всех уровней
    }
}
