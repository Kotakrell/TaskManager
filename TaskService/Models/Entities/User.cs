using System;
using System.Collections.Generic;

namespace TaskService.Models.Entities;

public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime? CreationDate { get; set; }

    public virtual ICollection<TaskAssignment> TaskAssignments { get; set; } = new List<TaskAssignment>();
}
