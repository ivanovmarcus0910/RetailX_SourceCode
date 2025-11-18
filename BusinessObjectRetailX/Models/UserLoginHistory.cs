using System;
using System.Collections.Generic;

namespace BusinessObjectRetailX.Models;

public partial class UserLoginHistory
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int? TenantId { get; set; }

    public DateTime? LoginTime { get; set; }

    public string? IpAddress { get; set; }

    public string? Device { get; set; }

    public virtual Tenant? Tenant { get; set; }

    public virtual User User { get; set; } = null!;
}
