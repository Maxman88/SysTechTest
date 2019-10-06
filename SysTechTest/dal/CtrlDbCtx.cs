using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace SysTechTest.dal
{
    public class CtrlDbCtx : IDisposable
    {
        #region Singleton
        private static CtrlDbCtx m_instance;
        public static CtrlDbCtx Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new CtrlDbCtx();
                }
                return m_instance;
            }
        }
        #endregion
        //------------
        private readonly DbCtx m_ctx;
        private Employee m_currentOrNull = null;
        //------------
        public Dictionary<int, string> Groups;
        public Dictionary<int, string> AccessTypes;
        public List<Employee> Employees;
        public bool LoggedIn { get { return m_currentOrNull != null; } }
        public bool HasChanges => m_ctx.HasChanges;

        private CtrlDbCtx() {
            m_ctx = new DbCtx();
            Groups = new Dictionary<int, string>();
            AccessTypes = new Dictionary<int, string>();
            Employees = new List<Employee>();
        }
        public async Task InitializeAsync() {
            await m_ctx.InitializeAsync();
            var t1 = m_ctx.AccessTypes.ToDictionaryAsync(pair => pair.Id, el=>el.TypeName);
            var t2 = m_ctx.Groups.ToDictionaryAsync(pair => pair.Id, el => el.GroupName);
            var t3 = m_ctx.Employees.ToListAsync();
            var tasks = new List<Task>() { t1, t2, t3 };
            while(tasks.Count > 0)
            {
                Task finished = await Task.WhenAny(tasks);
                if(finished == t1)
                {
                    this.AccessTypes = t1.Result;
                }
                else if(finished == t2)
                {
                    this.Groups = t2.Result;
                }
                else if(finished == t3)
                {
                    this.Employees = t3.Result;
                }
                tasks.Remove(finished);
            }
            foreach (Employee empl in Employees)
            {
                empl.GetSubordinates().AddRange(Employees.FindAll(item => item.ParentId == empl.Id));
            }
        }
        #region IDisposable Support
        private bool disposedValue = false; // Для определения избыточных вызовов
        protected virtual void Dispose(bool disposing) { 
            if (!disposedValue)
            {
                if (disposing)
                {
                    m_ctx.Dispose();
                }
                m_instance = null;
                disposedValue = true;
            }
        }

        // TODO: переопределить метод завершения, только если Dispose(bool disposing) выше включает код для освобождения неуправляемых ресурсов.
        // ~CtrlDbCtx()
        // {
        //   // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
        //   Dispose(false);
        // }

        // Этот код добавлен для правильной реализации шаблона высвобождаемого класса.
        public void Dispose() {
            // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
            Dispose(true);
            // TODO: раскомментировать следующую строку, если метод завершения переопределен выше.
            // GC.SuppressFinalize(this);
        }
        #endregion
        //TODO: Add реализовать обработку ошибок + проверка полей (например поле login должно быть уникальным)
        public void Add(Employee val) {
            m_ctx.Add(val);
            Employees.Add(val);
        }
        //TODO: Remove реализовать обработку ошибок
        public void Remove(Employee val) {
            m_ctx.Remove(val);
            Employees.Remove(val);
        }
        public void SaveChangesIfExists() {
            if (HasChanges)
            {
                m_ctx.SaveChanges();
            }
        }
        public event EventHandler LoginSuccess;
        public event EventHandler LoginFail;
        public async Task LoginAsync( string login, string pass ) {
            m_currentOrNull = await m_ctx.LoginAsync( login, pass );
            if (m_currentOrNull != null)
            {
                LoginSuccess?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                LoginFail?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler OnCollectionChanged;
        private void OnCollectionChangedHandler(object sender, EventArgs e) => OnCollectionChanged?.Invoke(sender, e);
    }
}
