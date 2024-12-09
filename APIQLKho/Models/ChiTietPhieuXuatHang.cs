using System;
using System.Collections.Generic;

namespace APIQLKho.Models;

public partial class ChiTietPhieuXuatHang
{
    public int MaSanPham { get; set; }

    public int MaPhieuXuatHang { get; set; }

    public int? SoLuong { get; set; }

    public decimal? DonGiaXuat { get; set; }

    public decimal? TienMat { get; set; }

    public decimal? NganHang { get; set; }

    public int? TrangThai { get; set; }

    public string? Image { get; set; }

    public string? Image2 { get; set; }

    public string? Image3 { get; set; }

    public string? Image4 { get; set; }

    public string? Image5 { get; set; }

    public string? Image6 { get; set; }

    public virtual PhieuXuatHang MaPhieuXuatHangNavigation { get; set; } = null!;

    public virtual SanPham MaSanPhamNavigation { get; set; } = null!;
}
