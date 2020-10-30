using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FQCS.DeviceAdmin.Data.Models;
using TNT.Core.Helpers.DI;

namespace FQCS.DeviceAdmin.Data
{

    public class Global
    {
        private static void InitDI(IServiceCollection services)
        {
            services.AddScoped<DataContext>()
                //return only 1 scoped context 
                .AddScoped<DbContext>(p => p.GetRequiredService<DataContext>());
            ServiceInjection.Register(new List<Assembly>()
            {
                Assembly.GetExecutingAssembly()
            });
        }

        public static void Init(IServiceCollection services)
        {
            InitDI(services);
        }

    }

}
