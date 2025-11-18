using System;
using System.Collections.Generic;

namespace BusinessObjectRetailX.Models;

public partial class Tenant
{
    public int Id { get; set; }

    public string CompanyName { get; set; } = null!;

    public string? LogoUrl { get; set; }

    public string? OwnerName { get; set; }

    public string? OwnerEmail { get; set; }

    public string DbServer { get; set; } = null!;

    public string DbName { get; set; } = null!;

    public string DbUser { get; set; } = null!;

    public string DbPassword { get; set; } = null!;

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();

    public virtual ICollection<TenantSubscription> TenantSubscriptions { get; set; } = new List<TenantSubscription>();

    public virtual ICollection<UserLoginHistory> UserLoginHistories { get; set; } = new List<UserLoginHistory>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
