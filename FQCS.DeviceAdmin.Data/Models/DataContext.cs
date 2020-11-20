using Microsoft.AspNetCore.Identity;
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
    public partial class DataContext : IdentityDbContext<AppUser, AppRole, string, IdentityUserClaim<string>,
        AppUserRole, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public DataContext()
        {
        }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Resource> Resource { get; set; }
        public virtual DbSet<QCEvent> QCEvent { get; set; }
        public virtual DbSet<QCEventDetail> QCEventDetail { get; set; }
        public virtual DbSet<DeviceConfig> DeviceConfig { get; set; }
        public virtual DbSet<AppClient> AppClient { get; set; }

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

                // init data
                entity.HasData(new AppRole
                {
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    Id = Guid.NewGuid().ToString(),
                    Name = Constants.RoleName.ADMIN,
                    NormalizedName = Constants.RoleName.ADMIN.ToUpper()
                }, new AppRole
                {
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    Id = Guid.NewGuid().ToString(),
                    Name = Constants.RoleName.DEVICE,
                    NormalizedName = Constants.RoleName.DEVICE.ToUpper()
                });
            });
            modelBuilder.Entity<AppUserRole>(entity =>
            {
                entity.HasOne(e => e.User)
                    .WithMany(e => e.UserRoles)
                    .HasForeignKey(e => e.UserId);
                entity.HasOne(e => e.Role)
                    .WithMany(e => e.UserRoles)
                    .HasForeignKey(e => e.RoleId);
            });
            modelBuilder.Entity<Resource>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(255);
            });
            modelBuilder.Entity<QCEvent>(entity =>
            {
                entity.Property(e => e.Id)
                    .IsUnicode(false)
                    .HasMaxLength(255);
                entity.Property(e => e.LeftImage)
                    .IsUnicode(false)
                    .HasMaxLength(2000);
                entity.Property(e => e.RightImage)
                    .IsUnicode(false)
                    .HasMaxLength(2000);
                entity.Property(e => e.SideImages)
                    .IsUnicode(false)
                    .HasMaxLength(2000);
                entity.Property(e => e.Seen)
                    .HasDefaultValue(false);
            });
            modelBuilder.Entity<QCEventDetail>(entity =>
            {
                entity.Property(e => e.Id)
                    .IsUnicode(false)
                    .HasMaxLength(255);
                entity.Property(e => e.DefectTypeCode)
                    .IsUnicode(false)
                    .HasMaxLength(100);
                entity.HasOne(e => e.Event)
                    .WithMany(e => e.Details)
                    .HasForeignKey(e => e.EventId)
                    .IsRequired(true)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_QCEvent_QCEventDetail");
            });
            modelBuilder.Entity<DeviceConfig>(entity =>
            {
                entity.Property(e => e.Identifier)
                    .IsUnicode(false)
                    .HasMaxLength(100);
                entity.Property(e => e.KafkaServer)
                    .HasMaxLength(255);
                entity.Property(e => e.KafkaUsername)
                    .HasMaxLength(255);
                entity.Property(e => e.KafkaPassword)
                    .HasMaxLength(2000);
            });
            modelBuilder.Entity<AppClient>(entity =>
            {
                entity.Property(e => e.Id)
                    .IsUnicode(false)
                    .HasMaxLength(100);
                entity.Property(e => e.SecretKey)
                    .IsUnicode(false)
                    .HasMaxLength(100);
                entity.Property(e => e.ClientName)
                    .HasMaxLength(255);
                entity.Property(e => e.Description)
                    .HasMaxLength(2000);
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
