using FQCS.DeviceAdmin.Business.Models;
using FQCS.DeviceAdmin.Business.Services;
using FQCS.DeviceAdmin.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using TNT.Core.Helpers.DI;

namespace FQCS.DeviceAdmin.Business.Test.Services
{
    [TestFixture]
    public class ResourceService_Create
    {
        private IServiceProvider _provider;
        private CreateResourceModel _createResourceModel;

        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.AddServiceInjection();
            ServiceInjection.Register(new List<Assembly>()
            {
                Assembly.GetAssembly(typeof(Business.Global))
            });
            Business.Global.Init(services);

            _createResourceModel = new CreateResourceModel
            {
                Name = "Test resource"
            };

            services.AddDbContext<DataContext>(options =>
                options.UseInMemoryDatabase(databaseName: "FQCSDevice"));

            _provider = services.BuildServiceProvider();
        }

        [Test]
        public void Create_Success()
        {
            using var scope = _provider.CreateScope();
            var provider = scope.ServiceProvider;
            var resourceService = provider.GetRequiredService<IResourceService>();
            var entity = resourceService.CreateResource(_createResourceModel);
            Assert.IsInstanceOf(typeof(Resource), entity);
        }

        [TearDown]
        public void Teardown()
        {
        }
    }
}
