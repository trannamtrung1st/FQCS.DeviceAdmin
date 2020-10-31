using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FQCS.DeviceAdmin.Data
{

    public static class Constants
    {
        public static class Data
        {
            public const string CONN_STR = "Server=localhost;Database=FQCSDevice;Trusted_Connection=False;User Id=sa;Password=123456;MultipleActiveResultSets=true";
        }

        public static class RoleName
        {
            public const string ADMIN = "Administrator";
        }

        public static class DefectTypeCode
        {
            public const string SIZE_MISMATCH = "SIZE_MISMATCH";
            public const string COLOR_MISMATCH = "COLOR_MISMATCH ";
            public const string SAMPLE_MISMATCH = "SAMPLE_MISMATCH";
            public const string EXCESS_GLUE = "EXCESS_GLUE";
            public const string WEEK_DEGUMMING = "WEEK_DEGUMMING";
            public const string STAIN = "STAIN";
        }

    }

}
