using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace APIQLKho.Models;

public partial class KiemKe
{
    public int MaKiemKe { get; set; }

    public int MaNhanVienKho { get; set; }

    public DateTime? NgayKiemKe { get; set; }

    public virtual ICollection<ChiTietKiemKe> ChiTietKiemKes { get; set; } = new List<ChiTietKiemKe>();
    [JsonIgnore] // Bỏ qua thuộc tính này khi chuyển đổi từ JSON
    public virtual NhanVienKho MaNhanVienKhoNavigation { get; set; } = null!;
}
