using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace SysTechTest.dal
{
    public class CtrlDbCtx : IDisposable {
        #region Singleton
        private static CtrlDbCtx m_instance;
        public static CtrlDbCtx Instance {
            get {
                if (m_instance == null) {
                    m_instance = new CtrlDbCtx();
                }
                return m_instance;
            }
        }
        #endregion
        //------------
        private readonly DbCtx m_ctx = new DbCtx();
        //------------
        private List<Group> Groups { get; set; } = new List<Group>();
        public List<Employee> Employees { get; private set; } = new List<Employee>();
        public List<PaySystemDesc> PaySystemDescs { get; private set; } = new List<PaySystemDesc>();


        public List<Group> GroupsOfEmployee { get => Groups.FindAll(p => p.Id != (int) DbHelpers.Group.Supervisor); }
    public Employee CurrentUserOrNull { get; private set; } = null;
        public bool LoggedIn { get => CurrentUserOrNull != null; }
        public bool HasChanges => m_ctx.HasChanges;

        private CtrlDbCtx() {
        }
        public async Task InitializeAsync() {
            await m_ctx.InitializeAsync().ConfigureAwait(false);
            var t1 = m_ctx.Groups.ToListAsync();
            var t2 = m_ctx.Employees.ToListAsync();
            var t3 = m_ctx.PaySystems.ToListAsync();

            var t4 = m_ctx.PayBaseRateParams.ToListAsync();
            var t5 = m_ctx.PayExperienceParams.ToListAsync();
            var t6 = m_ctx.PayForSubordinatesParams.ToListAsync();
            var PayBaseRateParams = new List<PayBaseRateParam>();
            var PayExperienceParams = new List<PayExperienceParam>();
            var PayForSubordinatesParams = new List<PayForSubordinatesParam>();

            List<Task> tasks = new List<Task>() { t1, t2, t3, t4, t5, t6 };
            while (tasks.Count > 0)
            {
                Task finished = await Task.WhenAny(tasks).ConfigureAwait(false);
                try
                {
                    if (finished == t1) Groups = t1.Result;
                    else if (finished == t2) Employees = t2.Result;
                    else if (finished == t3) PaySystemDescs = t3.Result;
                    else if (finished == t4) PayBaseRateParams = t4.Result;
                    else if (finished == t5) PayExperienceParams = t5.Result;
                    else if (finished == t6) PayForSubordinatesParams = t6.Result;
                    if (finished.IsFaulted)
                    {
                        throw new Exception("InitializeAsync: Can't load table from db.");
                    }
                    tasks.Remove(finished);
                }
                catch (AggregateException ex)
                {
                    throw new Exception("InitializeAsync: Can't load table from db.", ex);
                }
            }
            // Build employee subordinates
            foreach (Employee empl in Employees)
            {
                empl.GetSubordinates().AddRange(Employees.FindAll(item => item.ParentId == empl.Id));
            }
            // Check integrity pay systems
            foreach (var gr in Groups)
            {
                var p1 = PayBaseRateParams.Find(p => p.GroupId == gr.Id);
                if (null == p1)
                {
                    p1 = (PayBaseRateParam)m_ctx.Add(new PayBaseRateParam() { GroupId = gr.Id });
                }
                var p2 = PayExperienceParams.Find(p => p.GroupId == gr.Id);
                if (null == p2)
                {
                    p2 = (PayExperienceParam)m_ctx.Add(new PayExperienceParam() { GroupId = gr.Id });
                }
                var p3 = PayForSubordinatesParams.Find(p => p.GroupId == gr.Id);
                if (null == p3)
                {
                    p3 = (PayForSubordinatesParam)m_ctx.Add(new PayForSubordinatesParam() { GroupId = gr.Id });
                }
                gr.SetPaySystems(new PaySystem(p1,p2,p3));
            }
            if (HasChanges)
            {
                await m_ctx.SaveChangesAsync().ConfigureAwait(false);
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
        //TODO: Нужно реализовывать
        public async Task RollBackAsync() {

        }

        public event EventHandler LoginSuccess;
        public event EventHandler LoginFail;
        public async Task LoginAsync( string login, string pass ) {
            if(string.IsNullOrEmpty(login) || string.IsNullOrEmpty(pass))
            {
                throw new ArgumentNullException("login or password must be not empty.");
            }
            CurrentUserOrNull = await m_ctx.LoginAsync( login, pass ).ConfigureAwait(false);
            if (CurrentUserOrNull != null)
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
