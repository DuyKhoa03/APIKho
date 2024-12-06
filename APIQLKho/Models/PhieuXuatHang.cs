using System;
using System.Collections.Generic;

namespace APIQLKho.Models;

public partial class PhieuXuatHang
{
    public int MaPhieuXuatHang { get; set; }

    public int MaNguoiDung { get; set; }

    public int MaKhachHang { get; set; }

    public DateTime? NgayXuat { get; set; }

    public string? HinhThucThanhToan { get; set; }

    public decimal? PhiVanChuyen { get; set; }

    public int? TrangThai { get; set; }

    public bool? Hide { get; set; }

    public virtual ICollection<ChiTietPhieuXuatHang> ChiTietPhieuXuatHangs { get; set; } = new List<ChiTietPhieuXuatHang>();

    public virtual KhachHang MaKhachHangNavigation { get; set; } = null!;

    public virtual NguoiDung MaNguoiDungNavigation { get; set; } = null!;
}
