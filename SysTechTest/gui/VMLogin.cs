using System;
using System.Windows;
using SysTechTest.cmd;
using SysTechTest.dal;

namespace SysTechTest.gui
{
    public class VMLogin : VMNotifyBase
    {
        private readonly WndLoginDlg wndLogin;
        public VMLogin(WndLoginDlg wndLogin) {
            CommandLogin = new CmdAction(arg => ActionLoginAsync());
            CommandCloseApp = new CmdCloseApp();
            
            this.wndLogin = wndLogin ?? throw new ArgumentNullException(nameof(wndLogin), "VMLogin constructor: ArgumentNullException");
            wndLogin.PassTxt.PasswordChanged += PassTxt_PasswordChanged;
        }

        public CmdAction CommandLogin { get; private set; }
        public CmdAction CommandCloseApp { get; private set; }
        private string m_loginTxt;
        public string LoginTxt {
            get => m_loginTxt;
            set { m_loginTxt = value; CheckCanLogin(); }
        }
        private bool m_canLogin = false;
        public bool CanLogin {
            get => m_canLogin;
            private set { m_canLogin = value; OnPropertyChanged(); }
        }
        private bool m_loginProcessing = false;
        public bool LoginProcessing {
            get => m_loginProcessing;
            set { m_loginProcessing = value; CheckCanLogin(); OnPropertyChanged(); }
        }

        private void PassTxt_PasswordChanged(object sender, RoutedEventArgs e) {
            CheckCanLogin();
        }

        private async void ActionLoginAsync() {
            LoginProcessing = true;
            await CtrlDbCtx.Instance.LoginAsync(LoginTxt, wndLogin.PassTxt.Password).ConfigureAwait(false);
            LoginProcessing = false;
            if (CtrlDbCtx.Instance.LoggedIn)
            {
                wndLogin.PassTxt.PasswordChanged -= PassTxt_PasswordChanged;
                wndLogin.Close();
            }
            else
            {
                string title = "Ошибка входа в систему";
                string msg = "Нет такой пары login, password. Попытайтесь ещё разок. ;)";
                _ = MessageBoxEx.Show(wndLogin, msg, title, MessageBoxButton.OK);
                wndLogin.loginTxt.SelectAll();
            }
        }

        private void CheckCanLogin() {
            CanLogin = false == m_loginProcessing
                && false == string.IsNullOrEmpty(LoginTxt)
                && false == string.IsNullOrEmpty(wndLogin.PassTxt.Password);
        }
    }
}
