using System;
using System.Collections.Generic;

namespace APIQLKho.Models;

public partial class KiemKe
{
    public int MaKiemKe { get; set; }

    public int MaNhanVienKho { get; set; }

    public DateTime? NgayKiemKe { get; set; }

    public bool? Hide { get; set; }

    public virtual ICollection<ChiTietKiemKe> ChiTietKiemKes { get; set; } = new List<ChiTietKiemKe>();

    public virtual NhanVienKho MaNhanVienKhoNavigation { get; set; } = null!;
}
