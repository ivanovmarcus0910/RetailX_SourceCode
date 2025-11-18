using System;
using System.Collections.Generic;

namespace BusinessObjectRetailX.Models;

public partial class Package
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? MaxUsers { get; set; }

    public int? MaxWarehouses { get; set; }

    public int? MaxProducts { get; set; }

    public decimal? MonthlyPrice { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<TenantSubscription> TenantSubscriptions { get; set; } = new List<TenantSubscription>();
}
