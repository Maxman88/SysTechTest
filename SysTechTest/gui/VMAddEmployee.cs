using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using SysTechTest.cmd;
using SysTechTest.dal;

namespace SysTechTest.gui
{
    class VMAddEmployee : VMNotifyBase {
        private bool m_subscribed = false;
        private readonly Employee m_item;
        private readonly WndAddEmployee m_wnd;

        public VMAddEmployee(WndAddEmployee wnd) {
            m_wnd = wnd;
            m_item = new Employee() { Name = "Новый сотрудник" };
            CmdAdd = new CmdAction(arg => AddEmployee())
            {
                CanExecuteDelegate = CanExecuteCmdAdd
            };
            CmdCancel = new CmdAction(arg => m_wnd.Close());
            GroupsView = new ObservableCollection<Group>(CtrlDbCtx.Instance.GroupsOfEmployee);
            UpdateListAvailableChiefs();
            Subscribe();
        }
        ~VMAddEmployee() {
            Unsubscribe();
        }
        public string Name { get => m_item.Name; set => m_item.Name = value; }
        public long ParentId { get => m_item.ParentId; set => m_item.ParentId = value; }
        public int GroupId { get => m_item.GroupId; set => m_item.GroupId = value; }
        
        public DateTime DateOfEmployment { 
            get => DateTimeUtils.DateFromStr(m_item.DateOfEmployment); 
            set => m_item.DateOfEmployment = DateTimeUtils.DateToStr(value); 
        }
        public ObservableCollection<Group> GroupsView { get; set; }
        private ObservableCollection<Employee> m_listAvailableChiefs;
        public ObservableCollection<Employee> ListAvailableChiefs {
            get => m_listAvailableChiefs;
            private set { 
                m_listAvailableChiefs = value; 
                OnPropertyChanged();
                m_item.ParentId = m_item.ParentId;
            }
        }
        public string Login { get => m_item.Login; set => m_item.Login = value; }
        public CmdAction CmdAdd { get; private set; }
        public CmdAction CmdCancel { get; private set; }
        private void UpdateListAvailableChiefs() {
            var list = new List<Employee>() { new Employee() { Name = "Нет" } };
            list.AddRange(CtrlDbCtx.Instance.GetListAvailableChiefs(m_item));
            ListAvailableChiefs = new ObservableCollection<Employee>(list);
        }
        private void Subscribe() {
            if (m_subscribed == false)
            {
                m_wnd.PassBox.PasswordChanged += OnPasswordChanged;
                m_item.PropertyChanged += OnItemPropertyChanged;
                m_subscribed = true;
            }
        }
        private void Unsubscribe() {
            if (m_subscribed)
            {
                m_wnd.PassBox.PasswordChanged -= OnPasswordChanged;
                m_item.PropertyChanged -= OnItemPropertyChanged;
                m_subscribed = false;
            }
        }
        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e != null && e.PropertyName == "GroupId")
            {
                UpdateListAvailableChiefs();
            }
            CmdAdd.InvokeCanExecuteChanged();
        }
        private void OnPasswordChanged(object sender, RoutedEventArgs e) {
            CmdAdd.InvokeCanExecuteChanged();
        }
        private bool CanExecuteCmdAdd(object obj = null) {
            var res = false == string.IsNullOrEmpty(m_item.Login)
                     && false == string.IsNullOrEmpty(m_item.Name)
                     && false == string.IsNullOrEmpty(m_wnd.PassBox.Password);
            return res;
        }
        private void AddEmployee() {
            try
            {
                
                if (CanExecuteCmdAdd())
                {
                    if (false == CtrlDbCtx.Instance.IsLoginUnique(m_item.Login))
                    {
                        _ = MessageBoxEx.Show(m_wnd,"Ошибка, логин " + m_item.Login + " занят. Выберите другой.", "Ошибка. ");
                    }
                    else
                    {
                        Unsubscribe();
                        CtrlDbCtx.Instance.Add(m_item, m_wnd.PassBox.Password);
                        m_wnd.Close();
                    }
                }
                else
                {
                    _ = MessageBoxEx.Show(m_wnd, "Не все поля заполнены.");
                }


            }
            catch(Exception ex)
            {
                // TODO: Нужно как то обрабатывать.
                throw;
            }
        }
    }
}
