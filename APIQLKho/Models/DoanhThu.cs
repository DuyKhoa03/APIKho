using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

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
    [JsonIgnore] // Bỏ qua thuộc tính này khi chuyển đổi từ JSON
    public virtual SanPham MaSanPhamNavigation { get; set; } = null!;
}
