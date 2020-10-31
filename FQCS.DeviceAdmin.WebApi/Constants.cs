using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FQCS.DeviceAdmin.WebApi
{
    public static class Constants
    {
        public static class Policy
        {
            public static class And
            {
                public const string APP_CLIENT = "And.AppClient";
                public const string AUTH_USER = "And.AuthUser";
            }
            public static class Or
            {
                public const string APP_CLIENT = "Or.AppClient";
                public const string AUTH_USER = "Or.AuthUser";
            }
        }

        public static class RequestItemKey
        {
            public const string CLIENT_ID = "client_id";
        }
    }
}
