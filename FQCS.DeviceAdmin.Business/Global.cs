using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FQCS.DeviceAdmin.Business.Models;
using FQCS.DeviceAdmin.Business.Services;
using FQCS.DeviceAdmin.Data.Models;
using TNT.Core.Helpers.DI;

namespace FQCS.DeviceAdmin.Business
{
    public class Global
    {
        public static Random Random { get; private set; }
        public static IMapper Mapper { get; private set; }

        private static void InitAutoMapper()
        {
            //AutoMapper
            var mapConfig = new MapperConfiguration(cfg =>
            {
                var dType = typeof(IMappingModel);
                var modelTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(t => t.GetTypes())
                    .Where(t => dType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);
                var maps = new Dictionary<Type, Type>();
                foreach (var t in modelTypes)
                {
                    var genArgs = t.BaseType?.GetGenericArguments().FirstOrDefault();
                    if (genArgs != null) cfg.CreateMap(genArgs, t).ReverseMap();
                }
                //extra
            });
            Mapper = mapConfig.CreateMapper();
        }

        private static void InitDI(IServiceCollection services)
        {
            ServiceInjection.Register(new List<Assembly>()
            {
                Assembly.GetExecutingAssembly()
            });

            //extra
            var baseServiceType = typeof(Service);
            var serviceTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(t => t.GetTypes())
                .Where(t => baseServiceType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);
            foreach (var t in serviceTypes)
                services.AddScoped(t.GetInterfaces()[0], t);
        }

        public static void Init(IServiceCollection services)
        {
            Random = new Random();
            InitAutoMapper();
            InitDI(services);
        }

    }

}
