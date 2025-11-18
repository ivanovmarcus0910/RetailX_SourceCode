using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class Staff
{
    public int StaffId { get; set; }

    public string StaffName { get; set; } = null!;

    public byte Role { get; set; }

    public string Phone { get; set; } = null!;

    public string? Email { get; set; }

    public string? Address { get; set; }

    public decimal? BaseSalary { get; set; }

    public virtual ICollection<Log> Logs { get; set; } = new List<Log>();

    public virtual ICollection<ReportRevenue> ReportRevenues { get; set; } = new List<ReportRevenue>();

    public virtual Order StaffNavigation { get; set; } = null!;
}
