using System;
using System.Collections.Generic;

namespace TaskService.Models.Entities;

public partial class TaskHistory
{
    public int Id { get; set; }

    public int IdTask { get; set; }

    public int? IdOldStatus { get; set; }

    public int? IdNewStatus { get; set; }

    public DateTime? ChangeTime { get; set; }

    public virtual TaskStatus? IdNewStatusNavigation { get; set; }

    public virtual TaskStatus? IdOldStatusNavigation { get; set; }

    public virtual TaskEntity IdTaskNavigation { get; set; } = null!;
}
