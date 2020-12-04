using FQCS.DeviceAdmin.Business.Models;
using FQCS.DeviceAdmin.Business.Services;
using FQCS.DeviceAdmin.Data.Models;
using FQCS.DeviceAdmin.WebApi.Controllers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TNT.Core.Helpers.DI;

namespace FQCS.DeviceAdmin.WebApi.Test.Controllers
{
    [TestFixture]
    public class UsersController_Login
    {
        private IServiceProvider _provider;
        private AuthorizationGrantModel _validModel;
        private AuthorizationGrantModel _invalidModel;
        private ClaimsPrincipal _anonymous;

        [SetUp]
        public void SetUp()
        {
            var services = new ServiceCollection();
            services.AddServiceInjection();
            ServiceInjection.Register(new List<Assembly>()
            {
                Assembly.GetAssembly(typeof(WebApi.Startup))
            });
            services.AddScoped<UsersController>();
            // mocks
            _validModel = new AuthorizationGrantModel
            {
                username = "admin",
                password = "123123",
            };
            _invalidModel = new AuthorizationGrantModel
            {
                username = "admin",
                password = DateTime.Now.ToString(),
            };
            _anonymous = new ClaimsPrincipal();

            var identityServiceMock = new Mock<IIdentityService>();
            identityServiceMock.Setup(o => o.ValidateLogin(It.IsAny<ClaimsPrincipal>(),
                It.IsAny<AuthorizationGrantModel>()))
                .Returns(new ValidationData());
            identityServiceMock.Setup(o => o.AuthenticateAsync(
                It.Is<string>(u => u == _validModel.username),
                It.Is<string>(p => p == _validModel.password)))
                .Returns(Task.FromResult(new AppUser()));
            identityServiceMock.Setup(o => o.GetIdentityAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new ClaimsIdentity()));
            identityServiceMock.Setup(o => o.GenerateTokenResponse(It.IsAny<ClaimsPrincipal>(),
                It.IsAny<AuthenticationProperties>(), It.IsAny<string>()))
                .Returns(new TokenResponseModel());

            var dbContextMock = Mock.Of<DbContext>();

            services.AddScoped(p => identityServiceMock.Object);
            services.AddScoped(p => dbContextMock);

            _provider = services.BuildServiceProvider();
        }

        [Test]
        public async Task Login_Success()
        {
            // setup
            using var scope = _provider.CreateScope();
            var provider = scope.ServiceProvider;
            var controller = provider.GetRequiredService<UsersController>();
            var serviceInj = provider.GetRequiredService<ServiceInjection>();
            serviceInj.Inject(controller);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(),
                    RequestServices = provider
                }
            };

            // excution and test
            var resultObj = await controller.LogIn(_validModel);
            Assert.IsInstanceOf(typeof(OkObjectResult), resultObj);
            var okObj = resultObj as OkObjectResult;
            Assert.AreEqual((int)HttpStatusCode.OK,
                okObj.StatusCode);
        }

        [Test]
        public async Task Login_InvalidAccountOrPassword()
        {
            // setup
            using var scope = _provider.CreateScope();
            var provider = scope.ServiceProvider;
            var controller = provider.GetRequiredService<UsersController>();
            var serviceInj = provider.GetRequiredService<ServiceInjection>();
            serviceInj.Inject(controller);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(),
                    RequestServices = provider
                }
            };

            // excution and test
            var resultObj = await controller.LogIn(_invalidModel);
            Assert.IsInstanceOf(typeof(UnauthorizedObjectResult), resultObj);
            var okObj = resultObj as UnauthorizedObjectResult;
            Assert.AreEqual((int)HttpStatusCode.Unauthorized,
                okObj.StatusCode);
        }

        [TearDown]
        public void TearDown()
        {
        }
    }

}