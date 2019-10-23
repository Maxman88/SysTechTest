using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using SysTechTest.cmd;
using SysTechTest.dal;
using SysTechTest.PaySystems;

namespace SysTechTest.gui
{
    public class VMTreeEmployees : VMNotifyBase
    {
        private readonly Employee m_empty = new Employee();
        public VMTreeEmployees() {
            CmdSave = new CmdSave(this);
            CmdCalcPayroll = new CmdAction(arg => CalcPayroll()) { CanExecuteDelegate = CanCalculatePayroll };
            CmdCalcPayrollAll = new CmdAction(arg => CalcPayrollAll()) { CanExecuteDelegate = CanCalculatePayroll };
            CmdChangeLoginPass = new CmdAction(arg => OpenWndChangePass());
            CmdAddEmployee = new CmdAction(arg => AddEmployee()) { CanExecuteDelegate = CanChangeListEmployees };
            CmdRemoveEmployee = new CmdAction(arg => RemoveEmployee()) { CanExecuteDelegate = CanChangeListEmployees };
        }

        ~VMTreeEmployees() {
            if (Selected != null)
            {
                SafeUnsubscribe(Selected);
            }
            foreach (var node in EmployeesView)
            {
                UnSubscribe(node);
            }
        }
        #region Properties
        public ICommand CmdSave { get; private set; }
        public ICommand CmdChangeLoginPass { get; private set; }
        public CmdAction CmdCalcPayroll { get; private set; }
        public CmdAction CmdCalcPayrollAll { get; private set; }
        public CmdAction CmdAddEmployee { get; private set; }
        public CmdAction CmdRemoveEmployee { get; private set; }

        private bool m_adminHere = false;
        public bool AdminHere {
            get => m_adminHere;
            set { m_adminHere = value; OnPropertyChanged(); }
        }
        private Visibility m_adminVisibility = Visibility.Collapsed;
        public Visibility AdminVisibility {
            get => m_adminVisibility;
            set { m_adminVisibility = value; OnPropertyChanged(); }
        }
        private bool m_calcPayrollAvailable = false;
        private bool CanCalculatePayroll(object obj = null) => m_calcPayrollAvailable;
        private bool CanChangeListEmployees(object obj = null) => AdminHere;
        public ObservableCollection<NodeEmployee> EmployeesView { get; private set; } = new ObservableCollection<NodeEmployee>();
        private ObservableCollection<Group> m_groupsView;
        public ObservableCollection<Group> GroupsView {
            get => m_groupsView;
            private set { m_groupsView = value; OnPropertyChanged(); }
        }
        private ObservableCollection<Employee> m_listAvailableChiefs;
        public ObservableCollection<Employee> ListAvailableChiefs {
            get => m_listAvailableChiefs;
            private set { m_listAvailableChiefs = value; OnPropertyChanged(); }
        }

        private Employee m_selectedOrNull;
        public Employee Selected {
            get => m_selectedOrNull ?? m_empty;
            set {
                SafeUnsubscribe(m_selectedOrNull);
                m_selectedOrNull = value;
                SafeSubscribe(m_selectedOrNull);
                OnPropertyChanged();
            }
        }
        private NodeEmployee m_selectedNode;
        private NodeEmployee SelectedNode {
            get => m_selectedNode;
            set {
                m_selectedNode = value;
                Selected = m_selectedNode?.Owner;
            }
        }
        private DateTime m_dateBeginOfCalc;
        public DateTime DateBeginOfCalc {
            get => m_dateBeginOfCalc;
            set { m_dateBeginOfCalc = value; CheckCanCalculatePayroll(); OnPropertyChanged(); }
        }
        private DateTime m_dateEndOfCalc;
        public DateTime DateEndOfCalc {
            get => m_dateEndOfCalc;
            set { m_dateEndOfCalc = value; CheckCanCalculatePayroll(); OnPropertyChanged(); }
        }

        #endregion
        internal void Update() {
            SafeUnsubscribe(Selected);
            foreach( var node in EmployeesView)
            {
                UnSubscribe(node);
            }
            EmployeesView.Clear();

            DateBeginOfCalc = DateEndOfCalc = DateTime.Now.Date;
            GroupsView = new ObservableCollection<Group>(CtrlDbCtx.Instance.GroupsOfEmployee);
            var employees = CtrlDbCtx.Instance.EmployeesByCurrentUser;
            
            void AddChild(NodeEmployee node) {
                Subscribe(node);
                foreach (var item in employees.FindAll(item => item.ParentId == node.Owner.Id))
                {
                    var child = new NodeEmployee(item);
                    AddChild(child);
                    node.Childs.Add(child);
                }
            }
            void AttachEmployee(Employee item) {
                var node = new NodeEmployee(item);
                EmployeesView.Add(node);
                AddChild(node);
            }
            if (CtrlDbCtx.Instance.AdminHere) {
                AdminHere = true;
                AdminVisibility = Visibility.Visible;
                foreach (Employee root in employees.FindAll(item => item.ParentId == Employee.NoParent))
                {
                    AttachEmployee(root);
                }
            }
            else {
                AdminHere = false;
                AdminVisibility = Visibility.Collapsed;
                AttachEmployee(CtrlDbCtx.Instance.CurrentUserOrNull);
            }

            if (EmployeesView.Count > 0) {
                EmployeesView[0].IsSelected = true;
            }
            CmdAddEmployee.InvokeCanExecuteChanged();
            CmdRemoveEmployee.InvokeCanExecuteChanged();
            OnPropertyChanged("EmployeesView");
        }
        private void Subscribe(NodeEmployee nodeEmployee) {
            nodeEmployee.PropertyChanged += OnTreeViewItemSelectedChange;
            foreach (var item in nodeEmployee.Childs)
            {
                Subscribe(item);
            }
        }
        private void UnSubscribe(NodeEmployee nodeEmployee) {
            nodeEmployee.PropertyChanged -= OnTreeViewItemSelectedChange;
            foreach (var item in nodeEmployee.Childs)
            {
                UnSubscribe(item);
            }
        }

        private void SafeSubscribe(Employee emplOrNull) {
            if(emplOrNull != null)
            {
                emplOrNull.PropertyChanged += OnPropertyChanged;
            }
        }
        private void SafeUnsubscribe(Employee emplOrNull) {
            if (emplOrNull != null)
            {
                emplOrNull.PropertyChanged -= OnPropertyChanged;
            }
        }

        private void OnTreeViewItemSelectedChange(object sender, PropertyChangedEventArgs e) {
            var node = (NodeEmployee)sender;
            if (node != null && node.IsSelected)
            {
                var list = new List<Employee>() { new Employee() { Name = "Нет" } };
                list.AddRange(CtrlDbCtx.Instance.GetListAvailableChiefs(node.Owner));
                ListAvailableChiefs = new ObservableCollection<Employee>(list);
                SelectedNode = node;
            }
            else
            {
                ListAvailableChiefs.Clear();
                SelectedNode = null;
            }
        }
        public override void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
            base.OnPropertyChanged(sender, e);
            var item = (Employee)sender;
            if (item != null && e?.PropertyName == "ParentId") {
                Update();
                SetFocusAt(item);
            }
        }


        private void CheckCanCalculatePayroll() {
            m_calcPayrollAvailable = m_dateBeginOfCalc.Date <= m_dateEndOfCalc.Date;
            CmdCalcPayroll.InvokeCanExecuteChanged();
            CmdCalcPayrollAll.InvokeCanExecuteChanged();
        }
        private void OpenWndChangePass() {
            if(m_selectedOrNull != null)
            {
                var wnd = new WndChangeLoginPass() { Owner = App.Current.MainWindow };
                var ctx = new VMChangeLoginPass(m_selectedOrNull, wnd);
                wnd.DataContext = ctx;
                wnd.ShowDialog();
            }
        }
        private void CalcPayroll() {
            if(m_selectedOrNull!=null)
            {
                var t = CtrlWorkPayment.Instance.CalcWorkPayment(m_selectedOrNull, DateBeginOfCalc, DateEndOfCalc);
                _ = MessageBoxEx.Show(App.Current.MainWindow, "Оплата для " + Selected.Name + " равна " + t.ToString("C"));
            }
        }
        private void CalcPayrollAll() {
            var t = CtrlWorkPayment.Instance.CalcWorkPaymentAll(DateBeginOfCalc, DateEndOfCalc);
            _ = MessageBoxEx.Show(App.Current.MainWindow, "Оплата всего штата равна " + t.ToString("C"));
        }

        private void SetFocusAt(Employee empl) {
            NodeEmployee find(NodeEmployee node1) {
                if(node1.Owner == empl)
                {
                    return node1;
                }
                foreach (var child in node1.Childs)
                {
                    return find(child);
                }
                return null;
            }


            foreach (var node in EmployeesView)
            {
                var res = find(node);
                if (res!=null)
                {
                    res.IsSelected = true;
                    break;
                }
            }
        }
        // TODO: AddEmployee добавление нужно организовывать через отдельное окно с валидацией
        private void AddEmployee() {


            var newEmpl = new Employee() { Name = "Новый сотрудник", Login = "", GroupId = (int)DbHelpers.Group.Employee };
            CtrlDbCtx.Instance.Add(newEmpl);
            Update();
            SetFocusAt(newEmpl);
        }

        private void RemoveEmployee() {
            if (SelectedNode != null)
            {
                var result = MessageBoxEx.Show(App.Current.MainWindow, "Удалить сотрудника " + SelectedNode.Owner.Name +" ?", "Удаление", MessageBoxButton.YesNo);
                if(result == MessageBoxResult.No)
                {
                    return;
                }
                SafeUnsubscribe(SelectedNode.Owner);
                UnSubscribe(SelectedNode);
                var idx = EmployeesView.IndexOf(SelectedNode);
                CtrlDbCtx.Instance.Remove(SelectedNode.Owner);
                Update();
                if (idx == EmployeesView.Count )
                {
                    idx--;
                }
                if (0 <= idx && 0 < EmployeesView.Count)
                {
                    EmployeesView[idx].IsSelected = true;
                }
            }
        }


    }
    public class NodeEmployee : VMNotifyBase
    {
        private bool m_isSelected;

        public NodeEmployee(Employee owner) {
            Owner = owner;
        }
        public Employee Owner { get; }
        public bool IsSelected {
            get => m_isSelected;
            set { m_isSelected = value; OnPropertyChanged(); }
        }
        public ObservableCollection<NodeEmployee> Childs { get; } = new ObservableCollection<NodeEmployee>();
    }
}
