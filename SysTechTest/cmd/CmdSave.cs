using System;
using System.ComponentModel;
using System.Windows.Input;
using SysTechTest.dal;
using SysTechTest.gui;

namespace SysTechTest.cmd
{
    class CmdSave : ICommand
    {
        private VMSettings m_vm;

        public event EventHandler CanExecuteChanged;
        public CmdSave(VMSettings vm) {
            m_vm = vm;
            m_vm.PropertyPaymentChanged += Invoke;
            CtrlDbCtx.Instance.OnCollectionChanged += Invoke;
        }
        ~CmdSave() {
            m_vm.PropertyPaymentChanged -= Invoke;
            CtrlDbCtx.Instance.OnCollectionChanged -= Invoke;
        }
        private void Invoke(object sender, EventArgs e) {
            CanExecuteChanged?.Invoke(sender, e);

        }
        public bool CanExecute(object parameter) {
            return CtrlDbCtx.Instance.HasChanges;
        }

        public void Execute(object parameter) {
            CtrlDbCtx.Instance.SaveChangesIfExists();
            Invoke(this, EventArgs.Empty);
        }
    }
}
