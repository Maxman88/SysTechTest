using System.Windows;

namespace SysTechTest.cmd
{
    class CmdCloseApp : CmdAction
    {
        public CmdCloseApp() : base(arg => Application.Current.Shutdown()) { }
    }
}
