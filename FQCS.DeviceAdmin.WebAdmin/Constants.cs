using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FQCS.DeviceAdmin.WebAdmin
{
    public static class Constants
    {

        public static class Routing
        {
            public const string DASHBOARD = "/dashboard";
            public const string LOGIN = "/identity/login";
            public const string LOGOUT = "/identity/logout";
            public const string REGISTER = "/identity/register";
            public const string IDENTITY = "/identity";
            public const string APP_USER = "/appuser";
            public const string APP_USER_CREATE = "/appuser/create";
            public const string APP_USER_DETAIL = "/appuser/{id}";
            public const string RESOURCE = "/resource";
            public const string RESOURCE_CREATE = "/resource/create";
            public const string RESOURCE_DETAIL = "/resource/{id}";
            public const string APP_CLIENT = "/appclient";
            public const string APP_CLIENT_CREATE = "/appclient/create";
            public const string APP_CLIENT_DETAIL = "/appclient/{id}";
            public const string DEVICE_CONFIG = "/deviceconfig";
            public const string DEVICE_CONFIG_CREATE = "/deviceconfig/create";
            public const string DEVICE_CONFIG_DETAIL = "/deviceconfig/{id}";
            public const string QC_EVENT = "/qcevent";
            public const string QC_EVENT_DETAIL = "/qcevent/{id}";
            public const string ADMIN_ONLY = "/adminonly";
            public const string ACCESS_DENIED = "/accessdenied";
            public const string STATUS = "/status";
            public const string ERROR = "/error";
            public const string ERROR_CONTROLLER = "error";
            public const string INDEX = "/";
        }

        public static class AppController
        {
        }

        public static class AppCookie
        {
            public const string TOKEN = "_appuat";
        }

        public static class AppView
        {
            public const string MESSAGE = "MessageView";
            public const string STATUS = "StatusView";
            public const string ERROR = "ErrorView";
        }

        public static class Menu
        {
            public const string DASHBOARD = "dashboard";
            public const string RESOURCE = "resource";
            public const string APP_CLIENT = "app_client";
            public const string DEVICE_CONFIG = "device_config";
            public const string QC_EVENT = "qc_event";
            public const string APP_USER = "app_user";
            public const string ADMIN_ONLY = "admin_only";
        }
    }

}
