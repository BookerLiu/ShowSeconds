using ShowSeconds.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ShowSeconds
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {

        System.Threading.Mutex mutex;

        public App()
        {
            this.Startup += new StartupEventHandler(App_Startup);
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            mutex = new System.Threading.Mutex(true, 
                "ShowSeconds",
                out bool ret);
            if (!ret)
            {
                Environment.Exit(0);
            }
        }
    }
}
