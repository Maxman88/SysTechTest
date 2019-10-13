using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SysTechTest.dal

{
    public class DbCtx : DbContext
    {
        public DbSet<Group> Groups { get; set; }
        public DbSet<AccessType> AccessTypes { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<GroupPaySystem> GroupPaySystems { get; set; }
        public bool HasChanges => ChangeTracker.HasChanges();
        public DbCtx() : base("name=DbConnection") {
            Configuration.AutoDetectChangesEnabled = true;
        }
        public async Task InitializeAsync() {
            if(DbExistsAndNotEmpty())
            {
                var t1 = Groups.LoadAsync();
                var t2 = AccessTypes.LoadAsync();
                var t3 = Employees.LoadAsync();
                var t4 = GroupPaySystems.LoadAsync();
                var tasks = new List<Task>() { t1, t2, t3, t4 };
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
        internal async Task<Employee> LoginAsync(string login, string pass) {
            string passHash = GetSHA256Hash(pass);
            var list = await Employees.SqlQuery(
                "Select * from employees " +
                "where Login == @p0 and Password == @p1", login, passHash)
                .ToListAsync().ConfigureAwait(false);
            return list.Count == 1 ? list[0] : null;
        }
        private static string GetSHA256Hash(string s) {
            if (string.IsNullOrEmpty(s))
            {
                throw new ArgumentException("An empty string value cannot be hashed.");
            }
            Byte[] data = System.Text.Encoding.UTF8.GetBytes(s);
            Byte[] hash;
            using ( var sha = SHA256CryptoServiceProvider.Create())
            {
                hash = sha.ComputeHash(data);
            }
            return Convert.ToBase64String(hash);
        }
        // TODO: Реализовать тело метода DbExistsAndNotEmpty (https://habr.com/ru/post/56694/)
        private bool DbExistsAndNotEmpty() {
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
