
using System;
using System.ComponentModel;
using System.Reflection;

namespace SysTechTest.dal
{
    public static class DbHelpers
    {
        /// <summary>
        /// Приведение значения перечисления в удобочитаемый формат.
        /// </summary>
        /// <remarks>
        /// Для корректной работы необходимо использовать атрибут [Description("Name")] для каждого элемента перечисления.
        /// </remarks>
        /// <param name="enumElement">Элемент перечисления</param>
        /// <returns>Название элемента</returns>
        public static string GetDescription(Enum enumElement) {
            Type type = enumElement.GetType();

            MemberInfo[] memInfo = type.GetMember(enumElement.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }

            return enumElement.ToString();
        }
        public enum Group // по хорошему нужен srcgen из бд.
        {
            Supervisor = 1,
            Employee = 2,
            Manager  = 3,
            Salesman = 4
        }
        public enum PaySystem // по хорошему нужен srcgen из бд.
        {
            PayBaseRate = 1,
            PayExperience = 2,
            PayForSubordinates = 3
        }
        public enum PaySystemParamsName
        {
            BaseRate = 1,
            PercentInAYear = 2,
            MaxPercent = 3,
            Percent = 4,
            OnlyFirstLevelEnable = 5
        }
        public enum AccessEntity // по хорошему нужен srcgen из бд.
        {
            /// <summary>
            /// Имя сотрудника
            /// </summary>
            [Description("Employee.Name")]
            EmployeeName = 1,
            /// <summary>
            /// Группа сотрудника
            /// </summary>
            [Description("Employee.Group")]
            EmployeeGroup = 2,
            /// <summary>
            /// Начальник сотрудника
            /// </summary>
            [Description("Employee.Chief")]
            EmployeeChief = 3,
            /// <summary>
            /// Дата трудоустройства сотрудника
            /// </summary>
            [Description("Employee.DateOfEmployment")]
            EmployeeDateOfEmployment = 4,
            /// <summary>
            /// Логин
            /// </summary>
            [Description("Employee.Login")]
            EmployeeLogin = 5,
            /// <summary>
            /// пароль
            /// </summary>
            [Description("Employee.Password")]
            EmployeePassword = 6


        }
    }
}
