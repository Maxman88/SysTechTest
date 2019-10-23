using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace SysTechTest.dal

{
    public class DbCtx : DbContext
    {
        #region Сотрудники
        /// <summary>
        /// Список групп сотрудников
        /// </summary>
        public DbSet<Group> Groups { get; set; }
        /// <summary>
        /// Список сотрудников
        /// </summary>
        public DbSet<Employee> Employees { get; set; }
        #endregion
        #region Оплата труда
        /// <summary>
        /// Список способов оплаты труда
        /// </summary>
        public DbSet<PaySystemDesc> PaySystems { get; set; }
        /// <summary>
        /// Параметры для оплаты труда по базовой ставке в разрезе групп
        /// </summary>
        public DbSet<PayBaseRateParam> PayBaseRateParams { get; set; }
        /// <summary>
        /// Параметры для оплаты труда за стаж в разрезе групп
        /// </summary>
        public DbSet<PayExperienceParam> PayExperienceParams { get; set; }
        /// <summary>
        /// Параметры для оплаты труда в разрезе групп процент от оплаты подчинённых
        /// </summary>
        public DbSet<PayForSubordinatesParam> PayForSubordinatesParams { get; set; }
        #endregion
        public bool HasChanges => ChangeTracker.HasChanges();
        public DbCtx() : base("name=DbConnection") {
            Configuration.AutoDetectChangesEnabled = true;
            Configuration.EnsureTransactionsForFunctionsAndCommands = true;
        }
        public async Task InitializeAsync() {
            if(ValidateDatabase())
            {
                var tasks = new List<Task>() 
                    {
                        Groups.LoadAsync(),
                        Employees.LoadAsync(),

                        PaySystems.LoadAsync(),
                        PayBaseRateParams.LoadAsync(),
                        PayExperienceParams.LoadAsync(),
                        PayForSubordinatesParams.LoadAsync()
                    };
                while (tasks.Count > 0)
                {
                    Task finished = await Task.WhenAny(tasks).ConfigureAwait(false);
                    tasks.Remove(finished);
                }
            }
        }
        public void Add(Employee val) {
            Employees.Local.Add(val);
        }
        public void Remove(Employee val) {
            Employees.Local.Remove(val);
        }
        public PayParamBaseModel Add(PayBaseRateParam val) {
            PayBaseRateParams.Local.Add(val);
            return val;
        }
        public PayParamBaseModel Add(PayForSubordinatesParam val) {
            PayForSubordinatesParams.Local.Add(val);
            return val;
        }
        public PayParamBaseModel Add(PayExperienceParam val) {
            PayExperienceParams.Local.Add(val);
            return val;
        }
        internal async Task<Employee> LoginAsync(string login, string passHash) {
            
            var list = await Employees.SqlQuery(
                "Select * from employees " +
                "where Login == @p0 and Password == @p1", login, passHash)
                .ToListAsync().ConfigureAwait(false);
            return list.Count == 1 ? list[0] : null;
        }
        // TODO: Реализовать тело метода DbExistsAndNotEmpty (https://habr.com/ru/post/56694/)
        private bool ValidateDatabase() {
            // проверка наличия файла базы данных
            // проверка наличия таблиц, создание в случае отсутствия
            // заполнение дефолтными значениями
            /*
            bool isEmpty = m_ctx.Employees.Local.Count == 0;
            if (isEmpty)
            {
                Employee admin = new Employee()
                {
                    Login = "root",
                    Password = "1",
                    GroupId = 0
                };
                m_ctx.Employees.Local.Add(admin);
                m_ctx.SaveChanges();
            }
            */

            return true;
        }
    }
}
