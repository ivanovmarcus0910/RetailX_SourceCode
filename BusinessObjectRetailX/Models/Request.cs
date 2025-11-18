using System;
using System.Collections.Generic;

namespace BusinessObjectRetailX.Models;

public partial class Request
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int TenantId { get; set; }

    public virtual Tenant Tenant { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
