using System;
using System.Collections.Generic;

namespace CafeteriaManagementSystem.Models;

public partial class PurchaseOrder
{
    public int Id { get; set; }

    public DateTime OrderDate { get; set; }

    public string Status { get; set; } = null!;

    public int SupplierId { get; set; }

    public virtual ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();

    public virtual Supplier Supplier { get; set; } = null!;
}
