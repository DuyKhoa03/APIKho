using System;
using System.Collections.Generic;

namespace APIQLKho.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string? Name { get; set; }

    public string? Address { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public DateTime? AddDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public DateTime? RegisterDate { get; set; }

    public DateTime? LastLogin { get; set; }

    public bool? Hide { get; set; }

    public int? Role { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
