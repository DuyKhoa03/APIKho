using System;
using System.Collections.Generic;

namespace APIQLKho.Models;

public partial class StorageLocation
{
    public int LocationId { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
