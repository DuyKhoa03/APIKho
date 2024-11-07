using System;
using System.Collections.Generic;

namespace APIQLKho.Models;

public partial class EmployeePurchaseOrder
{
    public int EmployeeId { get; set; }

    public int PurchaseOrderId { get; set; }

    public string? Role { get; set; }

    public DateTime? DateAssigned { get; set; }

    public string? ActionType { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual PurchaseOrder PurchaseOrder { get; set; } = null!;
}
