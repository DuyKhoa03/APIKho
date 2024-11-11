using System;
using System.Collections.Generic;

namespace APIQLKho.Models;

public partial class NhanVienKho
{
    public int MaNhanVienKho { get; set; }

    public string? TenNhanVien { get; set; }

    public string? Email { get; set; }

    public int? Sdt { get; set; }

    public string? Hinhanh { get; set; }

    public DateTime? NamSinh { get; set; }

    public virtual ICollection<KiemKe> KiemKes { get; set; } = new List<KiemKe>();
}
