using System;
using System.Collections.Generic;

namespace NotificationService.Models.Entities;

public partial class NotificationReadStatus
{
    public int Id { get; set; }

    public int IdNotification { get; set; }

    public DateTime? ReadAt { get; set; }

    public virtual Notification Notification { get; set; } = null!;
}
