using System;
using System.Collections.Generic;

namespace APIQLKho.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int CustomerId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? Image { get; set; }

    public string? Status { get; set; }

    public decimal? TotalAmount { get; set; }

    public decimal? Discount { get; set; }

    public bool? Hide { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<EmployerOrder> EmployerOrders { get; set; } = new List<EmployerOrder>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
