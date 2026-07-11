using System;
using System.Collections.Generic;

namespace IsraelFootballFansEvents.DATA.Models;

public partial class Session
{
    public int Id { get; set; }

    public int EventId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? SpeakerName { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public string? RoomName { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual ICollection<SessionRegistration> SessionRegistrations { get; set; } = new List<SessionRegistration>();
}
