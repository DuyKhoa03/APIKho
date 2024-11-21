using System;
using System.Collections.Generic;

namespace APIQLKho.Models;

public partial class DoanhThu
{
    public int MaDoanhThu { get; set; }

    public int MaSanPham { get; set; }

    public decimal? TongPhiNhap { get; set; }

    public decimal? TongPhiXuat { get; set; }

    public decimal? PhiVanHanh { get; set; }

    public decimal? DoanhThuNgay { get; set; }

    public DateTime? NgayCapNhat { get; set; }

    public bool? Hide { get; set; }

    public virtual SanPham MaSanPhamNavigation { get; set; } = null!;
}
