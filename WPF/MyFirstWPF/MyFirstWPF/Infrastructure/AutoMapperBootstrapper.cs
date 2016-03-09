using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MyFirstWPF.Profile;

namespace MyFirstWPF.Infrastructure
{
    public static class AutoMapperBootstrapper
    {
        public static void Configuration()
        {
            Mapper.Initialize(mapperConfiguration =>
            {
                mapperConfiguration.AddProfile(new NodeProfile());
                mapperConfiguration.AddProfile(new StateSaveProfile());
            });
        }
    }
}
