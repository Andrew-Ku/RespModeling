using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using AutoMapper;
using MyFirstWPF.Infrastructure;
using MyFirstWPF.Profile;
using MyFirstWPF.ViewModel;

namespace MyFirstWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //Запуск одной копии приложения
        System.Threading.Mutex mut;
        private void App_Startup(object sender, StartupEventArgs e)
        {
            bool createdNew;
            string mutName = "Приложение";
            mut = new System.Threading.Mutex(true, mutName, out createdNew);
            if (!createdNew)
            {
                Shutdown();
            }

          AutoMapperBootstrapper.Configuration();

          var mw = new MainWindow
          {
              DataContext = new MainWindowVm()
          };

          mw.Show();
        }
    }
}
