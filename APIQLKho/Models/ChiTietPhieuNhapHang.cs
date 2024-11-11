using System;
using System.Collections.Generic;

namespace APIQLKho.Models;

public partial class ChiTietPhieuNhapHang
{
    public int MaPhieuNhapHang { get; set; }

    public int MaSanPham { get; set; }

    public int? SoLuong { get; set; }

    public decimal? DonGiaNhap { get; set; }

    public int? TrangThai { get; set; }

    public string? Image { get; set; }

    public virtual PhieuNhapHang MaPhieuNhapHangNavigation { get; set; } = null!;

    public virtual SanPham MaSanPhamNavigation { get; set; } = null!;
}
