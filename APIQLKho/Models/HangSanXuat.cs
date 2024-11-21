using System;
using System.Collections.Generic;

namespace APIQLKho.Models;

public partial class HangSanXuat
{
    public int MaHangSanXuat { get; set; }

    public string? TenHangSanXuat { get; set; }

    public bool? Hide { get; set; }

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
