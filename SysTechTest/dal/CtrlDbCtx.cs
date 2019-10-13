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
        public Dictionary<int, string> Groups { get; private set; }
        public Dictionary<int, string> AccessTypes { get; private set; }
        public Dictionary<int, List<GroupPaySystem>> PaySystemsByGroup { get; private set; }
        public List<Employee> Employees { get; private set; }
        public bool LoggedIn { get { return m_currentOrNull != null; } }
        public bool HasChanges => m_ctx.HasChanges;

        private CtrlDbCtx() {
            m_ctx = new DbCtx();
            Groups = new Dictionary<int, string>();
            AccessTypes = new Dictionary<int, string>();
            PaySystemsByGroup = new Dictionary<int, List<GroupPaySystem>>();
            Employees = new List<Employee>();
        }
        public async Task InitializeAsync() {
            await m_ctx.InitializeAsync().ConfigureAwait(false);
            var t1 = m_ctx.AccessTypes.ToDictionaryAsync(pair => pair.Id, el=>el.TypeName);
            var t2 = m_ctx.Groups.ToDictionaryAsync(pair => pair.Id, el => el.GroupName);
            var t3 = m_ctx.GroupPaySystems.ToDictionaryAsync(pair => pair.Id);
            var t4 = m_ctx.Employees.ToListAsync();
            var tasks = new List<Task>() { t1, t2, t3, t4 };
            while (tasks.Count > 0)
            {
                Task finished = await Task.WhenAny(tasks).ConfigureAwait(false);
                if(finished == t1)
                {
                    AccessTypes = t1.Result;
                }
                else if(finished == t2)
                {
                    Groups = t2.Result;
                }
                else if(finished == t3)
                {
                    foreach (var el in t3.Result)
                    {
                        if (PaySystemsByGroup.ContainsKey(el.Value.GroupId))
                        {
                            PaySystemsByGroup[el.Value.GroupId].Add(el.Value);
                        }
                        else
                        {
                            PaySystemsByGroup.Add(el.Value.GroupId, new List<GroupPaySystem>() { el.Value });
                        }
                    }
                }
                else if (finished == t4)
                {
                    Employees = t4.Result;
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
        ~CtrlDbCtx() {
            Dispose(false);
        }
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
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
            m_currentOrNull = await m_ctx.LoginAsync( login, pass ).ConfigureAwait(false);
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
