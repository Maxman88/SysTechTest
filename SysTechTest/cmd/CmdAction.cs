using System;
using System.Windows.Input;

namespace SysTechTest.cmd
{
    public class CmdAction : ICommand
    {
        public CmdAction(Action<object> action) { ExecuteDelegate = action; }
        public Predicate<object> CanExecuteDelegate { get; set; }
        public Action<object> ExecuteDelegate { get; set; }
        public void Execute(object parameter) {
            ExecuteDelegate?.Invoke(parameter);
            Invoke(this, EventArgs.Empty);
        }
        public bool CanExecute(object parameter) {
            if (CanExecuteDelegate != null)
            {
                return CanExecuteDelegate(parameter);
            }
            return true;
        }

        public event EventHandler CanExecuteChanged;
        private void Invoke(object sender, EventArgs e) => CanExecuteChanged?.Invoke(sender, e);


    }
}
