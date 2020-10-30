using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FQCS.DeviceAdmin.Data.Models
{
    public class TransDbContextOptions
    {
        public TransDbContextOptions(DbContextOptions<DataContext> options, SqlTransaction transaction)
        {
            Options = options;
            Transaction = transaction;
        }
        public SqlTransaction Transaction { get; }
        public DbContextOptions<DataContext> Options { get; }
    }
}
