using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Autofac;
using Autofac.Core;
using AutoMapper;
using MyFirstWPF.Infrastructure;
using MyFirstWPF.Profile;
using MyFirstWPF.Services;
using MyFirstWPF.Services.Interfaces;

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
            AutofacBootstrapper.Configuration();

            var window = new MainWindow();
            window.Show();
        }

        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("Error: " + e.Exception.Message + "\nStack: " + e.Exception.StackTrace);
            e.Handled = true;
        }
    }
}
