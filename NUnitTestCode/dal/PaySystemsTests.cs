using System;
using NUnit.Framework;
using SysTechTest.PaySystems;
using SysTechTest.dal;
using System.Collections.Generic;
using SysTechTest;

namespace NUnitTestCode.dal
{
    class PaySystemsTests
    {
        private DateTime DatеFromStr(string d) => DateTimeUtils.DateFromStr(d+" 0:00:00").Date;
        /// <summary>
        /// Проверка функции расчёта количества рабочих дней за произвольный период.
        /// </summary>
        [Test]
        public void BaseRateTestGetBusinessDays() {
            var br = new PayBaseRate(100M);
            int func(string dateFrom, string dateTo) => 
                br.CalcAmountBusinessDays(DatеFromStr(dateFrom), DatеFromStr(dateTo));
            int t;
            t = func("30.09.2019", "06.10.2019");
            Assert.IsTrue(t == 5);
            t = func("05.10.2019", "27.10.2019");
            Assert.IsTrue(t == 15);
            t = func("02.10.2019", "17.10.2019");
            Assert.IsTrue(t == 12);
            t = func("01.01.2019", "01.02.2019");
            Assert.IsTrue(t == 24);
            t = func("01.01.2019", "09.02.2019");
            Assert.IsTrue(t == 29);
            t = func("01.02.2016", "29.02.2016"); // 2016 высокосный год
            Assert.IsTrue(t == 21);
            t = func("01.02.2016", "09.03.2016"); // 2016 высокосный год
            Assert.IsTrue(t == 28);
            t = func("04.10.2019", "04.10.2019"); // 1 день, пятница
            Assert.IsTrue(t == 1);
            t = func("05.10.2019", "05.10.2019"); // 1 день, суббота
            Assert.Zero(t);
            // проверка реакции на некорректные входные данные
            try
            {
                t = func("01.01.2019", "01.01.2018");
            }
            catch (ArgumentException)
            {
                Assert.IsTrue(true);
            }
            catch
            {
                Assert.IsTrue(false);
            }

        }
        /// <summary>
        /// Проверка функции расчёта з.п. по базовой ставке. 
        /// </summary>
        [Test]
        public void BaseRateTest2() {
            PayBaseRate br = new PayBaseRate(100M);
            Employee empl = new Employee();
            decimal fn(Employee candidat, string dateFrom, string dateTo) =>
                        br.Calc(candidat, DatеFromStr(dateFrom), DatеFromStr(dateTo));
            decimal t;
            br.BaseRate = 23000M;
            // проверка расчёта з.п. в пределах одного дня
            t = fn(empl, "01.10.2019", "01.10.2019"); // вторник рабочий день
            Assert.IsTrue(t == 1000M);
            t = fn(empl, "05.10.2019", "05.10.2019"); // суббота выходной день
            Assert.IsTrue(t == 0M);
            t = fn(empl, "06.10.2019", "06.10.2019"); // воскресенье выходной день
            Assert.IsTrue(t == 0M);
            // проверка расчёта з.п. в пределах одного месяца
            t = fn(empl, "01.10.2019", "31.10.2019");
            Assert.IsTrue(t == 23000M);
            t = fn(empl, "07.10.2019", "31.10.2019");
            Assert.IsTrue(t.Equals(19000M));
            t = fn(empl, "01.10.2019", "06.10.2019");
            Assert.IsTrue(t == 4000M);
            // проверка расчёта з.п. в произвольный период
            t = fn(empl, "01.10.2019", "30.11.2019");
            Assert.IsTrue(t.Equals(46000M));
            t = fn(empl, "07.10.2019", "30.11.2019");
            Assert.IsTrue(t.Equals(42000M));
            t = fn(empl, "21.10.2019", "10.11.2019");
            Assert.IsTrue(t.Equals(15571.43M));
            t = fn(empl, "01.01.2019", "31.12.2019");
            Assert.IsTrue(t.Equals(276000M));
            t = fn(empl, "11.10.2019", "10.12.2019");
            Assert.IsTrue(t.Equals(45318.18M));

            t = fn(empl, "10.08.2019", "31.08.2019");
            Assert.IsTrue(t.Equals(15681.82M));
            t = fn(empl, "01.10.2019", "31.12.2019");
            Assert.IsTrue(t.Equals(69000M));
            t = fn(empl, "01.10.2019", "30.09.2020");
            Assert.IsTrue(t.Equals(276000M));
        }

        [Test]
        public void BaseRateTest3() {
            PayExperience al = new PayExperience(23000M, 3M, 30M);
            Employee empl = new Employee();

            decimal fn(string dateFrom, string dateTo)
                => al.Calc(empl, DatеFromStr(dateFrom), DatеFromStr(dateTo));

            decimal t;
            // проверка поведения в случае расчёта надбавки 
            // до того как был трудоустроен, или до наступления 1-го года стажа
            {
                al.BaseRate = 23000M;
                empl.DateOfEmployment = "01.10.2018 0:00:00";

                t = fn("01.01.2017", "01.10.2018");
                Assert.IsTrue(t.Equals(0.0M));
                t = fn("01.10.2018", "01.10.2019");
                Assert.IsTrue(t.Equals(0.0M));
            }
            // проверяем левую границу в пределах одного года стажа
            {
                al.BaseRate = 23000M;
                empl.DateOfEmployment = "10.08.2018 0:00:00";

                t = fn("10.08.2019", "31.08.2019");
                Assert.IsTrue(t.Equals(470.45M));
                t = fn("01.08.2019", "31.08.2019");
                Assert.IsTrue(t.Equals(470.45M));
                t = fn("01.01.2019", "31.08.2019");
                Assert.IsTrue(t.Equals(470.45M));
                t = fn("01.01.2018", "31.08.2019");
                Assert.IsTrue(t.Equals(470.45M));

                empl.DateOfEmployment = "30.09.2018 0:00:00";
                t = fn("01.11.2019", "30.11.2019");
                Assert.IsTrue(t.Equals(690M));
                t = fn("01.12.2019", "31.12.2019");
                Assert.IsTrue(t.Equals(690M));
            }
            // проверяем правую границу в пределах одного года стажа
            {
                al.BaseRate = 23000M;
                empl.DateOfEmployment = "01.10.2018 0:00:00";

                t = fn("01.10.2019", "31.10.2019");
                Assert.IsTrue(t.Equals(660M));

                empl.DateOfEmployment = "30.09.2018 0:00:00";
                t = fn("01.10.2019", "31.10.2019");
                Assert.IsTrue(t.Equals(690M));
                t = fn("01.10.2019", "31.12.2019");
                Assert.IsTrue(t.Equals(2070M));
                t = fn("01.10.2019", "30.09.2020");
                Assert.IsTrue(t.Equals(8280M));
            }
            // проверяем расчёт в случае перехода стажа с 1-го да до 2 лет
            {
                al.BaseRate = 23000M;
                empl.DateOfEmployment = "30.09.2018 0:00:00";

                t = fn("01.10.2019", "30.11.2019");
                Assert.IsTrue(t.Equals(1380M)); //23000/100*3 + 23000/100*3
                t = fn("01.10.2020", "30.11.2020");
                Assert.IsTrue(t.Equals(2760M)); //23000/100*6 + 23000/100*6
                t = fn("01.09.2020", "31.10.2020");
                Assert.IsTrue(t.Equals(2070M)); //23000/100*3 + 23000/100*6
            }
            // переход м/у стажами + произвольный период
            {
                al.BaseRate = 21000M;
                empl.DateOfEmployment = "15.10.2018 0:00:00";
                // c 01.10.2022 до 15.10.2022 получает за стаж 3 года (21000/21*10) /100*(3*3) = 900
                t = fn("01.10.2022", "15.10.2022");
                Assert.IsTrue(t.Equals(900M));
                // с 16.10.2022 до 31.10.2022 получает за стаж 4 года (21000/21*11) /100*(3*4) = 1320
                t = fn("16.10.2022", "31.10.2022");
                Assert.IsTrue(t.Equals(1320M));
                t = fn("01.10.2022", "31.10.2022");
                Assert.IsTrue(t.Equals(2220M));

                al.BaseRate = 23000M;
                empl.DateOfEmployment = "30.09.2018 0:00:00";
                t = fn("01.09.2020", "30.09.2020");
                Assert.IsTrue(t.Equals(690M));
                t = fn("01.10.2020", "31.10.2020");
                Assert.IsTrue(t.Equals(1380M));
                t = fn("01.09.2020", "31.10.2020");
                Assert.IsTrue(t.Equals(2070M)); //23000/100*3 + 23000/100*6

                al.BaseRate = 21000M;
                empl.DateOfEmployment = "15.10.2018 0:00:00";
                //01.10.2022-15.10.2022 , стаж 3 года   10000/100*(3*3) = 900
                //16.10.2022-15.10.2023 , стаж 4 года   = 31085.45
                //16.10.2023-31.10.2023 , стаж 5 лет 
                t = fn("01.10.2022", "31.10.2023");
                Assert.IsTrue(t.Equals(32803.64M));
                t = fn("01.10.2022", "31.10.2024");
                Assert.IsTrue(t.Equals(70932.33M));
            }
        }
    }
}