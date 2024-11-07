using System;
using System.Collections.Generic;

namespace APIQLKho.Models;

public partial class PaymentMethod
{
    public int PaymentMethodId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public bool? Hide { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
