using System;
using System.Collections.Generic;

namespace APIQLKho.Models;

public partial class NhaCungCap
{
    public int MaNhaCungCap { get; set; }

    public string? TenNhaCungCap { get; set; }

    public string? DiaChi { get; set; }

    public string? Email { get; set; }

    public string? Sdt { get; set; }

    public string? Image { get; set; }

    public bool? Hide { get; set; }

    public virtual ICollection<PhieuNhapHang> PhieuNhapHangs { get; set; } = new List<PhieuNhapHang>();

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
