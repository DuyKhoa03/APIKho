using System;
using System.Collections.Generic;

namespace APIQLKho.Models;

public partial class LoaiKhacHang
{
    public int MaLoai { get; set; }

    public string? TenLoai { get; set; }

    public decimal? ChietKhauXuatHang { get; set; }

    public decimal? ChiPhiVanChuyen { get; set; }

    public virtual ICollection<KhachHang> KhachHangs { get; set; } = new List<KhachHang>();
}
