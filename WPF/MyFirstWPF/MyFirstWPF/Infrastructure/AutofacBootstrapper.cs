using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using MyFirstWPF.Services;
using MyFirstWPF.Services.Interfaces;
using IContainer = Autofac.IContainer;

namespace MyFirstWPF.Infrastructure
{
    public static class AutofacBootstrapper
    {
        private static IContainer Container { get; set; }

        public static void Configuration()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<FileService>().As<IFileService>();

            Container = builder.Build();
        }

        public static T Resolve<T>()
        {
            if (Container.IsRegistered<T>())
                return Container.Resolve<T>();

            throw new Exception("Autofac не может разрешить тип");
        }
    }
}
