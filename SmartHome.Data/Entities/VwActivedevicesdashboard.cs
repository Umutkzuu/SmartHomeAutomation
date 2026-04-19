using System;
using System.Collections.Generic;

namespace SmartHome.Data.Entities;

public partial class VwActivedevicesdashboard
{
    public int DeviceId { get; set; }

    public string DeviceName { get; set; } = null!;

    public string RoomName { get; set; } = null!;

    public string CategoryName { get; set; } = null!;

    public decimal? CurrentValue { get; set; }

    public string? UnitSymbol { get; set; }
}
