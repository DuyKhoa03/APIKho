using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace APIQLKho.Models;

public partial class PhieuXuatHang
{
    public int MaPhieuXuatHang { get; set; }

    public int MaNguoiDung { get; set; }

    public int MaKhachHang { get; set; }

    public DateTime? NgayXuat { get; set; }

    public string? HinhThucThanhToan { get; set; }

    public string? PhiVanChuyen { get; set; }

    public int? TrangThai { get; set; }

    public virtual ICollection<ChiTietPhieuXuatHang> ChiTietPhieuXuatHangs { get; set; } = new List<ChiTietPhieuXuatHang>();
    [JsonIgnore] // Bỏ qua thuộc tính này khi chuyển đổi từ JSON
    public virtual KhachHang MaKhachHangNavigation { get; set; } = null!;
    [JsonIgnore] // Bỏ qua thuộc tính này khi chuyển đổi từ JSON
    public virtual NguoiDung MaNguoiDungNavigation { get; set; } = null!;
}
