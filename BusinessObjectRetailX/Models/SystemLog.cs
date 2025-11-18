using System;
using System.Collections.Generic;

namespace BusinessObjectRetailX.Models;

public partial class SystemLog
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? Action { get; set; }

    public string? Details { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual User? User { get; set; }
}
