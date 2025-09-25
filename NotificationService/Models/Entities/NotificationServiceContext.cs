using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace NotificationService.Models.Entities;

public partial class NotificationServiceContext : DbContext
{
    public NotificationServiceContext()
    {
    }

    public NotificationServiceContext(DbContextOptions<NotificationServiceContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<NotificationReadStatus> NotificationReadStatuses { get; set; }

    public virtual DbSet<NotificationType> NotificationTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Port=5432;Host=localhost;Username=postgres;Database=notification_service;Password=root");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("notifications_pkey");

            entity.ToTable("notifications");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_time");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.IdType).HasColumnName("id_type");
            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.IdTask).HasColumnName("id_task");

            entity.HasOne(d => d.Type).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.IdType)
                .HasConstraintName("notifications_type_id_fkey");
        });

        modelBuilder.Entity<NotificationReadStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("notification_read_status_pkey");

            entity.ToTable("notification_read_status");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IdNotification).HasColumnName("notification_id");
            entity.Property(e => e.ReadAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("read_at");

            entity.HasOne(d => d.Notification).WithMany(p => p.NotificationReadStatuses)
                .HasForeignKey(d => d.IdNotification)
                .HasConstraintName("notification_read_status_notification_id_fkey");
        });

        modelBuilder.Entity<NotificationType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("notification_types_pkey");

            entity.ToTable("notification_types");

            entity.HasIndex(e => e.Name, "notification_types_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
