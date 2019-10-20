using System;
using System.Collections.Generic;
using System.Windows;
using SysTechTest.cmd;
using SysTechTest.dal;

namespace SysTechTest.gui
{
    public class VMLogin
    {
        private readonly WndLoginDlg wndLogin;
        private bool m_loginProcessing = false;
        private List<UIElement> m_listUI;
        public string LoginTxt { get; set; }
        public CmdAction CommandLogin { get; private set; }
        public CmdAction CommandCloseApp { get; private set; }
        public VMLogin(WndLoginDlg wndLogin) {
            if(wndLogin == null)
            {
                throw new ArgumentNullException(nameof(wndLogin), "VMLogin constructor: ArgumentNullException");
            }
            CommandLogin = new CmdAction(arg => ActionLoginAsync());
            CommandCloseApp = new CmdAction(arg => Application.Current.Shutdown());
            this.wndLogin = wndLogin;
            wndLogin.loginTxt.Focus();
            m_listUI = new List<UIElement>() { wndLogin.btnLogin, wndLogin.btnCancel, wndLogin.loginTxt, wndLogin.PassTxt };
        }
        private async void ActionLoginAsync() {
            if(false == CanLogin(EventArgs.Empty))
            {
                return;
            }

            foreach (var ui in m_listUI)
            {
                ui.IsEnabled = false;
            }
            m_loginProcessing = true;
            await CtrlDbCtx.Instance.LoginAsync(LoginTxt, wndLogin.PassTxt.Password).ConfigureAwait(false);
            m_loginProcessing = false;
            if (CtrlDbCtx.Instance.LoggedIn)
            {
                wndLogin.Close();
            }
            else
            {
                foreach (var ui in m_listUI)
                {
                   ui.IsEnabled = true;
                }
                wndLogin.loginTxt.SelectAll();
                MessageBox.Show(wndLogin, "Нет такой пары login, password. Попытайтесь ещё разок. ;)");
            }
        }
        private bool CanLogin(object parameter) {
            var res = false == m_loginProcessing
                && false == string.IsNullOrEmpty(wndLogin.loginTxt.Text)
                && false == string.IsNullOrEmpty(wndLogin.PassTxt.Password);

            return res;
        }
    }
}
