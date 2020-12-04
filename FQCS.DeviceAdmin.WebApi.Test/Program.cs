using FQCS.DeviceAdmin.WebApi.Test.Controllers;
using System;
using System.Collections.Generic;
using System.Text;

namespace FQCS.DeviceAdmin.WebApi.Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var userControllerLoginTest = new UsersController_Login();
            userControllerLoginTest.SetUp();
            try
            {
                userControllerLoginTest.Login_Success().Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            userControllerLoginTest.TearDown();
        }
    }
}
