using System;
using System.Collections.Generic;

namespace NotificationService.Models.Entities;

public partial class Notification
{
    public int Id { get; set; }
    public int IdUser { get; set; }
    public int IdTask { get; set; }

    public int? IdType { get; set; }

    public string Message { get; set; } = null!;

    public DateTime CreateTime { get; set; }

    public virtual ICollection<NotificationReadStatus> NotificationReadStatuses { get; set; } = new List<NotificationReadStatus>();

    public virtual NotificationType Type { get; set; }
}
