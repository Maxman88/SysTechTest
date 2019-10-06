using SysTechTest.dal;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Windows;

namespace SysTechTest.gui
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class WndMain : Window
    {
        private CtrlDbCtx m_ctx = CtrlDbCtx.Instance;

        public WndMain()
        {
            InitializeComponent();

        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await m_ctx.InitializeAsync();
            await m_ctx.LoginAsync("root", "1");
            List<Employee> tt = m_ctx.Employees;
            var u = m_ctx.LoggedIn;

        }
    }
}
