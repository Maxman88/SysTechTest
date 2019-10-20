using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace SysTechTest.dal
{

    public class AccessEntity : DbDictionary 
    {
        public string AccessTypeIds { get; set; }
    }
    public class AccessType : DbDictionary { }
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

    public class AccessEntityByGroup: BaseModel
    {
        private int m_id = 0;
        private int m_groupId = 0;
        private int m_accessEntityId = 0;
        private bool m_enable = true;
        private string m_params="";
        public int Id {
            get { return m_id; }
            set { m_id = value; OnPropertyChanged(); }
        }
        public int GroupId {
            get { return m_groupId; }
            set { m_groupId = value; OnPropertyChanged(); }
        }
        public int AccessEntityId {
            get { return m_accessEntityId; }
            set { m_accessEntityId = value; OnPropertyChanged(); }
        }
        public bool Enable {
            get { return m_enable; }
            set { m_enable = value; OnPropertyChanged(); }
        }
        public string Params {
            get { return m_params; }
            set { m_params = value; OnPropertyChanged(); }
        }
    }

 
    public class Employee : BaseModel
    {
        private Int64 m_id = 0;
        private Int64 m_parentId = 0;
        private int m_groupId = 0;
        private string m_name = "";
        private string m_dateOfEmployment = DateTimeUtils.DateToStr(DateTime.Now);
        private decimal m_baseRate = new decimal();
        private string m_login = "";
        private string m_password = "";
        private readonly List<Employee> m_subordinates = new List<Employee>();
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
        public decimal BaseRate {
            get { return m_baseRate; }
            set { m_baseRate = value; OnPropertyChanged(); }
        }
        public string Login {
            get { return m_login; }
            set { m_login = value; OnPropertyChanged(); }
        }
        public string Password {
            get { return m_password; }
            set { m_password = value; OnPropertyChanged(); }
        }
        public List<Employee> GetSubordinates() {
            return m_subordinates;
        }
    }
}
