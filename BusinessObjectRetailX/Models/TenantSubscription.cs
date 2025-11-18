using System;
using System.Collections.Generic;

namespace BusinessObjectRetailX.Models;

public partial class TenantSubscription
{
    public int Id { get; set; }

    public int TenantId { get; set; }

    public int PackageId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual Package Package { get; set; } = null!;

    public virtual Tenant Tenant { get; set; } = null!;
}
