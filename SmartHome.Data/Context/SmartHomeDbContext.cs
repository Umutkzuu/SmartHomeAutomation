using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using SmartHome.Data.Entities;

namespace SmartHome.Data.Context;

public partial class SmartHomeDbContext : DbContext
{
    public SmartHomeDbContext()
    {
    }

    public SmartHomeDbContext(DbContextOptions<SmartHomeDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Device> Devices { get; set; }

    public virtual DbSet<DeviceCategory> DeviceCategories { get; set; }

    public virtual DbSet<DeviceLog> DeviceLogs { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<VwActivedevicesdashboard> VwActivedevicesdashboards { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=127.0.0.1;port=3306;database=SmartHomeDB;user=root;password=explorer.exe", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.44-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Device>(entity =>
        {
            entity.HasKey(e => e.DeviceId).HasName("PRIMARY");

            entity.ToTable("devices");

            entity.HasIndex(e => e.CategoryId, "fk_Device_Category");

            entity.HasIndex(e => e.RoomId, "fk_Device_Room");

            entity.HasIndex(e => e.DeviceName, "idx_DeviceName");

            entity.Property(e => e.DeviceId).HasColumnName("DeviceID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CurrentValue).HasPrecision(5, 2);
            entity.Property(e => e.DeviceName).HasMaxLength(100);
            entity.Property(e => e.IsOnline)
                .IsRequired()
                .HasDefaultValueSql("'1'");
            entity.Property(e => e.RoomId).HasColumnName("RoomID");

            entity.HasOne(d => d.Category).WithMany(p => p.Devices)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Device_Category");

            entity.HasOne(d => d.Room).WithMany(p => p.Devices)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("fk_Device_Room");
        });

        modelBuilder.Entity<DeviceCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PRIMARY");

            entity.ToTable("device_categories");

            entity.HasIndex(e => e.CategoryName, "CategoryName").IsUnique();

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(50);
            entity.Property(e => e.UnitSymbol).HasMaxLength(10);
        });

        modelBuilder.Entity<DeviceLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PRIMARY");

            entity.ToTable("device_logs");

            entity.HasIndex(e => e.DeviceId, "fk_Log_Device");

            entity.HasIndex(e => e.UserId, "fk_Log_User");

            entity.HasIndex(e => e.Timestamp, "idx_LogTimestamp");

            entity.Property(e => e.LogId).HasColumnName("LogID");
            entity.Property(e => e.ActionType).HasMaxLength(50);
            entity.Property(e => e.DeviceId).HasColumnName("DeviceID");
            entity.Property(e => e.NewValue).HasPrecision(5, 2);
            entity.Property(e => e.OldValue).HasPrecision(5, 2);
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Device).WithMany(p => p.DeviceLogs)
                .HasForeignKey(d => d.DeviceId)
                .HasConstraintName("fk_Log_Device");

            entity.HasOne(d => d.User).WithMany(p => p.DeviceLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_Log_User");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("PRIMARY");

            entity.ToTable("rooms");

            entity.HasIndex(e => e.RoomName, "RoomName").IsUnique();

            entity.Property(e => e.RoomId).HasColumnName("RoomID");
            entity.Property(e => e.FloorLevel).HasDefaultValueSql("'1'");
            entity.Property(e => e.RoomName).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "Email").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.PasswordHash).HasMaxLength(256);
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasDefaultValueSql("'User'");
        });

        modelBuilder.Entity<VwActivedevicesdashboard>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_activedevicesdashboard");

            entity.Property(e => e.CategoryName).HasMaxLength(50);
            entity.Property(e => e.CurrentValue).HasPrecision(5, 2);
            entity.Property(e => e.DeviceId).HasColumnName("DeviceID");
            entity.Property(e => e.DeviceName).HasMaxLength(100);
            entity.Property(e => e.RoomName).HasMaxLength(50);
            entity.Property(e => e.UnitSymbol).HasMaxLength(10);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
