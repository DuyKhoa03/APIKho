using System;
using System.Collections.Generic;

namespace APIQLKho.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public int SupplierId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Image { get; set; }

    public string? Image2 { get; set; }

    public string? Image3 { get; set; }

    public decimal? Price { get; set; }

    public int? Quantity { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public DateTime? AddDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public int? Order { get; set; }

    public string? Link { get; set; }

    public bool? Hide { get; set; }

    public virtual Supplier Supplier { get; set; } = null!;

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

    public virtual ICollection<StorageLocation> Locations { get; set; } = new List<StorageLocation>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
}
