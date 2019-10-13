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
            //tab1.Visibility = Visibility.Collapsed;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //var str = Properties.Resources.MSG_BASERATE_CALC_PERIOD_IS_NO_VALID;
           // var str = Properties.Resources.PayBaseRate_Calc_Exp_PeriodIsNotCorrected;
            await m_ctx.InitializeAsync().ConfigureAwait(false);
            await m_ctx.LoginAsync("root", "1").ConfigureAwait(false);
            List<Employee> tt = m_ctx.Employees;
            var u = m_ctx.LoggedIn;

        }
    }
}
