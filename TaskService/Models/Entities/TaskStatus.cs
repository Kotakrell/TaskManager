using System;
using System.Collections.Generic;

namespace TaskService.Models.Entities;

public partial class TaskStatus
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<TaskHistory> TaskHistoryIdNewStatusNavigations { get; set; } = new List<TaskHistory>();

    public virtual ICollection<TaskHistory> TaskHistoryIdOldStatusNavigations { get; set; } = new List<TaskHistory>();

    public virtual ICollection<TaskEntity> Tasks { get; set; } = new List<TaskEntity>();
}
