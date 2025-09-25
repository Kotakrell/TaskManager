﻿using System;
using System.Collections.Generic;

namespace NotificationService.Models.Entities;

public partial class NotificationType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
