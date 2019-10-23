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
            Type type = enumElement?.GetType();

            MemberInfo[] memInfo = type.GetMember(enumElement.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }

            return enumElement.ToString();
        }
        /// <summary>
        /// Группы пользователей
        /// </summary>
        public enum Group // по хорошему нужен srcgen из бд.
        {
            /// <summary>
            /// Суперпользователь
            /// </summary>
            [Description("Supervisor")]
            Supervisor = 1,
            /// <summary>
            /// Сотрудник
            /// </summary>
            [Description("Employee")]
            Employee = 2,
            /// <summary>
            /// Менеджер
            /// </summary>
            [Description("Manager")]
            Manager = 3,
            /// <summary>
            /// Продавец
            /// </summary>
            [Description("Salesman")]
            Salesman = 4
        }
        /// <summary>
        /// Виды алгоритмов по начислению оплаты труда
        /// </summary>
        public enum PaySystem // по хорошему нужен srcgen из бд.
        {
            /// <summary>
            /// Оплата по базовой ставке(окладу)
            /// </summary>
            [Description("PayBaseRate")]
            PayBaseRate = 1,
            /// <summary>
            /// Оплата за стаж
            /// </summary>
            [Description("PayExperience")]
            PayExperience = 2,
            /// <summary>
            /// Оплата процента от оплаты труда подчинённых
            /// </summary>
            [Description("PayForSubordinates")]
            PayForSubordinates = 3
        }
    }
}
