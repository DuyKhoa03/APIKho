using System;
using System.Collections.Generic;

namespace APIQLKho.Models;

public partial class KhachHang
{
    public int MaKhachHang { get; set; }

    public string? TenKhachHang { get; set; }

    public string? SoDt { get; set; }

    public string? Diachi { get; set; }

    public string? Email { get; set; }

    public int MaLoai { get; set; }

    public bool? Hide { get; set; }

    public virtual LoaiKhacHang MaLoaiNavigation { get; set; } = null!;

    public virtual ICollection<PhieuXuatHang> PhieuXuatHangs { get; set; } = new List<PhieuXuatHang>();
}
