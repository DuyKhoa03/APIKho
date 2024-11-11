using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace APIQLKho.Models;

public partial class SanPham
{
    public int MaSanPham { get; set; }

    public int MaLoaiSanPham { get; set; }

    public int MaHangSanXuat { get; set; }

    public string? TenSanPham { get; set; }

    public string? Mota { get; set; }

    public int? SoLuong { get; set; }

    public decimal? DonGia { get; set; }

    public string? XuatXu { get; set; }

    public string? Image { get; set; }

    public virtual ICollection<ChiTietKiemKe> ChiTietKiemKes { get; set; } = new List<ChiTietKiemKe>();

    public virtual ICollection<ChiTietPhieuNhapHang> ChiTietPhieuNhapHangs { get; set; } = new List<ChiTietPhieuNhapHang>();

    public virtual ICollection<ChiTietPhieuXuatHang> ChiTietPhieuXuatHangs { get; set; } = new List<ChiTietPhieuXuatHang>();

    public virtual ICollection<DoanhThu> DoanhThus { get; set; } = new List<DoanhThu>();
    [JsonIgnore] // Bỏ qua thuộc tính này khi chuyển đổi từ JSON
    public virtual HangSanXuat MaHangSanXuatNavigation { get; set; } = null!;
    [JsonIgnore] // Bỏ qua thuộc tính này khi chuyển đổi từ JSON
    public virtual LoaiSanPham MaLoaiSanPhamNavigation { get; set; } = null!;
}
