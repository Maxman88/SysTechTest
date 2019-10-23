using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace SysTechTest.dal
{

    public class Group : DbDictionary {
        private PaySystem m_paySystem;
        public void SetPaySystems(PaySystem val) => m_paySystem = val;
        public PaySystem GetPaySystems() => m_paySystem;
    }

    public class BaseModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "") {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
    public class Employee : BaseModel
    {
        public const long NoParent = -1;
 
        private Int64 m_id = (long)NoParent;
        private Int64 m_parentId = (long)NoParent;
        private int m_groupId = (int)DbHelpers.Group.Employee;
        private string m_name = "";
        private string m_dateOfEmployment = DateTimeUtils.DateToStr(DateTime.Now);
        private string m_login = "";
        private string m_password = "";
        [Key]
        public Int64 Id {
            get { return m_id; }
            set { m_id = value; OnPropertyChanged(); }
        }
        public Int64 ParentId {
            get { return m_parentId; }
            set { m_parentId = value; OnPropertyChanged(); }
        }
        public int GroupId {
            get { return m_groupId; }
            set { m_groupId = value; OnPropertyChanged(); }
        }
        public string Name {
            get { return m_name; }
            set { m_name = value; OnPropertyChanged(); }
        }
        public string DateOfEmployment {
            get { return m_dateOfEmployment; }
            set { m_dateOfEmployment = value; OnPropertyChanged(); }
        }
        public string Login {
            get { return m_login; }
            set { m_login = value; OnPropertyChanged(); }
        }
        public string Password {
            get { return m_password; }
            set { m_password = value; OnPropertyChanged(); }
        }
    }
}
