using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace APIQLKho.Models;

public partial class PhieuNhapHang
{
    public int MaPhieuNhapHang { get; set; }

    public int MaNguoiDung { get; set; }

    public int MaNhaCungCap { get; set; }

    public DateTime? NgayNhap { get; set; }

    public decimal? PhiVanChuyen { get; set; }

    public int? TrangThai { get; set; }

    public virtual ICollection<ChiTietPhieuNhapHang> ChiTietPhieuNhapHangs { get; set; } = new List<ChiTietPhieuNhapHang>();
    [JsonIgnore] // Bỏ qua thuộc tính này khi chuyển đổi từ JSON
    public virtual NguoiDung MaNguoiDungNavigation { get; set; } = null!;
    [JsonIgnore] // Bỏ qua thuộc tính này khi chuyển đổi từ JSON
    public virtual NhaCungCap MaNhaCungCapNavigation { get; set; } = null!;
}
