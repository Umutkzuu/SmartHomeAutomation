using System;
using System.Collections.Generic;

namespace SmartHome.Data.Entities;

public partial class Room
{
    public int RoomId { get; set; }

    public string RoomName { get; set; } = null!;

    public int FloorLevel { get; set; }

    public virtual ICollection<Device> Devices { get; set; } = new List<Device>();
}
