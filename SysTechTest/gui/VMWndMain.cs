using System;
using System.Windows;
using System.Windows.Input;
using SysTechTest.cmd;
using SysTechTest.dal;

namespace SysTechTest.gui
{
    public class VMWndMain
    {
        private bool m_subscribeLoginSuccess;
        private readonly WndMain m_wndMain;

        public ICommand CmdWindowLoaded { get; private set; }
        public ICommand CmdShowWndSettings { get; private set; }
        public VMWndMain(WndMain wndMain) {
            m_wndMain = wndMain;
            CmdWindowLoaded = new CmdAction(async arg => await WindowLoadedAsync().ConfigureAwait(false));
            CmdShowWndSettings = new CmdAction(arg => ShowWndSettings());
        }
        ~VMWndMain() {
            if(m_subscribeLoginSuccess)
            {
                CtrlDbCtx.Instance.LoginSuccess -= LoginSuccess;
            }
        }
        private async System.Threading.Tasks.Task WindowLoadedAsync() {
            m_wndMain.MainGrid.Visibility = Visibility.Hidden;
            CtrlDbCtx.Instance.LoginSuccess += LoginSuccess;
            m_subscribeLoginSuccess = true;
            await CtrlDbCtx.Instance.LoginAsync("log1", "1").ConfigureAwait(false);
            /*var wndLogin = new WndLoginDlg() { Owner = m_wndMain };
            wndLogin.DataContext = new VMLogin(wndLogin);
            wndLogin.ShowDialog();*/
        }
        private void LoginSuccess(object sender, EventArgs e) {
            var ctrl = CtrlDbCtx.Instance;
            if(m_subscribeLoginSuccess)
            {
                ctrl.LoginSuccess -= LoginSuccess;
                m_subscribeLoginSuccess = false;
            }
            if (ctrl.CurrentUserOrNull == null)
            {
                throw new NullReferenceException();
            }
            else  {
                _ = ctrl.InitializeAsync();
            }
        }
        private void ShowWndSettings() {
            var wnd = new WndSettings() { Owner = m_wndMain, DataContext = new VMSettings() };
            wnd.ShowDialog();
        }
    }
}
