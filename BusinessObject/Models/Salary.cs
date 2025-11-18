using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class Salary
{
    public int SalaryId { get; set; }

    public int StaffId { get; set; }

    public decimal? Bonus { get; set; }

    public decimal? Deduction { get; set; }

    public decimal? Amount { get; set; }

    public int Month { get; set; }

    public int Year { get; set; }

    public byte Status { get; set; }

    public int? DayPayment { get; set; }

    public virtual Staff Staff { get; set; } = null!;
}
