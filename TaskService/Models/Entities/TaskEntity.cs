using System;
using System.Collections.Generic;

namespace TaskService.Models.Entities;

public partial class TaskEntity
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int IdStatus { get; set; }

    public DateTime? CreateTime { get; set; }

    public virtual TaskStatus IdStatusNavigation { get; set; } = null!;

    public virtual ICollection<TaskAssignment> TaskAssignments { get; set; } = new List<TaskAssignment>();

    public virtual ICollection<TaskHistory> TaskHistories { get; set; } = new List<TaskHistory>();
}
