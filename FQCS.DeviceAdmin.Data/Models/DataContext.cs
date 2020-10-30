using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Text;
using static FQCS.DeviceAdmin.Data.Constants;

namespace FQCS.DeviceAdmin.Data.Models
{
    public partial class DataContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public DataContext()
        {
        }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Resource> Resource { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer(Constants.Data.CONN_STR)
                    .UseLazyLoadingProxies();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.Property(e => e.Id)
                    .IsUnicode(false)
                    .HasMaxLength(100);
            });
            modelBuilder.Entity<AppRole>(entity =>
            {
                entity.Property(e => e.Id)
                    .IsUnicode(false)
                    .HasMaxLength(100);
            });
            modelBuilder.Entity<Resource>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(255);
            });
        }
    }

    public class DbContextFactory : IDesignTimeDbContextFactory<DataContext>
    {

        public DataContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
            optionsBuilder.UseSqlServer(Constants.Data.CONN_STR);
            return new DataContext(optionsBuilder.Options);
        }
    }
}
