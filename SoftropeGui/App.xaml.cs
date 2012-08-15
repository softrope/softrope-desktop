using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace SoftropeGui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            SoftropeMainUi softRopeMainUi = new SoftropeMainUi();

            softRopeMainUi.Activate();
            softRopeMainUi.Show();
            if (e.Args.Length > 0 && e.Args[0].EndsWith(".softrope"))
            {
                softRopeMainUi.OpenFile(e.Args[0]); 
            }

        }
    }
}
