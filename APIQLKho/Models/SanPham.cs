using System;
using System.Collections.Generic;

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

    public double? KhoiLuong { get; set; }

    public string? KichThuoc { get; set; }

    public string? XuatXu { get; set; }

    public string? Image { get; set; }

    public string? Image2 { get; set; }

    public string? Image3 { get; set; }

    public string? Image4 { get; set; }

    public string? Image5 { get; set; }

    public string? MaVach { get; set; }

    public bool? Hide { get; set; }

    public DateTime? NgayTao { get; set; }

    public DateTime? NgayCapNhat { get; set; }

    public bool? TrangThai { get; set; }

    public int MaNhaCungCap { get; set; }

    public virtual ICollection<ChiTietKiemKe> ChiTietKiemKes { get; set; } = new List<ChiTietKiemKe>();

    public virtual ICollection<ChiTietPhieuNhapHang> ChiTietPhieuNhapHangs { get; set; } = new List<ChiTietPhieuNhapHang>();

    public virtual ICollection<ChiTietPhieuXuatHang> ChiTietPhieuXuatHangs { get; set; } = new List<ChiTietPhieuXuatHang>();

    public virtual HangSanXuat MaHangSanXuatNavigation { get; set; } = null!;

    public virtual LoaiSanPham MaLoaiSanPhamNavigation { get; set; } = null!;

    public virtual NhaCungCap MaNhaCungCapNavigation { get; set; } = null!;

    public virtual ICollection<SanPhamViTri> SanPhamViTris { get; set; } = new List<SanPhamViTri>();
}
