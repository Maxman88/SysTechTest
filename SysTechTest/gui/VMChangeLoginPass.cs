using System;
using System.Windows;
using SysTechTest.cmd;
using SysTechTest.dal;

namespace SysTechTest.gui
{
    class VMChangeLoginPass : VMNotifyBase
    {
        private readonly Employee m_item;
        private readonly WndChangeLoginPass m_wnd;
        private bool m_passBoxSigned = false;
        private readonly System.Windows.Controls.PasswordBox m_passwordBox;
        public VMChangeLoginPass(Employee item, WndChangeLoginPass wnd) {
            m_item = item;
            m_wnd = wnd;
            m_passwordBox = m_wnd.PassTxt;
            CmdClose = new CmdAction(arg => m_wnd.Close());
            CmdUpdLoginPass = new CmdAction(arg => UpdateLoginPass())
            {
                CanExecuteDelegate = CanExecuteChange
            };
            m_passBoxSigned = true;
            m_passwordBox.PasswordChanged += PasswordChanged;
            LoginTxt = m_item.Login;
        }

        ~VMChangeLoginPass() {
            Unsubscribe();
        }
        public CmdAction CmdClose { get; private set; }
        public CmdAction CmdUpdLoginPass { get; private set; }

        private string m_loginTxt;
        public string LoginTxt {
            get => m_loginTxt;
            set { m_loginTxt = value; CheckCanLogin(); OnPropertyChanged(); }
        }
        private bool m_canUpdLoginPass = false;
        public bool CanUpdLoginPass {
            get => m_canUpdLoginPass;
            private set {
                m_canUpdLoginPass = value;
                CmdUpdLoginPass.InvokeCanExecuteChanged();
                OnPropertyChanged();
            }
        }
        private bool CanExecuteChange(object obj = null) => CanUpdLoginPass;
        private void PasswordChanged(object sender, RoutedEventArgs e) => CheckCanLogin();
        private void CheckCanLogin() {
            CanUpdLoginPass = false == string.IsNullOrEmpty(LoginTxt)
                           && false == string.IsNullOrEmpty(m_passwordBox.Password);
        }
        private void UpdateLoginPass() {
            if (CanUpdLoginPass)
            {
                if (LoginTxt != m_item.Login && false == CtrlDbCtx.Instance.IsLoginUnique(LoginTxt))
                {
                    _ = MessageBoxEx.Show(m_wnd, "Ошибка при смене логина пароля", "Логин " + LoginTxt + " занят. Выберите другой.");
                }
                else
                {
                    CtrlDbCtx.Instance.UpdateLoginPassword(m_item, LoginTxt, m_passwordBox.Password);
                    CmdClose.Execute(EventArgs.Empty);
                }
            }
        }
        private void Unsubscribe() {
            if(m_passBoxSigned)
            {
                m_passBoxSigned = false;
                m_passwordBox.PasswordChanged -= PasswordChanged;
            }
        }
    }
}
