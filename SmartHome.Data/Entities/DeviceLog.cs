using System;
using System.Collections.Generic;

namespace SmartHome.Data.Entities;

public partial class DeviceLog
{
    public long LogId { get; set; }

    public int DeviceId { get; set; }

    public int? UserId { get; set; }

    public string ActionType { get; set; } = null!;

    public decimal? OldValue { get; set; }

    public decimal? NewValue { get; set; }

    public DateTime Timestamp { get; set; }

    public virtual Device Device { get; set; } = null!;

    public virtual User? User { get; set; }
}
