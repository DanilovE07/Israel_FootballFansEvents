using System;
using System.Collections.Generic;

namespace IsraelFootballFansEvents.DATA.Models;

public partial class User
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public virtual ICollection<SessionRegistration> SessionRegistrations { get; set; } = new List<SessionRegistration>();
}
