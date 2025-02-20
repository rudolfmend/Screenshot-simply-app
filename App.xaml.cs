using System.Configuration;
using System.Data;
using System.Windows;
using System.Runtime.InteropServices;
using Application = System.Windows.Application;

namespace Screenshot_2_WpfApp
{
    public partial class App : Application
    {
        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool SetProcessDpiAwarenessContext(int dpiFlag);

        private const int DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 = -4;

        protected override void OnStartup(StartupEventArgs e)
        {
            //  DPI-awareness
            SetProcessDpiAwarenessContext(DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);

            base.OnStartup(e);
        }
    }
}
