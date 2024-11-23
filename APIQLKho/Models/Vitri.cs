using System;
using System.Collections.Generic;

namespace APIQLKho.Models;

public partial class Vitri
{
    public int MaViTri { get; set; }

    public string? KhuVuc { get; set; }

    public string? Tang { get; set; }

    public string? Ke { get; set; }

    public string? Mota { get; set; }

    public bool? Hide { get; set; }

    public virtual ICollection<SanPhamViTri> SanPhamViTris { get; set; } = new List<SanPhamViTri>();
}
