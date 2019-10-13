using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace SysTechTest.dal
{
    public class Group { 
        public int Id { get; set; }
        public string GroupName { get; set; }
    }
    public class AccessType { 
        public int Id { get; set; }
        public string TypeName { get; set; }
    }
    public class GroupPaySystem
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public bool Enable { get; set; }
        public string Params { get; set; }
    }


    public class Employee : INotifyPropertyChanged
    {
        private Int64 m_id = 0;
        private Int64 m_parentId = 0;
        private int m_groupId = 0;
        private string m_name = "Новый сотрудник";
        private string m_dateOfEmployment = DateTimeUtils.DateToStr(DateTime.Now);
        private decimal m_baseRate = new decimal();
        private string m_login = "";
        private string m_password = "";
        private byte m_access = 0;
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
        public byte Access {
            get { return m_access; }
            set { m_access = value; OnPropertyChanged(); }
        }
        public List<Employee> GetSubordinates() {
            return m_subordinates;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
