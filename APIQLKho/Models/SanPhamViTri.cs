using System;
using System.Collections.Generic;

namespace APIQLKho.Models;

public partial class SanPhamViTri
{
    public int MaViTri { get; set; }

    public int MaSanPham { get; set; }

    public int? SoLuong { get; set; }

    public virtual SanPham MaSanPhamNavigation { get; set; } = null!;

    public virtual Vitri MaViTriNavigation { get; set; } = null!;
}
