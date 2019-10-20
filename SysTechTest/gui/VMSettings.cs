using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using SysTechTest.cmd;
using SysTechTest.dal;
using Group = SysTechTest.dal.Group;

namespace SysTechTest.gui
{
    internal partial class VMSettings : INotifyPropertyChanged {
        private bool m_selectedIsAvailable;
        private Group m_selectedItemOrNull;
        private readonly PaySystem m_default;
        private PayBaseRateParam m_payBaseRateParams;
        private PayExperienceParam m_payExperienceParams;
        private PayForSubordinatesParam m_payForSubordinatesParams;

        public ICommand CmdSave { get; private set; }

        public VMSettings() {
            CmdSave = new CmdSave(this);
            Groups = new ObservableCollection<Group>(CtrlDbCtx.Instance.GroupsOfEmployee);
            m_default = new PaySystem();
            SelectedItem = Groups.Count == 0 ? null : Groups[0];
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "") {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        public ObservableCollection<Group> Groups { get; private set; }
        public PayBaseRateParam PayBaseRateParams {
            get { return m_payBaseRateParams; }
            private set { m_payBaseRateParams = value; OnPropertyChanged(); }
        }
        public PayExperienceParam PayExperienceParams {
            get => m_payExperienceParams;
            private set { m_payExperienceParams = value; OnPropertyChanged(); }
        }
        public PayForSubordinatesParam PayForSubordinatesParams {
            get => m_payForSubordinatesParams;
            private set { m_payForSubordinatesParams = value; OnPropertyChanged(); }
        }
        public bool SelectedIsAvailable { 
            get => m_selectedIsAvailable;
            set { m_selectedIsAvailable = value; OnPropertyChanged(); }
        }
        public Group SelectedItem {
            get => m_selectedItemOrNull;
            set {
                if(m_selectedItemOrNull != null)
                {
                    PayBaseRateParams.PropertyChanged -= OnPaySystemPropertyChanged;
                    PayExperienceParams.PropertyChanged -= OnPaySystemPropertyChanged;
                    PayForSubordinatesParams.PropertyChanged -= OnPaySystemPropertyChanged;
                }
                m_selectedItemOrNull = value;
                SelectedIsAvailable = m_selectedItemOrNull != null;
                var ps = m_selectedItemOrNull == null ? m_default : m_selectedItemOrNull.GetPaySystems();
                PayBaseRateParams = ps.PayBaseRateParams;
                PayExperienceParams = ps.PayExperienceParams;
                PayForSubordinatesParams = ps.PayForSubordinatesParams;
                if (m_selectedItemOrNull != null)
                {
                    PayBaseRateParams.PropertyChanged += OnPaySystemPropertyChanged;
                    PayExperienceParams.PropertyChanged += OnPaySystemPropertyChanged;
                    PayForSubordinatesParams.PropertyChanged += OnPaySystemPropertyChanged;
                }
                OnPropertyChanged();
            }
        }
        public event EventHandler PropertyPaymentChanged;
        private void OnPaySystemPropertyChanged(object sender, PropertyChangedEventArgs e) {
            PropertyPaymentChanged?.Invoke(sender, e);
        }
    }
}
