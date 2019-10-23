using System.Windows;
using SysTechTest.gui;

namespace SysTechTest
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            var wndMain = new WndMain() { DataContext = new VMWndMain() };
            wndMain.Show();
        }
    }
}
