using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class PurchaseOrder
{
    public int PurchaseOrderId { get; set; }

    public int SupplierId { get; set; }

    public DateTime CreateDate { get; set; }

    public byte Status { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; } = new List<PurchaseOrderDetail>();

    public virtual Supplier Supplier { get; set; } = null!;
}
