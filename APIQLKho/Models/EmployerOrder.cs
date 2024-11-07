using System;
using System.Collections.Generic;

namespace APIQLKho.Models;

public partial class EmployerOrder
{
    public int EmployeeId { get; set; }

    public int OrderId { get; set; }

    public string? Role { get; set; }

    public DateTime? DateAssigned { get; set; }

    public string? ActionType { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
