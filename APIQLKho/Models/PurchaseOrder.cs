using System;
using System.Collections.Generic;

namespace APIQLKho.Models;

public partial class PurchaseOrder
{
    public int PurchaseOrderId { get; set; }

    public int SupplierId { get; set; }

    public string? Status { get; set; }

    public string? Image { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public bool? Hide { get; set; }

    public virtual ICollection<EmployeePurchaseOrder> EmployeePurchaseOrders { get; set; } = new List<EmployeePurchaseOrder>();

    public virtual Supplier Supplier { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
