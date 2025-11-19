using System;
using System.Collections.Generic;

namespace BusinessObjectRetailX.Models;

public partial class Statistic
{
    public int StatId { get; set; }

    public string Day { get; set; } = null!;

    public int TotalUser { get; set; }

    public int TotalUserActive { get; set; }

    public int TotalTenant { get; set; }

    public int TotalTenantActive { get; set; }

    public int AccessCount { get; set; }
}
