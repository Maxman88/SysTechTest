using System;
using System.Windows;
using SysTechTest.cmd;
using SysTechTest.dal;

namespace SysTechTest.gui
{
    public class VMWndMain : VMNotifyBase
    {
        private bool m_subscribeLoginSuccess = false;

        public VMWndMain() {
            CmdWindowLoaded = new CmdAction(arg => WindowLoadedAsync());
            CmdShowWndSettings = new CmdAction(arg => ShowToolWndDlg(new WndSettings(), new VMSettings()))
            {
                CanExecuteDelegate = SettingsAvailable
            };
        }
        ~VMWndMain() {
            if(m_subscribeLoginSuccess)
            {
                CtrlDbCtx.Instance.LoginSuccess -= LoginSuccess;
            }
        }
        
        public VMTreeEmployees VMTreeViewEmployees { get; private set; } = new VMTreeEmployees();

        public CmdAction CmdCloseApp { get; private set; } = new CmdCloseApp();
        public CmdAction CmdWindowLoaded { get; private set; }
        public CmdAction CmdShowWndSettings { get; private set; }
        private Visibility m_mainVisibility = Visibility.Hidden;
        public Visibility MainVisibility { get => m_mainVisibility; private set { m_mainVisibility = value; OnPropertyChanged(); } }
        private void WindowLoadedAsync() {
            CtrlDbCtx.Instance.LoginSuccess += LoginSuccess;
            m_subscribeLoginSuccess = true;
            MainVisibility = Visibility.Hidden;
            bool autoEnter = false;
            if(autoEnter)
            {
                CtrlDbCtx.Instance.LoginAsync("root", "1").ConfigureAwait(false);
            }
            else
            {
                var wndLogin = new WndLoginDlg() { Owner = App.Current.MainWindow };
                wndLogin.DataContext = new VMLogin(wndLogin);
                wndLogin.ShowDialog();
            }
        }
        private void LoginSuccess(object sender, EventArgs e) {
            var ctrl = CtrlDbCtx.Instance;
            if (m_subscribeLoginSuccess)
            {
                ctrl.LoginSuccess -= LoginSuccess;
                m_subscribeLoginSuccess = false;
            }
            _ = ctrl.InitializeAsync();
            MainVisibility = Visibility.Visible;
            m_settingsAvailable = ctrl.AdminHere;
            CmdShowWndSettings.InvokeCanExecuteChanged();
            VMTreeViewEmployees.Update();
        }
        bool m_settingsAvailable = false;
        private bool SettingsAvailable(object obj = null) => m_settingsAvailable;
        private void ShowToolWndDlg(Window wnd, Object Context = null) {
            wnd.Owner = App.Current.MainWindow;
            wnd.DataContext = Context;
            wnd.ShowDialog();
        }
    }

}
