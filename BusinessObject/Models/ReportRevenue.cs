using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class ReportRevenue
{
    public int ReportRevenueId { get; set; }

    public string? DayStart { get; set; }

    public string? DayEnd { get; set; }

    public int? Month { get; set; }

    public int? Year { get; set; }

    public DateTime? CreateDate { get; set; }

    public int StaffId { get; set; }

    public decimal? AmountRevenue { get; set; }

    public decimal? AmountCost { get; set; }

    public decimal? AmountSalary { get; set; }

    public decimal? Profit { get; set; }

    public virtual Staff Staff { get; set; } = null!;
}
