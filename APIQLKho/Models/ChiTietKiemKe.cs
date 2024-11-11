using System;
using System.Collections.Generic;

namespace APIQLKho.Models;

public partial class ChiTietKiemKe
{
    public int MaKiemKe { get; set; }

    public int MaSanPham { get; set; }

    public int? SoLuongTon { get; set; }

    public int? TrangThai { get; set; }

    public virtual KiemKe MaKiemKeNavigation { get; set; } = null!;

    public virtual SanPham MaSanPhamNavigation { get; set; } = null!;
}
