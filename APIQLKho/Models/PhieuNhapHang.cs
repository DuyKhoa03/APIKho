using System;
using System.Collections.Generic;

namespace APIQLKho.Models;

public partial class PhieuNhapHang
{
    public int MaPhieuNhapHang { get; set; }

    public int MaNguoiDung { get; set; }

    public int MaNhaCungCap { get; set; }

    public DateTime? NgayNhap { get; set; }

    public decimal? PhiVanChuyen { get; set; }

    public int? TrangThai { get; set; }

    public bool? Hide { get; set; }

    public virtual ICollection<ChiTietPhieuNhapHang> ChiTietPhieuNhapHangs { get; set; } = new List<ChiTietPhieuNhapHang>();

    public virtual NguoiDung MaNguoiDungNavigation { get; set; } = null!;

    public virtual NhaCungCap MaNhaCungCapNavigation { get; set; } = null!;
}
