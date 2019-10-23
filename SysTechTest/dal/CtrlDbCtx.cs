using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace SysTechTest.dal
{
    public class ChangeCollectionEventArgs : EventArgs {
        public bool Added { get; set; }
        public bool Removed { get; set; }
        public Employee Item { get; set; }

    }
    public class CtrlDbCtx : IDisposable {
        private static readonly List<Employee> EmptyListEmployees = new List<Employee>();
             
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
        private List<Employee> Employees { get; set; } = new List<Employee>();

        public Employee CurrentUserOrNull { get; private set; } = null;

        public void UpdateLoginPassword(Employee item, string loginTxt, string password) {
            if(item == null)
            {
                throw new ArgumentNullException("UpdateLoginPassword. parametr item is null..");
            }

            if(item.Login != loginTxt && false == IsLoginUnique(loginTxt))
            {
                throw new ArgumentException("UpdateLoginPassword. login is not unique!!!");
            }
            else
            {
                item.Login = loginTxt;
                item.Password = EncodePass(password);
                SaveChangesIfExists();
            }
            
        }

        public bool IsLoginUnique(string loginTxt) {
            return Employees.FindAll(p => p.Login == loginTxt).Count == 0;
        }

        public bool LoggedIn { get => CurrentUserOrNull != null; }
        public bool AdminHere { get => CurrentUserOrNull != null ? IsAdmin(CurrentUserOrNull.GroupId) : false; }
        public bool HasChanges => m_ctx.HasChanges;
        public List<Group> GroupsOfEmployee { get => Groups.FindAll(p => IsAdmin(p.Id) == false); }
        public List<Employee> GetListEmployees() => Employees.FindAll(p => IsAdmin(p.GroupId) == false);
        public List<Employee> GetListAdmins() => Employees.FindAll(p => IsAdmin(p.GroupId) == true);
        public List<Employee> EmployeesByCurrentUser {
            get {

                if (CurrentUserOrNull == null)
                {
                    return EmptyListEmployees;
                }
                if (AdminHere)
                {
                    return GetListEmployees();
                }


                List<Employee> AddChild(Employee owner, List<Employee> acc) {
                    foreach (var item in Employees.FindAll(item => item.ParentId == owner.Id))
                    {
                        AddChild(item, acc);
                        acc.Add(item);
                    }
                    return acc;
                }
                return AddChild(CurrentUserOrNull, new List<Employee>() { CurrentUserOrNull });
            }
        }


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

            var PaySystemDescs = new List<PaySystemDesc>();
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
                p1.SetPaySystemDesc(PaySystemDescs.Find(p => p.Id == p1.PaySystemDescId));
                p2.SetPaySystemDesc(PaySystemDescs.Find(p => p.Id == p2.PaySystemDescId));
                p3.SetPaySystemDesc(PaySystemDescs.Find(p => p.Id == p3.PaySystemDescId));
                
                gr.SetPaySystems(new PaySystem(p1,p2,p3));
            }
            if (HasChanges)
            {
                await m_ctx.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        internal List<Employee> GetListAvailableChiefs(Employee candidat) {
            var res = Employees.FindAll(p =>
                  p.GroupId != (int)DbHelpers.Group.Employee
               && false == IsAdmin(p.GroupId)
               && (p.Id != candidat.Id)
            );
            return res;
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
        public void Add(Employee val, string password) {
            val.Password = EncodePass(password);
            m_ctx.Add(val);
            Employees.Add(val);
            m_ctx.SaveChanges();
            OnCollectionChangedHandler(val, new ChangeCollectionEventArgs { Removed = false, Item = val, Added = true });

        }
        private string EncodePass(string password) {
            return Crypto.GetSHA256Hash(password);
        }

        //TODO: Remove реализовать обработку ошибок
        public void Remove(Employee val) {
            foreach (Employee empl in Employees.FindAll(item => item.ParentId == val.Id))
            {
                empl.ParentId = Employee.NoParent;
            }

            m_ctx.Remove(val);
            Employees.Remove(val);
            m_ctx.SaveChanges();
            OnCollectionChangedHandler(val, new ChangeCollectionEventArgs {Removed=true, Item=val, Added=false });
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
            if(string.IsNullOrEmpty(login) || string.IsNullOrEmpty(pass))
            {
                throw new ArgumentNullException("login or password must be not empty.");
            }
            CurrentUserOrNull = await m_ctx.LoginAsync(login, EncodePass(pass)).ConfigureAwait(false);
            if (CurrentUserOrNull != null)
            {
                LoginSuccess?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                LoginFail?.Invoke(this, EventArgs.Empty);
            }
        }
        private static bool IsAdmin(int GroupId) {
            return GroupId == (int)DbHelpers.Group.Supervisor;
        }

        public event EventHandler OnCollectionChanged;
        private void OnCollectionChangedHandler(object sender, EventArgs e) => OnCollectionChanged?.Invoke(sender, e);
    }
}
