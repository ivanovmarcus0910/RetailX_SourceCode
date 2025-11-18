using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class Log
{
    public int LogId { get; set; }

    public string Decription { get; set; } = null!;

    public byte LogLevel { get; set; }

    public DateTime? CreateDate { get; set; }

    public int StaffId { get; set; }

    public virtual Staff Staff { get; set; } = null!;
}
