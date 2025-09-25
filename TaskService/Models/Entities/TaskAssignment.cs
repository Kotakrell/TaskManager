using System;
using System.Collections.Generic;

namespace TaskService.Models.Entities;

public partial class TaskAssignment
{
    public int Id { get; set; }

    public int IdTask { get; set; }

    public int IdUser { get; set; }

    public DateTime? AssigneTime { get; set; }

    public virtual TaskEntity IdTaskNavigation { get; set; } = null!;

    public virtual User IdUserNavigation { get; set; } = null!;
}
