using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TaskService.Models.Entities;

public partial class TaskServiceContext : DbContext
{
    public TaskServiceContext()
    {
    }

    public TaskServiceContext(DbContextOptions<TaskServiceContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TaskEntity> Tasks { get; set; }

    public virtual DbSet<TaskAssignment> TaskAssignments { get; set; }

    public virtual DbSet<TaskHistory> TaskHistories { get; set; }

    public virtual DbSet<TaskStatus> TaskStatuses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Port=5432;Host=localhost;Username=postgres;Database=task_service;Password=root");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tasks_pkey");

            entity.ToTable("tasks");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_time");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IdStatus).HasColumnName("id_status");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");

            entity.HasOne(d => d.IdStatusNavigation).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.IdStatus)
                .HasConstraintName("tasks_id_status_fkey");
        });

        modelBuilder.Entity<TaskAssignment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("task_assignments_pkey");

            entity.ToTable("task_assignments");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AssigneTime)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("assigne_time");
            entity.Property(e => e.IdTask).HasColumnName("id_task");
            entity.Property(e => e.IdUser).HasColumnName("id_user");

            entity.HasOne(d => d.IdTaskNavigation).WithMany(p => p.TaskAssignments)
                .HasForeignKey(d => d.IdTask)
                .HasConstraintName("task_assignments_id_task_fkey");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.TaskAssignments)
                .HasForeignKey(d => d.IdUser)
                .HasConstraintName("task_assignments_id_user_fkey");
        });

        modelBuilder.Entity<TaskHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("task_history_pkey");

            entity.ToTable("task_history");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ChangeTime)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("change_time");
            entity.Property(e => e.IdNewStatus).HasColumnName("id_new_status");
            entity.Property(e => e.IdOldStatus).HasColumnName("id_old_status");
            entity.Property(e => e.IdTask).HasColumnName("id_task");

            entity.HasOne(d => d.IdNewStatusNavigation).WithMany(p => p.TaskHistoryIdNewStatusNavigations)
                .HasForeignKey(d => d.IdNewStatus)
                .HasConstraintName("task_history_id_new_status_fkey");

            entity.HasOne(d => d.IdOldStatusNavigation).WithMany(p => p.TaskHistoryIdOldStatusNavigations)
                .HasForeignKey(d => d.IdOldStatus)
                .HasConstraintName("task_history_id_old_status_fkey");

            entity.HasOne(d => d.IdTaskNavigation).WithMany(p => p.TaskHistories)
                .HasForeignKey(d => d.IdTask)
                .HasConstraintName("task_history_id_task_fkey");
        });

        modelBuilder.Entity<TaskStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("task_statuses_pkey");

            entity.ToTable("task_statuses");

            entity.HasIndex(e => e.Name, "task_statuses_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("creation_date");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
