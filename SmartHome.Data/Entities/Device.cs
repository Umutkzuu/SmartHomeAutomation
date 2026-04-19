using System;
using System.Collections.Generic;

namespace SmartHome.Data.Entities;

public partial class Device
{
    public int DeviceId { get; set; }

    public string DeviceName { get; set; } = null!;

    public int RoomId { get; set; }

    public int CategoryId { get; set; }

    public bool IsActive { get; set; }

    public decimal? CurrentValue { get; set; }

    public bool? IsOnline { get; set; }

    public virtual DeviceCategory Category { get; set; } = null!;

    public virtual ICollection<DeviceLog> DeviceLogs { get; set; } = new List<DeviceLog>();

    public virtual Room Room { get; set; } = null!;
}
