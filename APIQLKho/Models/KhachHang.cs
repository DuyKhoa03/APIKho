using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace APIQLKho.Models;

public partial class KhachHang
{
    public int MaKhachHang { get; set; }

    public string? TenKhachHang { get; set; }

    public int MaLoai { get; set; }
    [JsonIgnore] // Bỏ qua thuộc tính này khi chuyển đổi từ JSON
    public virtual LoaiKhacHang MaLoaiNavigation { get; set; } = null!;

    public virtual ICollection<PhieuXuatHang> PhieuXuatHangs { get; set; } = new List<PhieuXuatHang>();
}
