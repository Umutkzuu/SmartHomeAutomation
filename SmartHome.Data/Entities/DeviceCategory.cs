using System;
using System.Collections.Generic;

namespace SmartHome.Data.Entities;

public partial class DeviceCategory
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public string? UnitSymbol { get; set; }

    public virtual ICollection<Device> Devices { get; set; } = new List<Device>();
}
